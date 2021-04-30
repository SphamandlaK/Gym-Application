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
    public class PlannedRepsAndSetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PlannedRepsAndSets
        public ActionResult Index()
        {
            var plannedRepsAndSets = db.PlannedRepsAndSets.Include(p => p.Exercise).Include(p => p.ExerciseDayPrograms);
            return View(plannedRepsAndSets.ToList());
        }

        // GET: PlannedRepsAndSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlannedRepsAndSets plannedRepsAndSets = db.PlannedRepsAndSets.Find(id);
            if (plannedRepsAndSets == null)
            {
                return HttpNotFound();
            }
            return View(plannedRepsAndSets);
        }

        // GET: PlannedRepsAndSets/Create
        public ActionResult Create(int? id)
        {
            var exercisesInProgram = from ex in db.PlannedRepsAndSets
                                     select ex;

            exercisesInProgram = exercisesInProgram.Where(e => e.ExerciseDayProgramID == id);

            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", id);
            ViewBag.ExerciseID = new SelectList(db.Exercises.OrderBy(o => o.ExerciseName), "ID", "ExerciseName", 1);
            ViewBag.Exercises = exercisesInProgram;
            return View();
        }

        // POST: PlannedRepsAndSets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ExerciseID,ExerciseDayProgramID,PlannedSets,PlannedReps")] PlannedRepsAndSets plannedRepsAndSets)
        {
            if (ModelState.IsValid)
            {
                db.PlannedRepsAndSets.Add(plannedRepsAndSets);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", plannedRepsAndSets.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", plannedRepsAndSets.ExerciseDayProgramID);
            return View(plannedRepsAndSets);
        }

        // GET: PlannedRepsAndSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlannedRepsAndSets plannedRepsAndSets = db.PlannedRepsAndSets.Find(id);
            if (plannedRepsAndSets == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", plannedRepsAndSets.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", plannedRepsAndSets.ExerciseDayProgramID);
            return View(plannedRepsAndSets);
        }

        // POST: PlannedRepsAndSets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ExerciseID,ExerciseDayProgramID,PlannedSets,PlannedReps")] PlannedRepsAndSets plannedRepsAndSets)
        {
            if (ModelState.IsValid)
            {
                db.Entry(plannedRepsAndSets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExerciseID = new SelectList(db.Exercises, "ID", "ExerciseName", plannedRepsAndSets.ExerciseID);
            ViewBag.ExerciseDayProgramID = new SelectList(db.ExerciseDayPrograms, "ID", "ExerciseDayName", plannedRepsAndSets.ExerciseDayProgramID);
            return View(plannedRepsAndSets);
        }

        // GET: PlannedRepsAndSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlannedRepsAndSets plannedRepsAndSets = db.PlannedRepsAndSets.Find(id);
            if (plannedRepsAndSets == null)
            {
                return HttpNotFound();
            }
            return View(plannedRepsAndSets);
        }

        // POST: PlannedRepsAndSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlannedRepsAndSets plannedRepsAndSets = db.PlannedRepsAndSets.Find(id);
            db.PlannedRepsAndSets.Remove(plannedRepsAndSets);
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
