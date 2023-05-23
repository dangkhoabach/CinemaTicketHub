using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Nowshowing = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).OrderByDescending(o => o.NgayKhoiChieu).Take(3).ToList();
            ViewBag.Comingsoon = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu > DateTime.Now).OrderByDescending(o => o.NgayKhoiChieu).Take(3).ToList();

            var allnowshowing = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).OrderByDescending(o => o.NgayKhoiChieu).ToList();
            var randomMovies = allnowshowing.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            return View(randomMovies);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Chờ đợi bạn Huy :)).";
            return View();
        }
    }
}