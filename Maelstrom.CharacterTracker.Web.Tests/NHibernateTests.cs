using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NHibernate;
using Maelstrom.CharacterTracker.Web.Controllers;
using Rhino.Mocks;
using System.Web;
using System.Web.Routing;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Maelstrom.CharacterTracker.Web.Models;

namespace Maelstrom.CharacterTracker.Web.Tests
{
    public abstract class NHibernateTests
    {
        protected static  readonly ISessionFactory sessionFactory;
        private static readonly Configuration cfg;
        protected ISession currentSession;

        static NHibernateTests()
        {
            cfg = CreateNHibernateConfiguration();
            sessionFactory = cfg.BuildSessionFactory();
        }

        public NHibernateTests()
        {
            
            currentSession = sessionFactory.OpenSession();
            new SchemaExport(cfg).Execute(false, true, false, currentSession.Connection, null);
        }

        protected void SetupData(Action<ISession> action)
        {
                action(currentSession);
                currentSession.Flush();
        }

        protected void AssertData(Action<ISession> assertions)
        {
                assertions(currentSession);
        }

        protected static Configuration CreateNHibernateConfiguration()
        {
            return Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.SQLiteConfiguration.Standard.InMemory)
                .Mappings(m => m.AutoMappings
                    .Add(AutoMap.Assembly(typeof(Entity).Assembly).Where(t => typeof(Entity).IsAssignableFrom(t) && t!=typeof(Entity)))
                    .Add(AutoMap.Assembly(typeof(TestEntityWithBiCollection).Assembly).Where(t => typeof(Entity).IsAssignableFrom(t) && t!=typeof(Entity))
                        .Override<TestEntityWithBiCollection>(map => map.HasMany(x => x.Children).Cascade.All().Inverse())))
                .BuildConfiguration();

        }

        public void Dispose()
        {
            currentSession.Dispose();
        }

      
    }
}

