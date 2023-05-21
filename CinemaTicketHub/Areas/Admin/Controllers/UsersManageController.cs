using CinemaTicketHub.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    [Authorize(Roles = "Master")]
    public class UsersManageController : Controller
    {
        // GET: Admin/UsersManage
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var users = userManager.Users.OrderBy(o => o.Name).ToList();
            ViewBag.Users = new SelectList(users, "Id", "Email");
            return View();
        }

        public ActionResult DeleteUser(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = userManager.FindById(userId);
            /*string message;*/

            if (user != null)
            {
                var result = userManager.Delete(user);
                /*if (result.Succeeded)
                {
                    message = "Xoa User thanh cong!";
                }
                else
                {
                    message = "Xoa User that bai!";
                }*/
            }
            /*else
            {
                message = "Khong tim thay User";
            }

            TempData["message"] = message;*/

            return RedirectToAction("Index", "UsersManage");
        }
    }
}