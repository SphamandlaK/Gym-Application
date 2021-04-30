using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;

namespace GymApplication.Controllers
{
    public class RatingsController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();
		public ActionResult RatingIndex(int? filter)
		{
			if (filter > 0)
			{
				return View(db.ratingClasses.Where(m => m.Rating == filter).ToList());
			}
			return View(db.ratingClasses.ToList());
		}

		public ActionResult Success() => View();
		public ActionResult Rating() => View();

		[HttpPost]
		public ActionResult Rating(int rating, string Comment)
		{
			var RatingClasses = new RatingClass();
			RatingClasses.Id = Guid.NewGuid();
			RatingClasses.Comment = Comment;
			RatingClasses.Rating = rating;
			RatingClasses.Date = DateTime.Today;
			db.ratingClasses.Add(RatingClasses);
			db.SaveChanges();
			return View("Success");
		}
	}
}