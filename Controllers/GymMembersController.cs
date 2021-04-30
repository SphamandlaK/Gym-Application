using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GymApplication.Models;
using GymApplication.Common;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.IO;

namespace GymApplication.Controllers
{   [Authorize]
    public class GymMembersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

      
        
        // GET: GymMembers
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            GymMember memberProfile = db.GymMember.Where(p => p.ApplicationUser.Id == id).FirstOrDefault();
            if (memberProfile == null)
            {
                string name = User.Identity.Name;
                if (name.Equals("admin@gmail.com"))
                { return RedirectToAction("GymMembersList", "Admin"); }
                else{ return RedirectToAction("Create"); }
            }
            return View();
        }

       

        // GET: GymMembers/UserProfile/
        public ActionResult UserProfile()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
               return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GymMember memberProfile = db.GymMember.Where(p => p.ApplicationUser.Id == id).FirstOrDefault();
            if (memberProfile == null)
            {
                string name = User.Identity.Name;
                if (name.Equals("admin@gmai.com"))
                { return RedirectToAction("GymMembersList", "Admin"); }
                else { return RedirectToAction("Create"); }
            }
            return View(memberProfile);
        }

        // GET: GymMembers/Create
        public ActionResult Create()
        {

            var gymMemberobj = new GymMember();
            gymMemberobj.UserEmail = User.Identity.GetUserName();
            gymMemberobj.FirstName = db.Users.Where(x => x.Email == User.Identity.Name).Select(p => p.FirstName).Single();
            gymMemberobj.LastName = db.Users.Where(x => x.Email == User.Identity.Name).Select(a => a.LastName).Single();
            gymMemberobj.Address = db.Customers.Where(x => x.Email == User.Identity.Name).Select(w => w.address).Single();

            ViewBag.MembershipType = new SelectList(db.MembershipType, "Membership_Id", "Membership_tier");
            return View(gymMemberobj);
        }

        // POST: GymMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Member_Id,FirstName,LastName,UserEmail,DateOfBirth,Address,Height,Weight,MembershipType")] GymMember gymMember)
        {
            gymMember.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
            ModelState.Clear();
            TryValidateModel(gymMember);
            if (ModelState.IsValid)
            {
                gymMember.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
                
                db.GymMember.Add(gymMember);
                db.SaveChanges();
                String toEmail = gymMember.ApplicationUser.Email;
                String subject = "GymApplication Registeration Confirmation";
                String contents = String.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Email_Template/Email_Contents.html")))
                {
                    contents = reader.ReadToEnd();
                }
                
                EmailSender es = new EmailSender();
                es.Send(toEmail, subject, contents);
                return RedirectToAction("Index");
            }
            ViewBag.MembershipType = new SelectList(db.MembershipType, "Membership_Id", "Membership_tier");
            return View(gymMember);
        }

        


        // GET: GymMembers/Edit/5
        public ActionResult Edit(int? id)
        {

            ViewBag.MembershipType = new SelectList(db.MembershipType, "Membership_Id", "Membership_tier");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymMember gymMember = db.GymMember.Find(id);
            if (gymMember == null)
            {
                return HttpNotFound();
            }
            return View(gymMember);
        }

        // POST: GymMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Member_Id,FirstName,LastName,UserEmail,DateOfBirth,Address,Height,Weight,MembershipType")] GymMember gymMember)
        {
            gymMember.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
            ModelState.Clear();
            TryValidateModel(gymMember);

            if (ModelState.IsValid)
            {
                db.Entry(gymMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MembershipType = new SelectList(db.MembershipType, "Membership_Id", "Membership_tier");
            return View(gymMember);
        }

        /// <summary>  
        /// Validate Captcha  
        /// </summary>  
        /// <param name="response"></param>  
        /// <returns></returns>  
        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        // GET: Posts list
        public ActionResult MemberPostsList()
        {
            string id = User.Identity.GetUserId();
            GymMember memberProfile = db.GymMember.Where(p => p.ApplicationUser.Id == id).FirstOrDefault();
            if (memberProfile == null)
            {
                string name = User.Identity.Name;
                if (name.Equals("admin@gmail.com"))
                { return RedirectToAction("GymMembersList", "Admin"); }
                else { return RedirectToAction("Create"); }
            }
            return View(db.Posts.ToList());
        }


        // GET: PostMessages/Details/5
        
        public ActionResult MemberPostDetails(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            PostMessageDetailsViewModel postMessageDetailsViewModel = new PostMessageDetailsViewModel();
            var postMessage = db.PostMessages.Where(m => m.Post.Post_Id == id).ToList();
            postMessageDetailsViewModel.postMessages = postMessage;
            postMessageDetailsViewModel.post_id = post.Post_Id;
            if (postMessageDetailsViewModel == null)
            {
                return HttpNotFound();
            }
            return View(postMessageDetailsViewModel);
        }


        // POST: Posts/Detials
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MemberPostDetails([Bind(Include = "post_message")] PostMessageDetailsViewModel postMessageDetailsViewModel,string btn, HttpPostedFileBase postedFile)
        {
            CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
            if (response.Success)
            {
                var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
               
                if (ModelState.IsValid)
                {
                    int post_id = Convert.ToInt32(btn);
                    PostMessage postMessage = new PostMessage();
                    Image image = new Image();
                    postMessage.ApplicationUser = db.Users.Find(User.Identity.GetUserId());
                    postMessage.Post_Message = postMessageDetailsViewModel.post_message;
                    Post post = db.Posts.Find(post_id);
                    postMessage.Post = post;
                    if (postedFile != null)
                    {
                        image.Image_Path = myUniqueFileName;
                        string serverPath = Server.MapPath("~/Uploads/");
                        string fileExtension = Path.GetExtension(postedFile.FileName);
                        string filePath = image.Image_Path + fileExtension;
                        image.Image_Path = filePath;
                        postedFile.SaveAs(serverPath + image.Image_Path);
                        db.Images.Add(image);
                        db.SaveChanges();
                        postMessage.Image = image;
                    }
                    
                    
                    db.PostMessages.Add(postMessage);
                    db.SaveChanges();
                    return RedirectToAction("MemberPostDetails");
                }
                else { return View(postMessageDetailsViewModel); }
            }
            else
            {
                return Content("Error From Google ReCaptcha : " + response.ErrorMessage[0].ToString());
            }
        }




        // GET: Posts/DeletePostMessage/5
        public ActionResult DeletePostMessage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostMessage postMessage = db.PostMessages.Find(id);
            if (postMessage == null)
            {
                return HttpNotFound();
            }
            return View(postMessage);
        }

        // POST: Posts/DeletePostMessage/5
        [HttpPost, ActionName("DeletePostMessage")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostMessageConfirmed(int id)
        {
            PostMessage postMessage = db.PostMessages.Find(id);
            int post_id = postMessage.Post.Post_Id;
            db.PostMessages.Remove(postMessage);
            db.SaveChanges();
            return RedirectToAction("MemberPostDetails", new { id = post_id });
        }



        // GET: PostMessages/EditPostMessage/5
        public ActionResult EditPostMessage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostMessage postMessage = db.PostMessages.Find(id);
            if (postMessage == null)
            {
                return HttpNotFound();
            }
            PostMessageViewModel postMessageViewModel = new PostMessageViewModel();
            postMessageViewModel.post_message_id = postMessage.Post_Message_Id;
            postMessageViewModel.post_message = postMessage.Post_Message;
            int message_id = postMessage.Post_Message_Id;
            int post_id = db.PostMessages.Where(p => p.Post_Message_Id == message_id).Select(p => p.Post.Post_Id).FirstOrDefault();
            Post post = db.Posts.Find(post_id);
            //post.Post_Title = postMessageViewModel.post_title;
            postMessageViewModel.post_title = post.Post_Title;


            return View(postMessageViewModel);
        }

        // POST: PostMessages/EditPostMessage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPostMessage([Bind(Include = "post_title,post_message,post_message_id")] PostMessageViewModel postMessageViewModel)
        {
            int message_id = postMessageViewModel.post_message_id;
            
            PostMessage postMessage = new PostMessage();
            postMessage.Post_Message = postMessageViewModel.post_message;
            postMessage.Post_Message_Id = postMessageViewModel.post_message_id;
            postMessage.ApplicationUser = db.PostMessages.Where(p => p.Post_Message_Id == message_id).Select(p => p.ApplicationUser).FirstOrDefault();
            postMessage.Post = db.PostMessages.Where(p => p.Post_Message_Id == message_id).Select(p => p.Post).FirstOrDefault();
            postMessageViewModel.post_title = postMessage.Post.Post_Title;
            ModelState.Clear();
            TryValidateModel(postMessageViewModel);
            if (ModelState.IsValid)
            {
                
                db.Entry(postMessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MemberPostDetails", new { id = postMessage.Post.Post_Id });
            }
            return View(postMessageViewModel);
        }
    }
}
