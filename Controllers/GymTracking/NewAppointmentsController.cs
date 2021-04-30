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
using Microsoft.AspNet.Identity;

namespace GymApplication.Controllers.GymTracking
{
    public class NewAppointmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NewAppointments
        public ActionResult Index()
        {
            var newAppointments = db.NewAppointments.Include(n => n.AppointmentSlot).Include(n => n.Session).Include(n => n.Trainers);
            return View(newAppointments.ToList());
        }
        public ActionResult MyAppointments()
        {
            var email = User.Identity.GetUserName();
            var trainer = db.Trainers.FirstOrDefault(a => a.Email == email);
            return View(db.NewAppointments.Where(a => a.TrainerID == trainer.TrainerID).ToList());
        }
        public ActionResult SuccessPage()
        {
            return View();
        }
        // GET: NewAppointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewAppointment newAppointment = db.NewAppointments.Find(id);
            if (newAppointment == null)
            {
                return HttpNotFound();
            }
            string Mail = "";
            Mail = newAppointment.Email;
            AppointmentConfirmEmail objMail = new AppointmentConfirmEmail();
           // objMail.SendConfirmation(Mail, newAppointment.FullName, newAppointment.Date, newAppointment.getSesssionTime(), newAppointment.getSlot(), newAppointment.getTrainers());
            return View(newAppointment);
        }
        // GET: NewAppointments/Create
        public ActionResult Create()
        {
            ViewBag.AppointmentSlotId = new SelectList(db.AppointmentSlots, "AppointmentSlotId", "Slot");
            ViewBag.Member_Id = new SelectList(db.GymMember, "Member_Id", "FirstName");
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType");
            ViewBag.TrainerID = new SelectList(db.Trainers, "TrainerID", "Name");
            return View();
        }

        // POST: NewAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewAppointmentId,Date,FullName,Email,Member_Id,SessionID,TrainerID,AppointmentSlotId")] NewAppointment newAppointment)
        {
            if (newAppointment.Date.DayOfWeek == DayOfWeek.Saturday || newAppointment.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError("", "You can not book for the weekend.");
            }
            if (ModelState.IsValid)
            {
                if (!db.NewAppointments.Any(b => b.AppointmentSlotId == newAppointment.AppointmentSlotId && b.TrainerID == newAppointment.TrainerID && b.Date == newAppointment.Date))
                {
                    db.Entry(newAppointment).State = EntityState.Modified;
                    db.NewAppointments.Add(newAppointment);
                    db.SaveChanges();
                    return RedirectToAction("SuccessPage");
                }
                else
                {
                    ModelState.AddModelError("", "Sorry this session is unavailable.");
                }

            }

            ViewBag.AppointmentSlotId = new SelectList(db.AppointmentSlots, "AppointmentSlotId", "Slot", newAppointment.AppointmentSlotId);
           // ViewBag.Member_Id = new SelectList(db.GymMember, "Member_Id", "FirstName", newAppointment.Member_Id);
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", newAppointment.SessionID);
            ViewBag.TrainerID = new SelectList(db.Trainers, "TrainerID", "Name", newAppointment.TrainerID);
            return View(newAppointment);
        }

        // GET: NewAppointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewAppointment newAppointment = db.NewAppointments.Find(id);
            if (newAppointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AppointmentSlotId = new SelectList(db.AppointmentSlots, "AppointmentSlotId", "Slot", newAppointment.AppointmentSlotId);
           // ViewBag.Member_Id = new SelectList(db.GymMember, "Member_Id", "FirstName", newAppointment.Member_Id);
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", newAppointment.SessionID);
            ViewBag.TrainerID = new SelectList(db.Trainers, "TrainerID", "Name", newAppointment.TrainerID);
            return View(newAppointment);
        }

        // POST: NewAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewAppointmentId,Date,FullName,Email,Member_Id,SessionID,TrainerID,AppointmentSlotId")] NewAppointment newAppointment)
        {
            if (ModelState.IsValid)
            {
                string Mail = "";
                Mail = newAppointment.Email;
                db.Entry(newAppointment).State = EntityState.Modified;
                db.SaveChanges();
                DeclineAppointmentEmail objMail = new DeclineAppointmentEmail();
                objMail.SendConfirmation(Mail, newAppointment.FullName, newAppointment.Date, newAppointment.getSlot(), newAppointment.getTrainers());
                return RedirectToAction("MyAppointments");
            }
            ViewBag.AppointmentSlotId = new SelectList(db.AppointmentSlots, "AppointmentSlotId", "Slot", newAppointment.AppointmentSlotId);
           // ViewBag.Member_Id = new SelectList(db.GymMember, "Member_Id", "FirstName", newAppointment.Member_Id);
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", newAppointment.SessionID);
            ViewBag.TrainerID = new SelectList(db.Trainers, "TrainerID", "Name", newAppointment.TrainerID);
            return View(newAppointment);
        }
        // GET: NewAppointments/DeclineAppointment/5
        public ActionResult DeclineAppointment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewAppointment newAppointment = db.NewAppointments.Find(id);
            if (newAppointment == null)
            {
                return HttpNotFound();
            }
            return View(newAppointment);
        }

        // POST: NewAppointments/Decline/5
        [HttpPost, ActionName("DeclineAppointment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeclineAppointment(int id)
        {

            NewAppointment newAppointment = db.NewAppointments.Find(id);
            string Mail = "";
            Mail = newAppointment.Email;
            db.NewAppointments.Remove(newAppointment);
            db.SaveChanges();
            DeclineAppointmentEmail objMail = new DeclineAppointmentEmail();
            objMail.SendConfirmation(Mail, newAppointment.FullName, newAppointment.Date, newAppointment.getSlot(), newAppointment.getTrainers());
            return RedirectToAction("MyAppointments");
        }
        // GET: NewAppointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewAppointment newAppointment = db.NewAppointments.Find(id);
            if (newAppointment == null)
            {
                return HttpNotFound();
            }
            return View(newAppointment);
        }

        // POST: NewAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewAppointment newAppointment = db.NewAppointments.Find(id);
            db.NewAppointments.Remove(newAppointment);
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
