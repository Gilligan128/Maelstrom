using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using System.ComponentModel;
using Maelstrom.CharacterTracker.Web.Models;
using Maelstrom.CharacterTracker.Web.Controllers;
using System.Globalization;
using NHibernate.Metadata;
using NHibernate.Type;
using NHibernate.Engine;
using System.Collections;

namespace Maelstrom.CharacterTracker.Web.Infrastructure.Crud
{
    public class FromNHibernateModelBinderProvider : IModelBinderProvider
    {

        private ISessionFactory sessionFactory;

        public FromNHibernateModelBinderProvider(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        #region IModelBinderProvider Members

        public IModelBinder GetBinder(Type modelType)
        {
            var persistanceInfo = sessionFactory.GetClassMetadata(modelType);
            return persistanceInfo != null
                 ? new FromNHibernateModelBinder(modelType, persistanceInfo) { SessionFactory = sessionFactory }
                 : null;
        }

        #endregion
    }

    /// <summary>
    /// This binds exisitng NHibernate Entities to the input of a controller action then update any of its properties.
    /// Should only be used for POST Methods due to lazy loading. Very useful for lookups.
    /// </summary>
    /// <remarks>
    /// If the input or a property is an Entity, the binder will look for keys of [PropertyName], [PropertyName].Id or [PropertyName]Id
    /// </remarks>
    public class FromNHibernateModelBinder : DefaultModelBinder
    {
        private Type modelType;
        private IClassMetadata persistanceInfo;

        protected IList<string> ParentPropertyNames;

        public ISessionFactory SessionFactory { get; set; }

        public FromNHibernateModelBinder(Type modelType, IClassMetadata persistanceInfo)
        {
            this.modelType = modelType;
            this.persistanceInfo = persistanceInfo;
            ParentPropertyNames = new List<string> { "Parent", modelType.Name};
        }

        #region IModelBinder Members
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var session = ((MaelstromController)controllerContext.Controller).DataSession;

            if (session == null)
                return null;
            ValueProviderResult value = null;

            if (!String.IsNullOrEmpty(bindingContext.ModelName) && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                // We couldn't find any entry that began with the prefix. If this is the top-level element, fall back
                // to the empty prefix.
                if (bindingContext.FallbackToEmptyPrefix)
                {
                    bindingContext = new ModelBindingContext()
                    {
                        ModelMetadata = bindingContext.ModelMetadata,
                        ModelState = bindingContext.ModelState,
                        PropertyFilter = bindingContext.PropertyFilter,
                        ValueProvider = bindingContext.ValueProvider
                    };
                }
                else
                {
                    return null;
                }
            }

            foreach (var key in GetKeys(bindingContext))
            {
                value = bindingContext.ValueProvider.GetValue(key);
                if (value != null)
                {
                    break;
                }
            }

            if (value == null)
                return BindDefault(controllerContext, bindingContext, modelType);

            var id = value.ConvertTo(typeof(int));

            if (id == null)
                return BindDefault(controllerContext, bindingContext, modelType);

            var entity = session.Get(bindingContext.ModelType.FullName, id);

            if (entity == null && bindingContext.ModelMetadata.ContainerType == null)
                return BindDefault(controllerContext, bindingContext, modelType);

            if (entity != null)
            {
                var newBindingContext = CreateComplexElementalModelBindingContext(controllerContext, bindingContext, entity);
                // validation
                if (OnModelUpdating(controllerContext, newBindingContext))
                {
                    foreach (var prop in GetFilteredModelProperties(controllerContext, newBindingContext))
                    {
                        BindProperty(controllerContext, newBindingContext, prop);
                    }
                    OnModelUpdated(controllerContext, newBindingContext);
                }

            }
            return entity;
        }

        private IEnumerable<string> GetKeys(ModelBindingContext bindingContext)
        {
            var keys = new List<string> { bindingContext.ModelName, bindingContext.ModelName + ".Id", bindingContext.ModelName + "Id" };
            return keys.Where(x => bindingContext.PropertyFilter(x));
        }

        private object BindDefault(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return bindingContext.ModelType.IsAbstract ? null : base.BindModel(controllerContext, bindingContext);
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);

            //binding parent property of controller is model has inverse collection association
            var entityName = (SessionFactory as ISessionFactoryImplementor).TryGetGuessEntityName(modelType);
            var collectionMetadata = SessionFactory.GetCollectionMetadata(CreateSubPropertyName(entityName, propertyDescriptor.Name)); 

            if( collectionMetadata == null)
                return;

            var childMetadata = SessionFactory.GetClassMetadata(collectionMetadata.ElementType.ReturnedClass);
            var parentPropertyCandidates = childMetadata.PropertyNames.Zip(childMetadata.PropertyTypes, (name, type) => new { name, type })
                                    .Where(x => x.type.ReturnedClass == modelType) ;

            if(!parentPropertyCandidates.Any())
                return;

            //search for only property with matching type or for a parent property
            var parentProperty = parentPropertyCandidates.Count() == 1
                ? parentPropertyCandidates.Single()
                : parentPropertyCandidates.FirstOrDefault(p =>  ParentPropertyNames.Contains(p.name));

            if (parentProperty == null)
                return;

            var accessor = childMetadata.GetMappedClass(EntityMode.Poco).GetProperty(parentProperty.name);

            foreach (var item in (IEnumerable)propertyDescriptor.GetValue(bindingContext.Model))
            {
                accessor.SetValue(item, bindingContext.Model, null);
            }
        }

        #endregion
        private ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
        {
            BindAttribute bindAttr = (BindAttribute)GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(BindAttribute)];
            Predicate<string> newPropertyFilter = (bindAttr != null)
                                                      ? propertyName => bindAttr.IsPropertyAllowed(propertyName) && bindingContext.PropertyFilter(propertyName)
                                                      : bindingContext.PropertyFilter;

            ModelBindingContext newBindingContext = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, bindingContext.ModelType),
                ModelName = bindingContext.ModelName,
                ModelState = bindingContext.ModelState,
                PropertyFilter = newPropertyFilter,
                ValueProvider = bindingContext.ValueProvider
            };

            return newBindingContext;
        }

        private static bool IsCollectionType(Type type)
        {
            return null != type.GetInterface("IEnumerable`1") && typeof(string) != type;
        }


    }
}