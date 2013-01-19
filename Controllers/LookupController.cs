using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Maelstrom.CharacterTracker.Web.Controllers
{
    public class LookupController<T> : Controller where T : class, new()
    {
        private static IList<T> _repository = new List<T>();
        private static int nextId = 0;

        //
        // GET: /Lookup/

        public ActionResult Index()
        {
            return View(_repository);
        }

        //
        // GET: /Lookup/Details/5

        public ActionResult Details(int id)
        {
            var item = FindById(id);
            return View(item);
        }

        private static T FindById(int id)
        {
            var idProp = typeof(T).GetProperty("Id");
            var item = _repository.Single(x => idProp.GetValue(x, null).Equals(id));
            return item;
        }

        //
        // GET: /Lookup/Create

        public ActionResult Create()
        {
            dynamic model = new T();
            model.Id = nextId;
            return View("Edit", new T());
        }

        //
        // POST: /Lookup/Create

        [HttpPost]
        public ActionResult Create(T model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Add(model);

 
            return RedirectToAction("Index");
        }

        //
        // GET: /Lookup/Edit/5

        public ActionResult Edit(int id)
        {
            return View(FindById(id));
        }

        //
        // POST: /Lookup/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection form)
        {
            if (TryUpdateModel(FindById(id)))
            {
                return RedirectToAction("Index");
            }

            return View();

        }

        //
        // POST: /Lookup/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _repository.Remove(FindById(id));
            return RedirectToAction("Index");
        }

        protected static void Add(params T[] entity)
        {

            foreach (var item in entity)
            {
                dynamic dynaModel = item;
                dynaModel.Id = nextId;
                _repository.Add(item);
                nextId++;
            }
        }
    }
}
