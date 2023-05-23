using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Master")]
    public class OrdersManageController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Admin/OrdersManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var lstHoadon = _dbContext.HoaDon.OrderByDescending(o => o.NgayLap).ThenBy(t => t.Id).ToList();
            return View(lstHoadon);
        }

        public ActionResult Details(int MaHoaDon)
        {
            return View();
        }
    }
}