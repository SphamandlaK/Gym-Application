using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;

namespace GymApplication.Controllers
{
    public class ServiceTimingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceTimings
        public ActionResult Index()
        {
            return View(db.ServiceTimings.ToList());
        }

        // GET: ServiceTimings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceTimings serviceTimings = db.ServiceTimings.Find(id);
            if (serviceTimings == null)
            {
                return HttpNotFound();
            }
            return View(serviceTimings);
        }

        // GET: ServiceTimings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServiceTimings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Timing_Id,Timing")] ServiceTimings serviceTimings)
        {
            if (ModelState.IsValid)
            {
                db.ServiceTimings.Add(serviceTimings);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviceTimings);
        }

        // GET: ServiceTimings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceTimings serviceTimings = db.ServiceTimings.Find(id);
            if (serviceTimings == null)
            {
                return HttpNotFound();
            }
            return View(serviceTimings);
        }

        // POST: ServiceTimings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Timing_Id,Timing")] ServiceTimings serviceTimings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceTimings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceTimings);
        }

        // GET: ServiceTimings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceTimings serviceTimings = db.ServiceTimings.Find(id);
            if (serviceTimings == null)
            {
                return HttpNotFound();
            }
            return View(serviceTimings);
        }

        // POST: ServiceTimings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceTimings serviceTimings = db.ServiceTimings.Find(id);
            db.ServiceTimings.Remove(serviceTimings);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
