using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maelstrom.CharacterTracker.Web.Controllers;
using Rhino.Mocks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Maelstrom.CharacterTracker.Web.Tests
{
    public abstract class ControllerTests : NHibernateTests
    {
        protected ControllerContext ControllerContext { get; set; }
        protected object Model { get; set; }


        protected void ExecuteAction<TController>(Func<TController, ActionResult> action)
  where TController : MaelstromController, new()
        {


            var controller = new TController { DataSession = currentSession };

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Response).Return(MockRepository.GenerateStub<HttpResponseBase>());
            ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);
            controller.ControllerContext = ControllerContext;

            action(controller);

            Model = controller.ViewData.Model;

            controller.DataSession.Flush();
        }

    }
}
