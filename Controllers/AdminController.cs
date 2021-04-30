using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using GymApplication.Models;

namespace GymApplication.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string UserName = form["txtEmail"];
            string email = form["txtEmail"];
            string pwd = form["txtPassword"];

            var user = new ApplicationUser();
            user.UserName = UserName;
            user.Email = email;

            var newuser = userManager.Create(user, pwd);
            TempData["Message"] = "<script>alert('User added!');</script>";
            return View();
        }
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewRole(FormCollection form)
        {

            string rolename = form["RoleName"];
            var roleMananger = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleMananger.RoleExists(rolename))
            {
                var role = new IdentityRole(rolename);
                roleMananger.Create(role);
            }

            TempData["Message"] = "<script>alert('Role added!');</script>";
            return View("Index");

        }
        public ActionResult DashBoard()
        {

            return PartialView("DashBoard");
        }
        public ActionResult AssignRole()
        {
            ViewBag.Users = context.Users.Select(u => new SelectListItem { Value = u.Id, Text = u.Email }).ToList();
            ViewBag.Roles = context.Roles.Select(c => new SelectListItem { Value = c.Name, Text = c.Name }).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {

            string userid = form["txtUserid"];
            string rolename = form["RoleName"];
            
            //            ApplicationUser user = context.Users.Where(w => w.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(userid, rolename);
            TempData["Message"] = "<script>alert('Role assigned to user!');</script>";
            return View("Index");

        }
        //public ActionResult AssignCustomer(string userid, string rolename)
        //{
        //    //string userid = form["txtUserid"];
        //    //string rolename = form["RoleName"];
        //    //            ApplicationUser user = context.Users.Where(w => w.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        //    userManager.AddToRole(userid, rolename);
        //    return View("Index");
        //}
    }
}