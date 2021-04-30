using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using GymApplication.Models.GymTracking;

namespace GymApplication.Controllers.GymTracking
{
    public class AppointmentSlotsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AppointmentSlots
        public ActionResult Index()
        {
            var appointmentSlots = db.AppointmentSlots;
            return View(appointmentSlots.ToList());
        }

        // GET: AppointmentSlots/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentSlot appointmentSlot = db.AppointmentSlots.Find(id);
            if (appointmentSlot == null)
            {
                return HttpNotFound();
            }
            return View(appointmentSlot);
        }

        // GET: AppointmentSlots/Create
        public ActionResult Create()
        {
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "Subject");
            return View();
        }

        // POST: AppointmentSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentSlotId,Slot,Time")] AppointmentSlot appointmentSlot)
        {
            if (ModelState.IsValid)
            {
                db.AppointmentSlots.Add(appointmentSlot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

          //  ViewBag.EventID = new SelectList(db.Events, "EventID", "Subject", appointmentSlot.EventID);
            return View(appointmentSlot);
        }

        // GET: AppointmentSlots/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentSlot appointmentSlot = db.AppointmentSlots.Find(id);
            if (appointmentSlot == null)
            {
                return HttpNotFound();
            }
            //ViewBag.EventID = new SelectList(db.Events, "EventID", "Subject", appointmentSlot.EventID);
            return View(appointmentSlot);
        }

        // POST: AppointmentSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentSlotId,Slot,Time")] AppointmentSlot appointmentSlot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointmentSlot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.EventID = new SelectList(db.Events, "EventID", "Subject", appointmentSlot.EventID);
            return View(appointmentSlot);
        }

        // GET: AppointmentSlots/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentSlot appointmentSlot = db.AppointmentSlots.Find(id);
            if (appointmentSlot == null)
            {
                return HttpNotFound();
            }
            return View(appointmentSlot);
        }

        // POST: AppointmentSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppointmentSlot appointmentSlot = db.AppointmentSlots.Find(id);
            db.AppointmentSlots.Remove(appointmentSlot);
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
