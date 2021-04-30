using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using GymApplication.Logic;

namespace GymApplication.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders

        // GET: Orders
        private ApplicationDbContext db = new ApplicationDbContext();

        Order_Business ob = new Order_Business();

        //Customer orders
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "Mananger")]
        //[Authorize(Roles = "Mananger")]




        public ActionResult Customer_Orders(string id)
        {
            if (String.IsNullOrEmpty(id) || id == "all")
            {
                ViewBag.Status = "All";
                return View(ob.cust_all());
            }
            else
            {
                ViewBag.Status = id;
                return View(ob.cust_find_by_status(id));
            }
        }
    public ActionResult Order_List(string id)
    {
        if (String.IsNullOrEmpty(id) || id == "all")
        {
            ViewBag.Status = "All";
            return View(ob.cust_all().Where(x=>x.isAssigned ==false));
        }
        else
        {
            ViewBag.Status = id;
            return View(ob.cust_find_by_status(id).Where(x => x.isAssigned == false));
        }
    }


    public ActionResult New_Orders(string id)
        {
            if (String.IsNullOrEmpty(id) || id == "all")
            {
                ViewBag.Status = "All";
                return View(ob.cust_all());
            }
            else
            {
                ViewBag.Status = id;
                return View(ob.cust_find_by_status(id));
            }
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

        public ActionResult Order_Tracking(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ViewBag.Order = ob.cust_find_by_id(id);
                return View(ob.get_tracking_report(id));
            }
            else
                return RedirectToAction("Not_Found", "Error");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Mark_As_Packed(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ob.mark_as_packed(id);
                return RedirectToAction("Order_Details", new { id = id });
            }
            else
                return RedirectToAction("Not_Found", "Error");
        }
        public ActionResult PaymentReceived(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ob.mark_as_paid(id);
                return RedirectToAction("Order_Details", new { id = id });
            }
            else
                return RedirectToAction("Not_Found", "Error");
        }

        public ActionResult EnRoute(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ob.mark_as_EnRoute(id);
                return RedirectToAction("Order_Details", new { id = id });
            }
            else
                return RedirectToAction("Not_Found", "Error");
        }

        //Need this later(Assigning Oders)
        public ActionResult schedulered(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ob.schedule_delivery(id);
                return RedirectToAction("Order_Details", new { id = id });
            }
            else
                return RedirectToAction("Not_Found", "Error");
        }

        public ActionResult Delivered(int? id)
        {
            if (id == null)
                return RedirectToAction("Bad_Request", "Error");
            if (ob.cust_find_by_id(id) != null)
            {
                ob.mark_as_Delivered(id);
                return RedirectToAction("Order_Details", new { id = id });
            }
            else
                return RedirectToAction("Not_Found", "Error");
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

        //account orders
        public ActionResult Order_History()
        {
            return View(ob.cust_all().Where(x => x.Customer.Email == User.Identity.Name));
        }
    
    }
}

