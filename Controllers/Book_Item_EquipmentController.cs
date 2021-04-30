using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using GymApplication.Models.EquipmentBooking;
using Microsoft.AspNet.Identity;

namespace GymApplication.Controllers
{
	public class Book_Item_EquipmentController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Book_Item_Equipment
		public ActionResult Index()
		{
			var book_Item_Equipment = db.Book_Item_Equipment.Include(b => b.Category).Include(b => b.Item);
			return View(book_Item_Equipment.ToList());
		}
		public ActionResult BookedEquipment()
		{
			var userName = User.Identity.GetUserName();
			var book_equipment = db.Book_Item_Equipment.Include(b => b.Category).Include(b => b.Item);
			return View(book_equipment.ToList().Where(x => x.userId == userName));
		}
		public ActionResult ConfirmBooking(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			return View(book_Item_Equipment);
		}
		public ActionResult ConfirmPickUp(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			return View(book_Item_Equipment);
		}
		public ActionResult ConfirmCollection(int? id)
		{
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			book_Item_Equipment.status = "Picked Up";
			db.Entry(book_Item_Equipment).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("Index");

		}
		public ActionResult ReturnEquipment(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			ViewBag.PickUpDate = book_Item_Equipment.Date_From.Date;
			ViewBag.ReturnDate = book_Item_Equipment.Date_To.Date;
			ViewBag.userId = book_Item_Equipment.userId;
			ViewBag.status = book_Item_Equipment.status;
			ViewBag.item_name = book_Item_Equipment.ItemName;
			ViewBag.desc = book_Item_Equipment.Description;
			ViewBag.depos = book_Item_Equipment.Deposit;
			ViewBag.item_Code = book_Item_Equipment.ItemCode;
			ViewBag.Cat_Id = book_Item_Equipment.Category_ID;
			ViewBag.Cus_FName = book_Item_Equipment.CustomerName;
			ViewBag.Cus_LName = book_Item_Equipment.CustomerSurname;
			ViewBag.price = book_Item_Equipment.Price;
			ViewBag.pic = book_Item_Equipment.Picture;
			ViewBag.numofdays = book_Item_Equipment.numOfDays;
			ViewBag.bookicost = book_Item_Equipment.Booking_Cost;
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", book_Item_Equipment.Category_ID);
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", book_Item_Equipment.ItemCode);
			return View(book_Item_Equipment);
		}
		public ActionResult AvailableEquipment(int? id)
		{
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			book_Item_Equipment.status = "Completed";
			//book_Item_Equipment.Booking_Cost =
			book_Item_Equipment.numOfDays = Convert.ToInt32(book_Item_Equipment.CalcNum_of_Days(book_Item_Equipment));
			Item item = db.Items.ToList().Find(x => x.ItemCode == book_Item_Equipment.ItemCode);
			item.Status = "Available";
			db.Entry(book_Item_Equipment).State = EntityState.Modified;
			db.Entry(item).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("Index");
		}
		// GET: Book_Item_Equipment/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			return View(book_Item_Equipment);
		}

		// GET: Book_Item_Equipment/Create
		public ActionResult Create(int? id)
		{
			ViewBag.Id = id;
			ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name");
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name");
			return View();
		}

		// POST: Book_Item_Equipment/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ItemId,userId,CustomerName,CustomerSurname,ItemName,Description,ItemCode,Category_ID,Date_From,Date_To,Price,Picture,Booking_Cost,Deposit,numOfDays,status")] Book_Item_Equipment book_Item_Equipment)
		{
			var userName = User.Identity.GetUserName();
			book_Item_Equipment.userId = userName;
			var customers = db.Customers.Where(x => x.Email == userName).FirstOrDefault();

			if (ModelState.IsValid)
			{
				book_Item_Equipment.CustomerName = customers.FirstName;
				book_Item_Equipment.CustomerSurname = customers.LastName;
				book_Item_Equipment.numOfDays = Convert.ToInt32(book_Item_Equipment.CalcNum_of_Days(book_Item_Equipment));
				db.Book_Item_Equipment.Add(book_Item_Equipment);
				db.SaveChanges();
				return RedirectToAction("ConfirmBooking", "Book_Item_Equipment", new { id = book_Item_Equipment.ItemId });
			}

			ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", book_Item_Equipment.Category_ID);
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", book_Item_Equipment.ItemCode);
			return View(book_Item_Equipment);
		}

		// GET: Book_Item_Equipment/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", book_Item_Equipment.Category_ID);
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", book_Item_Equipment.ItemCode);
			return View(book_Item_Equipment);
		}

		// POST: Book_Item_Equipment/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ItemId,userId,CustomerName,CustomerSurname,ItemName,Description,ItemCode,Category_ID,Date_From,Date_To,Price,Picture,Booking_Cost,Deposit,numOfDays,status")] Book_Item_Equipment book_Item_Equipment)
		{
			if (ModelState.IsValid)
			{
				db.Entry(book_Item_Equipment).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.Category_ID = new SelectList(db.Categories, "Category_ID", "Category_Name", book_Item_Equipment.Category_ID);
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", book_Item_Equipment.ItemCode);
			return View(book_Item_Equipment);
		}

		// GET: Book_Item_Equipment/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			if (book_Item_Equipment == null)
			{
				return HttpNotFound();
			}
			return View(book_Item_Equipment);
		}

		// POST: Book_Item_Equipment/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Book_Item_Equipment book_Item_Equipment = db.Book_Item_Equipment.Find(id);
			db.Book_Item_Equipment.Remove(book_Item_Equipment);
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
