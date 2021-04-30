using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using GymApplication.Common;

namespace GymApplication.Controllers
{
    [Authorize]
    public class ServiceBookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceBookings
        [Authorize]
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            GymMember memberProfile = db.GymMember.Where(p => p.ApplicationUser.Id == id).FirstOrDefault();
            if (memberProfile == null)
            {
                string name = User.Identity.Name;
                if (name.Equals("admin@admin.com"))
                { return RedirectToAction("GymMembersList", "Admin"); }
                else { return RedirectToAction("Create", "GymMembers"); }
                //return HttpNotFound();
                
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            var serviceBookings = db.ServiceBooking.Where(m => m.ApplicationUser.Id == user.Id).ToList();
            return View(serviceBookings);
        }

        // GET: ServiceBookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceBooking serviceBooking = db.ServiceBooking.Find(id);
            if (serviceBooking == null)
            {
                return HttpNotFound();
            }
            return View(serviceBooking);
        }

        
        public ActionResult CancelBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceBooking serviceBooking = db.ServiceBooking.Find(id);
            if (serviceBooking == null)
            {
                return HttpNotFound();
            }
            serviceBooking.BookingStatus = false;
            if (ModelState.IsValid)
            {
                db.Entry(serviceBooking).State = EntityState.Modified;
                db.SaveChanges();
                String toEmail = serviceBooking.ApplicationUser.Email;
                String subject = serviceBooking.Service.SeviceName + " Booking  Cancellation Details";
                //String contents = newsletterViewModel.News_content;
                String contents1 = String.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Email_Template/Cancel_Contents.html")))
                {
                    contents1 = reader.ReadToEnd();
                }
                contents1 = contents1.Replace("CONTENT0", serviceBooking.Booking_Id.ToString());
                contents1 = contents1.Replace("CONTENT1", serviceBooking.Service.SeviceName);
                contents1 = contents1.Replace("CONTENT2", serviceBooking.BookingDate.ToShortDateString());
                contents1 = contents1.Replace("CONTENT3", serviceBooking.ServiceTimings.Timing);
                //contents = contents1 + contents ;
                EmailSender es = new EmailSender();
                es.Send(toEmail, subject, contents1);
                return RedirectToAction("Index");
            }
            return View(serviceBooking);
        }
        // GET: ServiceBookings/Create
        public ActionResult Create()
        {
            
            ViewBag.Service = new SelectList(db.Service, "Service_Id ", "SeviceName");
            ViewBag.Timing = new SelectList(db.ServiceTimings, "Timing_Id", "Timing");
            return View();
        }

        //public JsonResult GetTimings(int timing_id)
        //{

        //    List<ServiceTimings> ServiceTimings = db.ServiceTimings.Where(x => x.Service.Service_Id == timing_id).ToList();
        //    return Json(ServiceTimings, JsonRequestBehavior.AllowGet);
        //}

        // POST: ServiceBookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Booking_Id,Service_Id,Timing_Id,BookingDate,BookingStatus,Service,ServiceTimings")] ServiceBooking serviceBooking)
        {
             //int s_Id = serviceBooking.Service.Service_Id;
             //int t_Id = serviceBooking.ServiceTimings.Timing_Id;


           serviceBooking.BookingStatus = true;
           Service service  = db.Service.Where(x => x.Service_Id == serviceBooking.Service.Service_Id).SingleOrDefault();

           ServiceTimings Timings = db.ServiceTimings.Where(x => x.Timing_Id == serviceBooking.ServiceTimings.Timing_Id).SingleOrDefault();

           serviceBooking.Service = db.Service.Where(x => x.Service_Id == serviceBooking.Service.Service_Id).SingleOrDefault();
          serviceBooking.ServiceTimings = db.ServiceTimings.Where(x => x.Timing_Id == serviceBooking.ServiceTimings.Timing_Id).SingleOrDefault();

           serviceBooking.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
           serviceBooking.Service.SeviceName = service.SeviceName;
           serviceBooking.Service.ServiceDesc = service.ServiceDesc;
           serviceBooking.Service.Service_Id = service.Service_Id;
           serviceBooking.ServiceTimings.Timing_Id = Timings.Timing_Id;
           serviceBooking.ServiceTimings.Timing = Timings.Timing;
           //serviceBooking.ServiceTimings.Service = Timings.Service;

            ModelState.Clear();
            TryValidateModel(serviceBooking);




            if (ModelState.IsValid)
            {
                serviceBooking.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
                db.ServiceBooking.Add(serviceBooking);
                db.SaveChanges();
                String toEmail = serviceBooking.ApplicationUser.Email;
                String subject = serviceBooking.Service.SeviceName + " Booking Details";
                //String contents = newsletterViewModel.News_content;
                String contents1 = String.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Email_Template/Booking_Contents.html")))
                {
                    contents1 = reader.ReadToEnd();
                }
                contents1 = contents1.Replace("CONTENT0", serviceBooking.Booking_Id.ToString());
                contents1 = contents1.Replace("CONTENT1", serviceBooking.Service.SeviceName);
                contents1 = contents1.Replace("CONTENT2", serviceBooking.BookingDate.ToShortDateString());
               // contents1 = contents1.Replace("CONTENT3", serviceBooking.ServiceTimings.Timing);
                //contents = contents1 + contents ;
                EmailSender es = new EmailSender();
                es.Send(toEmail, subject, contents1);
                return RedirectToAction("Index");
            }
            ViewBag.Service = new SelectList(db.Service, "Service_Id ", "SeviceName", serviceBooking.Service_Id);
           // ViewBag.Timing = new SelectList(db.ServiceTimings, "Timing_Id", "Timing", serviceBooking.Timing_Id);
            return View(serviceBooking);
        }

        // GET: ServiceBookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceBooking serviceBooking = db.ServiceBooking.Find(id);
            if (serviceBooking == null)
            {
                return HttpNotFound();
            }
            return View(serviceBooking);
        }

        // POST: ServiceBookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Booking_Id,BookingDate,BookingStatus")] ServiceBooking serviceBooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceBooking);
        }

        // GET: ServiceBookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceBooking serviceBooking = db.ServiceBooking.Find(id);
            if (serviceBooking == null)
            {
                return HttpNotFound();
            }
            return View(serviceBooking);
        }

        // POST: ServiceBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceBooking serviceBooking = db.ServiceBooking.Find(id);
            db.ServiceBooking.Remove(serviceBooking);
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
