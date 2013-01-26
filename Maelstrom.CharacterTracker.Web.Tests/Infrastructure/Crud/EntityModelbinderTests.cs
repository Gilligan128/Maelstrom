using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Globalization;
using Maelstrom.CharacterTracker.Web.Tests;
using Xunit;
using Maelstrom.CharacterTracker.Web.Infrastructure;
using Maelstrom.CharacterTracker.Web.Models;
using Rhino.Mocks;
using Maelstrom.CharacterTracker.Web.Controllers;
using Maelstrom.CharacterTracker.Web.Infrastructure.Crud;

namespace Maelstrom.CharacterTracker.Tests.Infrastructure
{

    public class EntityModelbinderTests : NHibernateTests
    {
        protected IValueProvider ValueProvider { get; set; }
        protected NameValueCollection Values { get; set; }
        protected ControllerContext ControllerContext { get; set; }


        public EntityModelbinderTests()
        {
            Values = new NameValueCollection();
            ModelBinderProviders.BinderProviders.Add(new FromNHibernateModelBinderProvider(sessionFactory));
            ControllerContext = new ControllerContext();
            var controller =  new TraitsController();
            ControllerContext.Controller = controller;
            controller.DataSession = currentSession;

        }


        [Fact]
        public void DatabindsChildCollectionWithExistingItems()
        {
            var model = new TestEntityWithBiCollection { Name = "Parent" };
            model.Children.Add(new TestChildEntity { Name = "Child 1" });
            model.Children.Add(new TestChildEntity { Name = "Child 2" });
            SetupData(session =>
            {
                session.Save(model);
            });
            Values.Add("Id", model.Id.ToString());
            Values.Add("Children[0].Id", "1");
            Values.Add("Children[0].Name", "new name");

            var newModel = BindModel<TestEntityWithBiCollection>();

            Assert.Equal(newModel.Children[0], model.Children[0]);
            Assert.Equal("new name", newModel.Children[0].Name);
        }


        [Fact]
        public void ModelbindsExistingParentWithNewChild()
        {
            var model = new TestEntityWithBiCollection { Name = "Parent" };
            SetupData(s => s.Save(model));
            Values.Add("Id", model.Id.ToString());
            Values.Add("Children[0].Name", "new name");

            var newModel = BindModel<TestEntityWithBiCollection>();

            Assert.Equal(model, newModel);
            Assert.Equal(1, model.Children.Count);
            Assert.Equal(model, model.Children[0].Parent);
        }

        [Fact(Skip="Not sure if this should be a valid use case")]
        public void DeletesMissingChildren()
        {
            var model = new TestEntityWithBiCollection { Name = "Parent" };
            model.Children.Add(new TestChildEntity());
            SetupData(s => s.Save(model));
            Values.Add("Id", model.Id.ToString());

            var newModel = BindModel<TestEntityWithBiCollection>();

            Assert.Empty(model.Children);
        }

        [Fact]
        public void NHibernateStuff()
        {
            var parentMetadata = sessionFactory.GetClassMetadata(typeof(TestEntityWithBiCollection));
            var collectionMetadata = sessionFactory.GetCollectionMetadata(typeof(TestEntityWithBiCollection).FullName + "." + "Children");
            var childMetadata = sessionFactory.GetClassMetadata(typeof(TestChildEntity));

           // Assert.Equal(
        }

        protected T BindModel<T>()
        {
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(T)),
                FallbackToEmptyPrefix = true,
                ModelState = new ModelStateDictionary(),
                ValueProvider = new NameValueCollectionValueProvider(Values, CultureInfo.InvariantCulture)
            };

            var binder = ModelBinders.Binders.GetBinder(typeof(T));

            return (T)binder.BindModel(ControllerContext, bindingContext);
        }
    }

}
