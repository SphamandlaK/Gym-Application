using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Logic;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using ZXing;

namespace GymApplication.Controllers
{
    public class ManifestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
		Order_Business ob = new Order_Business();
		//Order_Business ob = new Order_Business();

		// GET: Manifests
		public ActionResult Index()
        {
            var manifests = db.Manifests.Include(m => m.Driver).Include(m => m.Employee).Include(m => m.Order);
            return View(manifests.ToList());

            //GymApplication 
            //List<Report> ListReport = new ListReport

            //var man = (from Order_Item in db.Orders
            //           join DriverName in db.Drivers
            //           on Order_Item.Order_ID equals DriverName.Driver_ID
            //           select new
            //           {
            //               Order_Item.Order_ID,
            //               DriverName.Driver_ID,
            //           }
            //           ).ToList();
            
            //foreach(var item in man)
            //{
            //    ListReport mn = new ListReport();
            //    mn.Order_ID = item.Order_ID,
            //    mn.Driver_ID = item.Driver_ID;
            //}
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

		//[Authorize(Roles = "Driver")]
		public ActionResult DriverList()
        {
            var uid = User.Identity.GetUserId();
            var manifests = db.Manifests.Include(m => m.Driver).Include(m => m.Employee).Include(m => m.Order).Where(m=>m.Driver_ID == uid);
            return View(manifests.ToList());
        }
        //[Authorize]
        public ActionResult EmployeeList()
        {
            var uid = User.Identity.GetUserId();
            var manifests = db.Manifests.Include(m => m.Driver).Include(m => m.Employee).Include(m => m.Order).Where(m => m.Employee_ID == uid);
            return View(manifests.ToList());
        }
        // GET: Manifests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manifest manifest = db.Manifests.Find(id);
            ViewBag.Orders = db.Orders.ToList();
            if (manifest == null)
            {
                return HttpNotFound();
            }
            return View(manifest);
        }
        public ActionResult UpdateProcess(int id)
        {
            var order = db.Orders.Find(id);

                //order tracking
                db.Order_Trackings.Add(new Order_Tracking()
                {
                    order_ID = order.Order_ID,
                    date = DateTime.Now,
                    status = "At warehouse",

                    Recipient = ""
                });
            return RedirectToAction("Create",new {id = id});
        }
        // GET: Manifests/Create
        public ActionResult ViewPDF(int id)
        {
            var report = new Rotativa.ActionAsPdf("Manifest", new { id = id }) { FileName = "Invoice.pdf" };
            return report;

            //return new ViewAsPdf("Invoice", new { id = id });
        }
        public ActionResult Manifest(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manifest orderList = db.Manifests.Find(id);
            if (orderList == null)
            {
                return HttpNotFound();
            }
            //Order order = (from v in db.Orders
            //               where v.Order_ID == orderList.Order_ID
            //               select v).FirstOrDefault();

            //order.statues = "Is Packed";
            return View(orderList);
        }

        
        public ActionResult Create(int id)
        {
            var mOrder = new Manifest
            {
                Order_ID = id
            };
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name");
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Employee_Name");
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Email");
            return View(mOrder);
        }

        // POST: Manifests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Manifest_ID,Order_ID,Employee_ID,Driver_ID")] Manifest manifest)
        {
            if (ModelState.IsValid)
            {
                
                db.Manifests.Add(manifest);
                db.SaveChanges();
                Assign(manifest.Order_ID);
                return RedirectToAction("Order_List", "Orders");
            }

            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", manifest.Driver_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Employee_Name", manifest.Employee_ID);
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Email", manifest.Order_ID);
            return View(manifest);
        }
        public void Assign(int id)
        {
            Order order = (from d in db.Orders
                           where d.Order_ID ==id
                           select d).Single();
            order.isAssigned = true;
            db.SaveChanges();
        }

        // GET: Manifests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manifest manifest = db.Manifests.Find(id);
            if (manifest == null)
            {
                return HttpNotFound();
            }
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", manifest.Driver_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Employee_Name", manifest.Employee_ID);
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Email", manifest.Order_ID);
            return View(manifest);
        }

        // POST: Manifests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Manifest_ID,Order_ID,Employee_ID,Driver_ID")] Manifest manifest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manifest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Driver_ID = new SelectList(db.Drivers, "Driver_ID", "Driver_Name", manifest.Driver_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "Employee_ID", "Employee_Name", manifest.Employee_ID);
            ViewBag.Order_ID = new SelectList(db.Orders, "Order_ID", "Email", manifest.Order_ID);
            return View(manifest);
        }

        // GET: Manifests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manifest manifest = db.Manifests.Find(id);
            if (manifest == null)
            {
                return HttpNotFound();
            }
            return View(manifest);
        }

        // POST: Manifests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Manifest manifest = db.Manifests.Find(id);
            db.Manifests.Remove(manifest);
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
		public ActionResult ManifestDocument([Bind(Include = "Manifest_ID,Order_ID,Employee_ID,Driver_ID")] Manifest manifest)
		{

			return View();
		}

		public double get_order_total(int id)
		{
			double amount = 0;
			foreach (var item in db.Order_Items.ToList().FindAll(match: x => x.Order_id == id))
			{
				amount += (item.price * item.quantity);
			}
			return amount;
		}
		public ActionResult Order_Details(int? id)
		{
			if (id == null)
				return RedirectToAction("Bad_Request", "Error");
			if (ob.cust_find_by_id(id) != null)
			{
				ViewBag.Order_Items = ob.cust_Order_items(id);
				ViewBag.Address = db.Order_Addresses.ToList().Find(x => x.Order_ID == id);
				ViewBag.Total = get_order_total((int)id);

				return View(ob.cust_find_by_id(id));
			}
			else
				return RedirectToAction("Not_Found", "Error");
		}


		public ActionResult Indexxxx(int? id)
		{
			Session["CapturedImage"] = "";
			string id1 = id.ToString();
			return View();
		}
		[HttpPost]
		public ActionResult Capture()
		{
			if (Request.InputStream.Length > 0)
			{
				using (StreamReader reader = new StreamReader(Request.InputStream))
				{
					//var qrs = db.Scans.OrderByDescending(p => p.id).Take(1).FirstOrDefault();

					string hexString = Server.UrlEncode(reader.ReadToEnd());
					string imageName = "scan" ;
					string imagePath = string.Format("~/Captures/{0}.png", imageName);
					System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
					Session["CapturedImage"] = VirtualPathUtility.ToAbsolute(imagePath);

					QcodeDTO rt = new QcodeDTO();
					{
						byte[] imaa = ConvertHexToBytes(hexString);
						rt.DriverName = "tthhhhhhhhhhhhh";
						rt.QrName = "ttttttt";
						rt.image = imaa;
						db.Qcodes.Add(rt);
						db.SaveChanges();
					}
				}
			}

			return RedirectToAction("Capturer");
		}

		[HttpPost]
		public ContentResult GetCapture()
		{
			string url = Session["CapturedImage"].ToString();
			Session["CapturedImage"] = null;
			return Content(url);
		}

		private static byte[] ConvertHexToBytes(string hex)
		{
			byte[] bytes = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return bytes;
		}


		public ActionResult OrderErrr()
		{
			return View();
		}



		public ActionResult Capturer()
		{

			try
			{




				//var qrs = db.Scans.OrderByDescending(p => p.id).Take(1).FirstOrDefault();

				var scann = db.Qcodes.OrderByDescending(x => x.Id).FirstOrDefault();






				var barcodeReader = new BarcodeReader();

				byte[] imaa = scann.image;


				Stream stream = new MemoryStream(imaa);

				// create an in memory bitmap
				var barcodeBitmap = (Bitmap)Bitmap.FromStream(stream);

				// decode the barcode from the in memory bitmap
				var barcodeResult = barcodeReader.Decode(barcodeBitmap);

				string link = barcodeResult.ToString();
				// output results to console

				string ordercode = link;
				//var scans = db.Scans.OrderByDescending(x => x.id).FirstOrDefault();



				int userBalance = 0;
				var q = db.Orders.FirstOrDefault(x => x.OrderCode == ordercode && x.status == "PAID      " && (x.status == "SHIPPED" || x.status == "PROCESSING"));
				userBalance = q.Order_ID;

				//if (userBalance == scans.orderId)
				
					int userId = userBalance; // Set a user ID that you would like to retrieve
					var DbContext = new ApplicationDbContext (); // Your entity framework DbContext

					// Retrieve a user from the database
					var user = DbContext.Set<Order>().Find(userId);
					// Update a property on your user
					user.status = "DELIVERED";
					TempData.Keep("idt");
					TempData["idt"] = userId;
					TempData.Keep("idt");

					// Save the new value to the database
					DbContext.SaveChanges();


				
			}


			catch (NullReferenceException e)
			{
				return RedirectToAction("OrderDeliverd", e);
			}




			return RedirectToAction("OrderDeliverd");

		}




	}
}



