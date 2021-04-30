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
using GymApplication.ViewModels;

namespace GymApplication.Controllers.GymTracking
{
    public class ExerciseDayProgramsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ExerciseDayPrograms
        public ActionResult Index(int? id)
        {
            var viewModel = new ExerciseDayProgramIndexData();
            viewModel.ExerciseDayPrograms = db.ExerciseDayPrograms.Include(p => p.PlannedExercises.Select(e => e.Exercise))
                .OrderBy(p => p.ExerciseDayName);


            if (id != null)
            {
                ViewBag.ExerciseDayProgramID = id.Value;
                viewModel.PlannedRepsAndSets = viewModel.ExerciseDayPrograms.Where(
                    i => i.ID == id.Value).Single().PlannedExercises;
            }

            return View(viewModel);
        }
        // GET: ExerciseDayPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExerciseDayProgram exerciseDayProgram = db.ExerciseDayPrograms.Find(id);
            if (exerciseDayProgram == null)
            {
                return HttpNotFound();
            }
            return View(exerciseDayProgram);
        }

        // GET: ExerciseDayPrograms/Create
        public ActionResult Create()
        {
            var newExerciseProgram = new ExerciseDayProgram();
            newExerciseProgram.PlannedExercises = new List<PlannedRepsAndSets>();
            /*PopulateExercises(newExerciseProgram);   */
            return View();
        }

        // POST: ExerciseDayPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ExerciseDayName,Description")] ExerciseDayProgram exerciseDayProgram, string input)
        {
            if (ModelState.IsValid)
            {
                db.ExerciseDayPrograms.Add(exerciseDayProgram);
                db.SaveChanges();

                if (input == "Create and add exercises")
                {
                    return RedirectToAction("Create/" + exerciseDayProgram.ID, "PlannedRepsAndSets");
                }
                else
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Create/" + exerciseDayProgram.ID, "PlannedRepsAndSets");
            }

            return View(exerciseDayProgram);
        }

        // GET: ExerciseDayPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExerciseDayProgram exerciseDayProgram = db.ExerciseDayPrograms.Find(id);
            if (exerciseDayProgram == null)
            {
                return HttpNotFound();
            }
            return View(exerciseDayProgram);
        }

        // POST: ExerciseDayPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ExerciseDayName,Description")] ExerciseDayProgram exerciseDayProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exerciseDayProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exerciseDayProgram);
        }

        // GET: ExerciseDayPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExerciseDayProgram exerciseDayProgram = db.ExerciseDayPrograms.Find(id);
            if (exerciseDayProgram == null)
            {
                return HttpNotFound();
            }
            return View(exerciseDayProgram);
        }

        // POST: ExerciseDayPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExerciseDayProgram exerciseDayProgram = db.ExerciseDayPrograms.Find(id);
            db.ExerciseDayPrograms.Remove(exerciseDayProgram);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private void PopulateExercises(ExerciseDayProgram exerciseProgram)
        {
            var allExercises = db.Exercises;
            var selectedExercises = new HashSet<int>(exerciseProgram.PlannedExercises.Select(e => e.ExerciseID));
            var viewModel = new List<AssignedExercises>();

            foreach (var exercise in allExercises)
            {
                viewModel.Add(new AssignedExercises
                {
                    ExerciseID = exercise.ID,
                    ExerciseName = exercise.ExerciseName,
                    Assigned = selectedExercises.Contains(exercise.ID)
                });
            }

            ViewBag.Exercises = viewModel;
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
