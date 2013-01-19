using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;

namespace Maelstrom.CharacterTracker.Web.Controllers
{
    public abstract class MaelstromController : Controller
    {

        public ISession DataSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DataSession = MvcApplication.SessionFactory.OpenSession();
            DataSession.BeginTransaction();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (DataSession == null)
                return;

            if ((filterContext.Exception == null || filterContext.ExceptionHandled) && ModelState.IsValid)
                DataSession.Transaction.Commit();
            else
            {
                DataSession.Transaction.Rollback();
            }

            DataSession.Dispose();
            DataSession = null;

            base.OnActionExecuted(filterContext);
        }
    }
}
