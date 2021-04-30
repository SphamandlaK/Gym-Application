using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using Microsoft.AspNet.Identity;
using PayFast;
using PayFast.AspNet;

namespace GymApplication.Controllers
{
	public class RentEquipmentsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: RentEquipments
		public ActionResult Index()
		{
			var rentEquipments = db.RentEquipments.Include(r => r.Item);
			return View(rentEquipments.ToList());
		}
		public ActionResult HireEquipment()
		{

			var rentEquipments = db.RentEquipments.Include(r => r.Item);
			return View(rentEquipments.ToList());
		}
		public ActionResult RentalView()
		{
			var rent = new RentEquipment();
			var user = User.Identity.GetUserName();
			rent.UserEmail = User.Identity.GetUserName();
			rent.FullName = db.Users.Where(x => x.Email == User.Identity.Name).Select(p => p.FirstName).Single() + " " + db.Users.Where(x => x.Email == User.Identity.Name).Select(i => i.LastName).Single();
			var rentEquipments = db.RentEquipments.Include(r => r.Item);
			return View(rentEquipments.ToList().Where(x => x.UserEmail == user));
		}
		public ActionResult MyEquipmentBooking()
		{
			var rent = new RentEquipment();
			var user = User.Identity.GetUserName();
			rent.UserEmail = User.Identity.GetUserName();
			rent.FullName = db.Users.Where(x => x.Email == User.Identity.Name).Select(p => p.FirstName).Single() + " " + db.Users.Where(x => x.Email == User.Identity.Name).Select(i => i.LastName).Single();
			var rentEquipments = db.RentEquipments.Include(r => r.Item);
			return View(rentEquipments.ToList().Where(x => x.UserEmail == user));
		}
		public ActionResult ConfirmBooking(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}
			return View(rentEquipment);
		}
		public ActionResult ConfirmPickUp(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}
			return View(rentEquipment);
		}
		public ActionResult ConfirmCollection(int? id)
		{
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			rentEquipment.status = "Picked Up";
			db.Entry(rentEquipment).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("Index");

		}
		public ActionResult ReturnEquipment(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			ViewBag.PickUpDate = rentEquipment.Date_From.Date;
			ViewBag.ReturnDate = rentEquipment.Date_To.Date;
			ViewBag.userName = rentEquipment.UserEmail;
			ViewBag.status = rentEquipment.status;
			ViewBag.depos = rentEquipment.Deposit;
			ViewBag.numofdays = rentEquipment.numOfDays;
			ViewBag.bookicost = rentEquipment.Cost_Fee;
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", rentEquipment.ItemCode);
			return View(rentEquipment);
		}
		public ActionResult AvailableEquipment(int? id)
		{
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			rentEquipment.status = "Completed";
			rentEquipment.Cost_Fee = Convert.ToDecimal(rentEquipment.CalcCostCharge(rentEquipment));
			rentEquipment.numOfDays = Convert.ToInt32(rentEquipment.CalcNum_of_Days(rentEquipment));
			Item item = db.Items.ToList().Find(x => x.ItemCode == rentEquipment.ItemCode);
			item.Status = "Available";
			db.Entry(rentEquipment).State = EntityState.Modified;
			db.Entry(item).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("MyEquipmentBooking", "RentEquipments", rentEquipment);
		}
		// GET: RentEquipments/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}
			return View(rentEquipment);
		}

		// GET: RentEquipments/Create
		public ActionResult Create(int? id)
		{
			ViewBag.Id = id;
			var rent = new RentEquipment();
			rent.UserEmail = User.Identity.GetUserName();
			rent.FullName = db.Users.Where(x => x.Email == User.Identity.Name).Select(p => p.FirstName).Single() + " " + db.Users.Where(x => x.Email == User.Identity.Name).Select(i => i.LastName).Single();

			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", rent.ItemCode);
			return View(rent);
		}

		// POST: RentEquipments/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,ItemCode,UserEmail,FullName,Date_From,Date_To,Cost_Fee,Deposit,numOfDays,Total,status")] RentEquipment rentEquipment)
		{
			//var userName = User.Identity.GetUserName();
			//rentEquipment.UserEmail = userName;
			if (ModelState.IsValid)
			{
				rentEquipment.Cost_Fee = rentEquipment.CalcCostCharge(rentEquipment);
				rentEquipment.numOfDays = rentEquipment.CalcNum_of_Days(rentEquipment);
				
				db.RentEquipments.Add(rentEquipment);
				db.SaveChanges();
				return RedirectToAction("ConfirmBooking", "RentEquipments", new { id = rentEquipment.Id });
			}

			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name", rentEquipment.ItemCode);
			return View(rentEquipment);
		}

		// GET: RentEquipments/Edit/5
		public ActionResult Edit(int? id)
		{

			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name");
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}

			return View(rentEquipment);
		}

		// POST: RentEquipments/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,ItemCode,UserEmail,FullName,Date_From,Date_To,Cost_Fee,Deposit,numOfDays,Total,status")] RentEquipment rentEquipment)
		{
			if (ModelState.IsValid)
			{
				Item item = db.Items.ToList().Find(x => x.ItemCode == rentEquipment.ItemCode);
				db.Entry(item).State = EntityState.Modified;
				db.Entry(rentEquipment).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("ReturnEquipment", "RentEquipments", new { id = rentEquipment.Id });
			}
			ViewBag.ItemCode = new SelectList(db.Items, "ItemCode", "Name");
			return View(rentEquipment);
		}

		// GET: RentEquipments/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			if (rentEquipment == null)
			{
				return HttpNotFound();
			}
			return View(rentEquipment);
		}

		// POST: RentEquipments/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			db.RentEquipments.Remove(rentEquipment);
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
		public ActionResult ThankYouPage()
		{
			return View();
		}
		public RentEquipmentsController()
		{
			this.payFastSettings = new PayFastSettings();
			this.payFastSettings.MerchantId = ConfigurationManager.AppSettings["MerchantId"];
			this.payFastSettings.MerchantKey = ConfigurationManager.AppSettings["MerchantKey"];
			this.payFastSettings.PassPhrase = ConfigurationManager.AppSettings["PassPhrase"];
			this.payFastSettings.ProcessUrl = ConfigurationManager.AppSettings["ProcessUrl"];
			this.payFastSettings.ValidateUrl = ConfigurationManager.AppSettings["ValidateUrl"];
			this.payFastSettings.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
			this.payFastSettings.CancelUrl = ConfigurationManager.AppSettings["CancelUrl"];
			this.payFastSettings.NotifyUrl = ConfigurationManager.AppSettings["NotifyUrl"];
		}
		//Payment
		#region Fields

		private readonly PayFastSettings payFastSettings;

		#endregion Fields

		#region Constructor

		//public ApprovedOwnersController()
		//{

		//}

		#endregion Constructor

		#region Methods



		public ActionResult Recurring()
		{
			var recurringRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

			// Merchant Details
			recurringRequest.merchant_id = this.payFastSettings.MerchantId;
			recurringRequest.merchant_key = this.payFastSettings.MerchantKey;
			recurringRequest.return_url = this.payFastSettings.ReturnUrl;
			recurringRequest.cancel_url = this.payFastSettings.CancelUrl;
			recurringRequest.notify_url = this.payFastSettings.NotifyUrl;

			// Buyer Details
			recurringRequest.email_address = "sbtu01@payfast.co.za";

			// Transaction Details
			recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
			recurringRequest.amount = 20;
			recurringRequest.item_name = "Recurring Option";
			recurringRequest.item_description = "Some details about the recurring option";

			// Transaction Options
			recurringRequest.email_confirmation = true;
			recurringRequest.confirmation_address = "drnendwandwe@gmail.com";

			// Recurring Billing Details
			recurringRequest.subscription_type = SubscriptionType.Subscription;
			recurringRequest.billing_date = DateTime.Now;
			recurringRequest.recurring_amount = 20;
			recurringRequest.frequency = BillingFrequency.Monthly;
			recurringRequest.cycles = 0;

			var redirectUrl = $"{this.payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

			return Redirect(redirectUrl);
		}
		//public ActionResult pay(int? id)
		//{
		//    StudentApplication studentApplication = db.Studentapplications.Find(id);
		//    var priceId = db.ClassFees.Where(p => p.ClassNameId == studentApplication.ClassNameId).Select(p => p.FeeTypeId).FirstOrDefault();
		//    var price = db.FeeTypes.Where(p => p.Id == priceId).Select(p => p.FeeAmount).FirstOrDefault();
		//    studentApplication.Status = "Paid";
		//    db.Entry(studentApplication).State = EntityState.Modified;

		//    // db.Studentapplications.Add(studentApplication);
		//    db.SaveChanges();
		//    return RedirectToAction("Index2");

		//}
		public ActionResult OnceOff(int? id)
		{
			var onceOffRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

			// Merchant Details
			onceOffRequest.merchant_id = this.payFastSettings.MerchantId;
			onceOffRequest.merchant_key = this.payFastSettings.MerchantKey;
			onceOffRequest.return_url = this.payFastSettings.ReturnUrl;
			onceOffRequest.cancel_url = this.payFastSettings.CancelUrl;
			onceOffRequest.notify_url = this.payFastSettings.NotifyUrl;

			// Buyer Details

			onceOffRequest.email_address = "sbtu01@payfast.co.za";
			//onceOffRequest.email_address = "sbtu01@payfast.co.za";

			// Transaction Details
			RentEquipment rentEquipment = db.RentEquipments.Find(id);
			rentEquipment.status = "Paid";
			db.Entry(rentEquipment).State = EntityState.Modified;
			db.SaveChanges();

			Item item = db.Items.ToList().Find(x => x.ItemCode == rentEquipment.ItemCode);
			item.Status = "Not Available";
			db.Entry(item).State = EntityState.Modified;
			db.SaveChanges();

			onceOffRequest.m_payment_id = "";
			onceOffRequest.amount = rentEquipment.Deposit;
			onceOffRequest.item_name = "Gym Equipment Payment";
			onceOffRequest.item_description = "Some details about the once off payment";
			var userName = User.Identity.GetUserName();

			//var mailTo = new List<MailAddress>();
			//mailTo.Add(new MailAddress(userName, carHiring.CustomerName));
			//var body = $"Dear {carHiring.CustomerName} {carHiring.CustomerSurname} <br/><br/>" +
			//	$"Your car hiring was successful, please see details below: <br/><br/>" +
			//	$"Hire Date:{System.DateTime.Now.Date} \n <br/>" +
			//	$"Pick-Up Date: {carHiring.PickUpDate}\n <br/>" +
			//	$"Return-Date: {carHiring.ReturnDate}\n <br/>" +
			//	$"Number Of Days: {carHiring.numOfDays}\n <br/>" +
			//	$"Deposit: {carHiring.Deposit}\n <br/>" +
			//	$"Daily Cost: {carHiring.car.Cost_Per_Day}\n <br/>" +
			//	$"Vehicle: {carHiring.car.carMake.CarMakeType} {carHiring.car.carModel.CarModelType}\n <br/>" +
			//	$"Transmission: {carHiring.car.transmission.TransmissionType}\n <br/>" +
			//	$"Fuel: {carHiring.car.fuel.FuelType}<br/>" +
			//	$"Regards,<br/><br/> Lindani Driving School <br/> .";

			//LindaniDrivingSchool.Logic.EmailService emailService = new LindaniDrivingSchool.Logic.EmailService();
			//emailService.SendEmail(new EmailContent()
			//{
			//	mailTo = mailTo,
			//	mailCc = new List<MailAddress>(),
			//	mailSubject = "Car Hire Confirmation | Ref No.:" + carHiring.BookingId,
			//	mailBody = body,
			//	mailFooter = "<br/> Many Thanks, <br/> <b>Lindani Driving School</b>",
			//	mailPriority = MailPriority.High,
			//	mailAttachments = new List<Attachment>()

			//});
			// Transaction Options
			onceOffRequest.email_confirmation = true;
			onceOffRequest.confirmation_address = "sbtu01@payfast.co.za";

			var redirectUrl = $"{this.payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";
			return Redirect(redirectUrl);
		}

		public ActionResult AdHoc()
		{
			var adHocRequest = new PayFastRequest(this.payFastSettings.PassPhrase);

			// Merchant Details
			adHocRequest.merchant_id = this.payFastSettings.MerchantId;
			adHocRequest.merchant_key = this.payFastSettings.MerchantKey;
			adHocRequest.return_url = this.payFastSettings.ReturnUrl;
			adHocRequest.cancel_url = this.payFastSettings.CancelUrl;
			adHocRequest.notify_url = this.payFastSettings.NotifyUrl;

			// Buyer Details
			adHocRequest.email_address = "sbtu01@payfast.co.za";

			// Transaction Details
			adHocRequest.m_payment_id = "";
			adHocRequest.amount = 70;
			adHocRequest.item_name = "Adhoc Agreement";
			adHocRequest.item_description = "Some details about the adhoc agreement";

			// Transaction Options
			adHocRequest.email_confirmation = true;
			adHocRequest.confirmation_address = "sbtu01@payfast.co.za";

			// Recurring Billing Details
			adHocRequest.subscription_type = SubscriptionType.AdHoc;

			var redirectUrl = $"{this.payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

			return Redirect(redirectUrl);
		}

		public ActionResult Return()
		{
			return View();
		}

		public ActionResult Cancel()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Notify([ModelBinder(typeof(PayFastNotifyModelBinder))] PayFastNotify payFastNotifyViewModel)
		{
			payFastNotifyViewModel.SetPassPhrase(this.payFastSettings.PassPhrase);

			var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();

			var isValid = payFastNotifyViewModel.signature == calculatedSignature;

			System.Diagnostics.Debug.WriteLine($"Signature Validation Result: {isValid}");

			// The PayFast Validator is still under developement
			// Its not recommended to rely on this for production use cases
			var payfastValidator = new PayFastValidator(this.payFastSettings, payFastNotifyViewModel, IPAddress.Parse(this.HttpContext.Request.UserHostAddress));

			var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

			System.Diagnostics.Debug.WriteLine($"Merchant Id Validation Result: {merchantIdValidationResult}");

			var ipAddressValidationResult = payfastValidator.ValidateSourceIp();

			System.Diagnostics.Debug.WriteLine($"Ip Address Validation Result: {merchantIdValidationResult}");

			// Currently seems that the data validation only works for successful payments
			if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
			{
				var dataValidationResult = await payfastValidator.ValidateData();

				System.Diagnostics.Debug.WriteLine($"Data Validation Result: {dataValidationResult}");
			}

			if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
			{
				System.Diagnostics.Debug.WriteLine($"Subscription was cancelled");
			}

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		public ActionResult Error()
		{
			return View();
		}

		#endregion Methods
	}
}
