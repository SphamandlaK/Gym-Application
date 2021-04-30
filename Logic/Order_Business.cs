using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymApplication.Models;

namespace GymApplication.Logic
{
    public class Order_Business
    {
         private ApplicationDbContext db = new ApplicationDbContext();

        public List<Order> cust_all()
        {
            return db.Orders.ToList();
        }
        public List<Order> cust_find_by_status(string status)
        {
            return db.Orders.Where(p => p.status.ToLower() == status.ToLower()).ToList();
        }
        public Order cust_find_by_id(int? id)
        {
            return db.Orders.Find(id);
        }
        public List<Order_Item> cust_Order_items(int? id)
        {
            return cust_find_by_id(id).Order_Items.ToList();
        }

        public List<Order_Tracking> get_tracking_report(int? id)
        {
            return db.Order_Trackings.Where(x => x.order_ID == id).ToList();
        }
        public void mark_as_packed(int? id)
        {
            var order = cust_find_by_id(id);
            //order.packed = true;
            if (db.Order_Addresses.Where(p => p.Order_ID == id) != null)
            {
                order.status = "With courier";
            
                db.Order_Trackings.Add(new Order_Tracking()
                {
                    order_ID = order.Order_ID,
                    date = DateTime.Now,
                    status = "With courier",
                Recipient = ""
                });
            }


            db.SaveChanges();
        }
        public void mark_as_paid(int? id)
        {
            var order = cust_find_by_id(id);
            //order.packed = true;
            if (db.Order_Addresses.Where(p => p.Order_ID == id) != null)
            {
                order.status = "Payment Received";

                db.Order_Trackings.Add(new Order_Tracking()
                {
                    order_ID = order.Order_ID,
                    date = DateTime.Now,
                    status = "Payment Received",
                    Recipient = ""
                });
            }


            db.SaveChanges();
        }
        public void mark_as_EnRoute(int? id)
        {
            var order = cust_find_by_id(id);
            //order.packed = true;
            if (db.Order_Addresses.Where(p => p.Order_ID == id) != null)
            {
                order.status = "En route for delivery";

                db.Order_Trackings.Add(new Order_Tracking()
                {
                    order_ID = order.Order_ID,
                    date = DateTime.Now,
                    status = "En route for delivery",
                    Recipient = ""
                });
            }


            db.SaveChanges();
        }
        public void schedule_delivery(int? order_Id)
        {
            DateTime mydate = DateTime.Now;
            var order = cust_find_by_id(order_Id);
            order.status = "Scheduled for delivery"+" "+mydate.AddDays(2);
            //order tracking
            db.Order_Trackings.Add(new Order_Tracking()
            {
                order_ID = order.Order_ID,
                date = mydate,
                //date.AddDays(1),
                status = "Scheduled for delivery",
                Recipient = ""
            });
            db.SaveChanges();
        }
        public void mark_as_Delivered(int? order_Id)
        {
            DateTime mydate = DateTime.Now;
            var order = cust_find_by_id(order_Id);
            order.status = "Delivered";
            //order tracking
            db.Order_Trackings.Add(new Order_Tracking()
            {
                order_ID = order.Order_ID,
                date = mydate,
                //date.AddDays(1),
                status = "Delivered",
                Recipient = ""
            });
            db.SaveChanges();
        }
    }
}