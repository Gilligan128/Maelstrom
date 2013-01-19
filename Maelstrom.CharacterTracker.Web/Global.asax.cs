using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Maelstrom.CharacterTracker.Web.Models;

namespace Maelstrom.CharacterTracker.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static ISessionFactory SessionFactory { get; protected set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var cfg = CreateNHibernateConfiguration();
            SessionFactory = cfg.BuildSessionFactory();

            new SchemaUpdate(cfg).Execute(false, true);

        }

        protected Configuration CreateNHibernateConfiguration()
        {
            return Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("DefaultConnection")))
                .Mappings(m => m.AutoMappings.Add(AutoMap.Assembly(typeof(Entity).Assembly).Where(t => typeof(Entity).IsAssignableFrom(t))))
                .BuildConfiguration();

        }
    }
}