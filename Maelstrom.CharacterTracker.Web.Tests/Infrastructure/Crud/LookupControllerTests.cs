using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maelstrom.CharacterTracker.Web.Models;
using Xunit;
using System.Web.Mvc;
using Maelstrom.CharacterTracker.Web.Controllers;
using Rhino.Mocks;
using System.Web;
using System.Web.Routing;

namespace Maelstrom.CharacterTracker.Web.Tests.Infrastructure.DynamicData
{
    public class LookupControllerTests : NHibernateTests
    {

        protected ControllerContext ControllerContext { get; set; }

        [Fact]
        public void UpdatesTheModel()
        {
            var model = new TestEntityWithBiCollection { Children = new List<TestChildEntity> { new TestChildEntity { Name = "Child" } }, Name = "Parent" };
            SetupData(session =>
                {
                    session.Save(model);

                });
        }

        protected void ExecuteAction<TController>(Action<TController> action)
          where TController : MaelstromController, new()
        {
            

            var controller = new TController { DataSession = currentSession };

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Response).Return(MockRepository.GenerateStub<HttpResponseBase>());
            ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
            controller.ControllerContext = ControllerContext;

            action(controller);

            controller.DataSession.Flush();
        }
    }
}
