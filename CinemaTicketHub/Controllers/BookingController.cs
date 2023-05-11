using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
    public class BookingController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SeatSelection(int MaSuatChieu)
        {
            /*var seat = _dbContext.Ghe.Where(p => p.MaSuatChieu == MaSuatChieu).OrderBy(o => o.Day).ThenBy(t => t.Cot).ToList();*/
            ViewBag.rowA = _dbContext.Ghe.Where(x => x.MaSuatChieu == MaSuatChieu && x.Day == "A").ToList();
            ViewBag.rowB = _dbContext.Ghe.Where(x => x.MaSuatChieu == MaSuatChieu && x.Day == "B").ToList();
            ViewBag.rowC = _dbContext.Ghe.Where(x => x.MaSuatChieu == MaSuatChieu && x.Day == "C").ToList();
            ViewBag.rowD = _dbContext.Ghe.Where(x => x.MaSuatChieu == MaSuatChieu && x.Day == "D").ToList();
            ViewBag.rowE = _dbContext.Ghe.Where(x => x.MaSuatChieu == MaSuatChieu && x.Day == "E").ToList();

            var moviesdetail = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == MaSuatChieu).FirstOrDefault();
            return View(moviesdetail);
        }
    }
}