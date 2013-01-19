using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Linq;
using Maelstrom.CharacterTracker.Web.Models;

namespace Maelstrom.CharacterTracker.Web.Controllers
{
    [Authorize]
    public class TraitsController : MaelstromController
    {
        //
        // GET: /Traits/

        public ActionResult Index()
        {
            var model = DataSession.Query<Trait>().ToArray();
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = DataSession.Get<Trait>(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Trait model)
        {
            if (model == null)
                return HttpNotFound();

            if(!ModelState.IsValid)
            {
                return View();
            }

            DataSession.Merge(model);

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View(new Trait());
        }

        [HttpPost]
        public ActionResult Create(Trait model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            DataSession.Save(model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            //we use Load() instead of Get() because deleting is idempotent and we don't want an error if we delete an entity that does not exist.
            DataSession.Delete(DataSession.Load<Trait>(id)); 

            return RedirectToAction("Index");
        }
    }
}
