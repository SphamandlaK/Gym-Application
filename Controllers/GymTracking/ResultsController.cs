using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GymApplication.Models;
using GymApplication.Models.GymTracking;

namespace GymApplication.Controllers.GymTracking
{
    public class ResultsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Results
        public ActionResult Index(string exerciseDaySearchName, string exerciseName)
        {

            // Create a selectList for the view with the different workout program names
            var exerciseDays = new List<string>();

            var edQuery = from ed in db.ExerciseDayPrograms
                          select ed.ExerciseDayName;

            exerciseDays.AddRange(edQuery.Distinct());

            ViewBag.exerciseDaySearchName = new SelectList(exerciseDays);


            // Create a selectList for the various exercises
            var activities = new List<string>();

            var exerciseQuery = from eqResults in db.Results
                                join ex in db.Exercises on eqResults.ExerciseID equals ex.ID
                                orderby ex.ExerciseName
                                select ex.ExerciseName;

            activities.AddRange(exerciseQuery.Distinct());

            ViewBag.exerciseName = new SelectList(activities);


            // Generate results 
            var results = from e in db.Results
                          select e;

            var GuID = "*";

            if (User.Identity.IsAuthenticated)
            {
                GuID = User.Identity.GetUserId();
            }


            results = db.Results.Include(r => r.Exercise).Include(r => r.ExerciseDayProgram).Where(r => r.GuID == GuID);


            // If there is a filter for the exerciseName filter the results
            if (!String.IsNullOrEmpty(exerciseName))
            {
                int exID = (from exName in db.Exercises
                            where exName.ExerciseName == exerciseName
                            select exName.ID).First();

                results = results.Where(r => r.ExerciseID == exID);
            }


            //If there is a filter for the exercise day program, filter the results
            if (!String.IsNullOrEmpty(exerciseDaySearchName))
            {
                int exD = (from exDay in db.ExerciseDayPrograms
                           where exDay.ExerciseDayName == exerciseDaySearchName
                           select exDay.ID).First();

                results = results.Where(r => r.ExerciseDayProgramID == exD);
            }

            results = results.OrderByDescending(r => r.ExerciseDate);

            return View(results.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return View(results);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName");
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName");

            var sets = new List<SelectListItem>();
            for (var i = 1; i < 16; i++)
                sets.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            ViewBag.Sets = sets;

            return View();


        }

        // POST: Results/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ExerciseID,ExerciseDayProgramID,ExerciseDate,SetNumber,Weight,Reps,GuID")] Results results)
        {
            if (ModelState.IsValid)
            {
                var GuID = User.Identity.GetUserId();
                results.GuID = GuID;
                db.Results.Add(results);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", results.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", results.ExerciseDayProgramID);
            return View(results);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", results.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", results.ExerciseDayProgramID);
            return View(results);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ExerciseID,ExerciseDayProgramID,ExerciseDate,SetNumber,Weight,Reps,GuID")] Results results)
        {
            if (ModelState.IsValid)
            {
                var GuID = User.Identity.GetUserId();
                results.GuID = GuID;
                db.Entry(results).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", results.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", results.ExerciseDayProgramID);
            return View(results);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Results results = db.Results.Find(id);
            if (results == null)
            {
                return HttpNotFound();
            }
            return View(results);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Results results = db.Results.Find(id);
            db.Results.Remove(results);
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
