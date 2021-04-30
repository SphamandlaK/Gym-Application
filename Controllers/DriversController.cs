using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GymApplication.Logic;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GymApplication.Controllers
{
    public class DriversController : Controller
    {


		Order_Business ob = new Order_Business();



		private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

		[Authorize]
		public ActionResult Deliveries()
		{

			//return View(ob.cust_all().Where(x => x.Customer.Email == User.Identity.Name));

			

			return View(db.Manifests.ToList());
		}



        // GET: Drivers
        public ActionResult Index()
        {
            return View(db.Drivers.ToList());
        }



		 public ActionResult Tractiking(int? id)
		{
			if (id == null)
				return RedirectToAction("Bad_Request", "Error");
			if (ob.cust_find_by_id(id) != null)
			{
				ViewBag.Order_Items = ob.cust_Order_items(id);
				ViewBag.Address = db.Order_Addresses.ToList().Find(x => x.Order_ID == id);
				

				return View(ob.cust_find_by_id(id));
			}
			else
				return RedirectToAction("Not_Found", "Error");
		}
        // GET: Drivers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // GET: Drivers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Driver_ID,Driver_Name,Driver_Image,Driver_Surname,Driver_IDNo,Driver_CellNo,Driver_Address,Driver_Email,Driver_Pass")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                driver.Driver_Pass = "Password@01";

                var user = new ApplicationUser { UserName = driver.Driver_Email, Email = driver.Driver_Email };
                await UserManager.CreateAsync(user, driver.Driver_Pass);
                driver.Driver_ID = user.Id;

                db.Drivers.Add(driver);
                db.SaveChanges();
                UserManager.AddToRole(driver.Driver_ID, "Driver");
                return RedirectToAction("Index");
            }

            return View(driver);
        }

        // GET: Drivers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Driver_ID,Driver_Name,Driver_Image,Driver_Surname,Driver_IDNo,Driver_CellNo,Driver_Address,Driver_Email,Driver_Pass")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Driver driver = db.Drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Driver driver = db.Drivers.Find(id);
            db.Drivers.Remove(driver);
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
