using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;

namespace GymApplication.Controllers
{
    public class WishlistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Wishlists
        public ActionResult Index()
        {
            Customer u = new Customer();
            var user = db.Customers.ToList().Find(x => x.Email == User.Identity.Name);
            var wishlists = db.Wishlists.Where(x => x.Email == user.Email).Include(w => w.CUstomer).Include(w => w.Item);
            return View(wishlists.ToList());
            //return View();
        }

        // GET: Wishlists/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            return View(wishlist);
        }

        // GET: Wishlists/Create
        public ActionResult Create()
        {
            ViewBag.CustId = new SelectList(db.Customers, "CustId", "CustName");
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Description");
            return View();
        }

        // POST: Wishlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WishlistID,CustId,ItemCode,DateAdded")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    wishlist.ItemCode = Convert.ToInt16(Request["ItemCode"]); ;
                    wishlist.WishlistID = Guid.NewGuid().ToString();
                    wishlist.DateAdded = DateTime.Now;
                    var customer = db.Customers.ToList().Find(x => x.Email == User.Identity.Name);
                    wishlist.Email = customer.Email;
                    db.Wishlists.Add(wishlist);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Item added to wishlist";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    TempData["AlertMessage"] = e.Message;

                }

            }

            ViewBag.CustId = new SelectList(db.Customers, "CustId", "CustName", wishlist.Email);
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Description", wishlist.ItemCode);
            return View(wishlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNew([Bind(Include = "WishlistID,CustId,ItemCode,DateAdded")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    wishlist.ItemCode = Convert.ToInt16(Request["ItemCode"]);
                    wishlist.WishlistID = Guid.NewGuid().ToString();
                    wishlist.DateAdded = DateTime.Now;
                    var customer = db.Customers.ToList().Find(x => x.Email == User.Identity.Name);
                    wishlist.Email = customer.Email;
                    db.Wishlists.Add(wishlist);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Item added to wishlist";
                    return RedirectToAction("Index", "Shopping");
                }
                catch (Exception e)
                {
                    TempData["AlertMessage"] = e.Message;

                }

            }

            ViewBag.CustId = new SelectList(db.Customers, "CustId", "CustName", wishlist.Email);
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Description", wishlist.ItemCode);
            return View(wishlist);
        }

        // GET: Wishlists/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustId = new SelectList(db.Customers, "CustId", "CustName", wishlist.Email);
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Description", wishlist.ItemCode);
            return View(wishlist);
        }

        // POST: Wishlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WishlistID,CustId,ItemCode,DateAdded")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wishlist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustId = new SelectList(db.Customers, "CustId", "CustName", wishlist.Email);
            ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Description", wishlist.ItemCode);
            return View(wishlist);
        }

        // GET: Wishlists/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            return View(wishlist);
        }

        // POST: Wishlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            db.Wishlists.Remove(wishlist);
            db.SaveChanges();
            TempData["AlertMessage"] = "Item Removed from wishlist";
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public ActionResult Remove(int? ItemCode)
        {
            Wishlist wishlist = db.Wishlists.ToList().Find(x => x.ItemCode == ItemCode);
            db.Wishlists.Remove(wishlist);
            db.SaveChanges();
            TempData["AlertMessage"] = "Item Removed from wishlist";
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