using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using GymApplication.Logic;
using System.IO;

namespace GymApplication.Controllers
{
	public class ItemsController : Controller
	{
		//// GET: Items
		//public ActionResult Index()
		//{
		//    return View();
		//}
		private ApplicationDbContext db = new ApplicationDbContext();
		private Item_Business ib = new Item_Business();
		Category_Business cb = new Category_Business();


		public string shoppingCartID { get; set; }

		public const string CartSessionKey = "CartId";

		public ItemsController() { }

		public ActionResult Index()
		{
			return View(ib.all());
		}
		//[Authorize(Roles = "Admin")]
		public ActionResult HireEquip()
		{
			var item = db.Items.ToList();
			return View(item);
		}
		public ActionResult Details(int? id)
		{
			if (id == null)
				return RedirectToAction("Bad_Request", "Error");
			if (ib.find_by_id(id) != null)
				return View(ib.find_by_id(id));
			else
				return RedirectToAction("Not_Found", "Error");
		}
		public byte[] ConvertToBytes(HttpPostedFileBase file)
		{
			BinaryReader reader = new BinaryReader(file.InputStream);
			return reader.ReadBytes((int)file.ContentLength);
		}

		// Display File
		public FileStreamResult RenderImage(int id)
		{
			MemoryStream ms = null;

			var item = db.Items.FirstOrDefault(x => x.ItemCode == id);
			if (item != null)
			{
				ms = new MemoryStream(item.Picture);
			}
			return new FileStreamResult(ms, item.ImageType);
		}
		//[Authorize(Roles = "Admin")]
		public ActionResult Create()
		{

			ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name");
			ViewBag.Category_ID = new SelectList(cb.all(), "Category_ID", "Category_Name");
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Item model, HttpPostedFileBase img_upload)
		{
			ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name", model.SupplierId);
			ViewBag.Category_ID = new SelectList(cb.all(), "Category_ID", "Category_Name");
			if (img_upload != null && img_upload.ContentLength > 0)
			{
				model.ImageType = Path.GetExtension(img_upload.FileName);
				model.Picture = ConvertToBytes(img_upload);
			}
			//byte[] data = null;
			//data = new byte[img_upload.ContentLength];
			//img_upload.InputStream.Read(data, 0, img_upload.ContentLength);
			//model.Picture = data;
			//- model.QuantityInStock = 0;
			if (ModelState.IsValid)
			{
				model.Price = model.CalculateSellingPrice(model.CostPrice, model.MarkupPercentage);
				ib.add(model);
				return RedirectToAction("Index");
			}
			return View(model);
		}
		//[Authorize(Roles = "Admin")]
		public ActionResult Edit(int? id)
		{
			ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name");
			ViewBag.Category_ID = new SelectList(cb.all(), "Category_ID", "Category_Name");
			if (id == null)
				return RedirectToAction("Bad_Request", "Error");
			if (ib.find_by_id(id) != null)
				return View(ib.find_by_id(id));
			else
				return RedirectToAction("Not_Found", "Error");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Item model, HttpPostedFileBase img_upload)
		{
			byte[] data = null;
			data = new byte[img_upload.ContentLength];
			img_upload.InputStream.Read(data, 0, img_upload.ContentLength);
			model.Picture = data;
			if (ModelState.IsValid)
			{
				ib.edit(model);
				return RedirectToAction("Index");
			}
			ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "Name", model.SupplierId);
			ViewBag.Category_ID = new SelectList(cb.all(), "Category_ID", "Category_Name");
			return View(model);
		}
		//[Authorize(Roles = "Admin")]
		public ActionResult Delete(int? id)
		{
			if (id == null)
				return RedirectToAction("Bad_Request", "Error");
			if (ib.find_by_id(id) != null)
				return View(ib.find_by_id(id));
			else
				return RedirectToAction("Not_Found", "Error");
		}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			ib.delete(ib.find_by_id(id));
			return RedirectToAction("Index");
		}


		public ActionResult Fall_catalog()
		{
			return View(ib.all());
		}

		public string GetCartID()
		{
			if (System.Web.HttpContext.Current.Session[CartSessionKey] == null)
			{
				if (!String.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name))
				{
					System.Web.HttpContext.Current.Session[CartSessionKey] = System.Web.HttpContext.Current.User.Identity.Name;
				}
				else
				{
					Guid temp_cart_ID = Guid.NewGuid();
					System.Web.HttpContext.Current.Session[CartSessionKey] = temp_cart_ID.ToString();
				}
			}
			return System.Web.HttpContext.Current.Session[CartSessionKey].ToString();
		}
		public ActionResult ListItems()
		{
			var items = db.Items.Include("Category");
			return PartialView("ListItems", items);
		}

	}
}