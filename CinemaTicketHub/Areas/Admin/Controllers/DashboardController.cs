using CinemaTicketHub.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Master")]
    public class DashboardController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Admin/Dashboard
        public ActionResult OrdersStatistics()
        {
            return View();
        }

        public ActionResult TicketsStatistics()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in _dbContext.HoaDon/*
                        join od in _dbContext.Ve
                        on o.NgayLap equals od.NgayBanVe*/
                        select new
                        {
                            NgayLap = o.NgayLap,
                            TongTien = o.TongTien,/*
                            GiaVe = od.GiaVe,*/
                        };
            if (!String.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.NgayLap >= startDate);
            }
            if (!String.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.NgayLap < endDate);
            }
            //truncatetime : lấy ngày bỏ giờ
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.NgayLap)).Select(x => new
            {
                Date = x.Key.Value,
                DoanhThuDoAn = x.Sum(z => z.TongTien),/*
                DoanhThuBanVe = x.Sum(y=>y.GiaVe),*/
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.DoanhThuDoAn,/*
                BanVe = x.DoanhThuBanVe,*/
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStatisticalVe(string fromDate, string toDate)
        {
            var query = from o in _dbContext.HoaDon
                        join od in _dbContext.Ve
                        on o.MaHoaDon equals od.MaHoaDon
                        select new
                        {
                            NgayLap = o.NgayLap,
                            GiaVe = od.GiaVe,
                        };
            if (!String.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.NgayLap >= startDate);
            }
            if (!String.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.NgayLap < endDate);
            }
            //truncatetime : lấy ngày bỏ giờ
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.NgayLap)).Select(x => new
            {
                Date = x.Key.Value,
                DoanhThuBanVe = x.Sum(z => z.GiaVe),
            }).Select(x => new
            {
                Date = x.Date,
                BanVe = x.DoanhThuBanVe,
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}