using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var nowshowing = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).OrderByDescending(o => o.NgayKhoiChieu).ToList();
            return View(nowshowing);
        }

        public ActionResult ComingSoon()
        {
            var comingsoon = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu > DateTime.Now).OrderBy(o => o.NgayKhoiChieu).ToList();
            return View(comingsoon);
        }

        public ActionResult Details(string MaPhim, DateTime? selectedDate)
        {
            Phim phim = _dbContext.Phim.Find(MaPhim);

            if (selectedDate.HasValue)
            {
                DateTime selectedDateTruncated = selectedDate.Value.Date;

                ViewBag.SuatChieu = _dbContext.SuatChieu.Where(m => m.MaPhim == MaPhim && DbFunctions.TruncateTime(m.NgayChieu) == selectedDateTruncated).ToList();
            }
            else
            {
                ViewBag.SuatChieu = _dbContext.SuatChieu.Where(m => m.MaPhim == MaPhim && DbFunctions.TruncateTime(m.NgayChieu) == DateTime.Today).ToList();
            }

            return View(phim);
        }
    }
}