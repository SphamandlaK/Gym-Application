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
using PagedList;
using Rotativa;
using GymApplication.ViewModels;

namespace GymApplication.Controllers
{
    public class TrainersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Trainers
        public ActionResult Index(string searching, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 3;
            var trainerList = db.Trainers.OrderByDescending(x => x.TrainerID).Where(m => m.Name.Contains(searching) || m.LastName.Contains(searching) || searching == null).Include(m => m.Session).ToPagedList(pageNumber, pageSize);
            return View(trainerList);
        }
        //public ActionResult Rating()
        //{
        //    RatingTrainerViewModel LBDV = new RatingTrainerViewModel
        //    {
        //        Trainers = db.Trainers.OrderByDescending(c => c.TrainerID).ToList()
        //    };
        //    return View(LBDV);
        //}
        public ActionResult PrintAll()
        {
            var q = new ActionAsPdf("Index") { FileName = "trainer.pdf" };
            return q;
        }


        //public ActionResult RatingIndex(int? filter)
        //{
        //    if (filter > 0)
        //    {
        //        return View(db.RatingClass.Where(m => m.Rate == filter).ToList());
        //    }
        //    return View(db.RatingClass.ToList());
        //}

        public ActionResult Success() => View();
        public ActionResult Rating() => View();

        //[HttpPost]
        //public ActionResult Rating(int rate, string Comment)
        //{
        //    var ratingClass = new Rating();
        //    ratingClass.Id = Guid.NewGuid();
        //    ratingClass.Comment = Comment;
        //    ratingClass.Rate = rate;
        //    ratingClass.Date = DateTime.Today;
        //    ratingClass.BookingRef = "";
        //    db.RatingClass.Add(ratingClass);
        //    db.SaveChanges();
        //    return View("Success");
        //}
        //public PartialViewResult TrainerPartial()
        //{
        //    var trainerList = db.Trainers.OrderByDescending(x => x.TrainerID).Take(2);
        //    return PartialView(trainerList);
        //}
        //// GET: Trainers/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Trainer trainer = db.Trainers.Find(id);
        //    if (trainer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.ArticleId = id.Value;
        //    var comments = db.CommentsRatings.Where(d => d.ArticleId.Equals(id.Value)).ToList();
        //    ViewBag.Comments = comments;

        //    var ratings = db.CommentsRatings.Where(d => d.ArticleId.Equals(id.Value)).ToList();
        //    if (ratings.Count() > 0)
        //    {
        //        var ratingSum = ratings.Sum(d => d.Rating.Value);
        //        ViewBag.RatingSum = ratingSum;
        //        var ratingCount = ratings.Count();
        //        ViewBag.RatingCount = ratingCount;
        //    }
        //    else
        //    {
        //        ViewBag.RatingSum = 0;
        //        ViewBag.RatingCount = 0;
        //    }

        //    return View(trainer);
        //}

        // GET: Trainers/Create
        public ActionResult Create()
        {
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType");
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TrainerID,Name,LastName,Contact_No,Experience,Description,Picture,ID_No,DOB,age,gender,century,SessionID")] Trainer trainer, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid)
            {
                if (image1 != null)
                {
                    trainer.Picture = new byte[image1.ContentLength];
                    image1.InputStream.Read(trainer.Picture, 0, image1.ContentLength);
                }
                trainer.DOB = trainer.calcyear();
                trainer.age = trainer.calcage();
                trainer.gender = trainer.getgender();
                trainer.century = trainer.century;
                db.Trainers.Add(trainer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", trainer.SessionID);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainer trainer = db.Trainers.Find(id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", trainer.SessionID);
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TrainerID,Name,LastName,Contact_No,Experience,Description,Picture,ID_No,DOB,age,gender,century,SessionID")] Trainer trainer, HttpPostedFileBase image1)
        {
            if (ModelState.IsValid)
            {
                if (trainer.TrainerID != 0)
                {
                    Trainer sesInDB = db.Trainers.Single(c => c.TrainerID == trainer.TrainerID);
                    if (image1 != null)
                    {
                        trainer.Picture = new byte[image1.ContentLength];
                        image1.InputStream.Read(trainer.Picture, 0, image1.ContentLength);
                        sesInDB.Picture = trainer.Picture;
                    }

                    sesInDB.Name = trainer.Name;
                    sesInDB.LastName = trainer.LastName;
                    sesInDB.Contact_No = trainer.Contact_No;
                    sesInDB.Experience = trainer.Experience;
                    sesInDB.Description = trainer.Description;
                    sesInDB.SessionID = trainer.SessionID;
                    trainer.DOB = trainer.calcyear();
                    trainer.age = trainer.calcage();
                    trainer.gender = trainer.getgender();
                    trainer.century = trainer.century;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = trainer.TrainerID });
                }
                ViewBag.SessionID = new SelectList(db.Sessions, "SessionID", "SessionType", trainer.SessionID);
                return HttpNotFound();
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Trainers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainer trainer = db.Trainers.Find(id);
            if (trainer == null)
            {
                return HttpNotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trainer trainer = db.Trainers.Find(id);
            db.Trainers.Remove(trainer);
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
