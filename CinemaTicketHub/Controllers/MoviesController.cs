using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
    public class MoviesController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Movies
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NowShowing()
        {
            var nowshowing = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).ToList();
            return View(nowshowing);
        }

        public ActionResult ComingSoon()
        {
            var comingsoon = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu > DateTime.Now).ToList();
            return View(comingsoon);
        }

        public ActionResult Details(string MaPhim)
        {
            Phim phim = _dbContext.Phim.Find(MaPhim);
            return View(phim);
        }
    }
}