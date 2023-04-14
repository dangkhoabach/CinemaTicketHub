using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    public class ShowtimesManageController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Admin/ShowtimesManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View(_dbContext.SuatChieu.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Phim = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).ToList();
            ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult SaveShowtimes(SuatChieu suatchieu)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Phim = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).ToList();
                    ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();
                    return View("Create", suatchieu);
                }

                _dbContext.SuatChieu.Add(suatchieu);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                
            }
            return RedirectToAction("List", "ShowtimesManage");
        }

        public ActionResult Details(int MaSuatChieu)
        {
            SuatChieu suatchieu = _dbContext.SuatChieu.Find(MaSuatChieu);
            ViewBag.Phim = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).ToList();
            ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();
            return View(suatchieu);
        }

        [HttpPost]
        public ActionResult Edit(SuatChieu suatchieu)
        {
            var item = _dbContext.SuatChieu.Find(suatchieu.MaSuatChieu);
            try
            {
                if (item == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Phim = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).ToList();
                ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();

                item.NgayChieu = suatchieu.NgayChieu;
                item.GioBatDau = suatchieu.GioBatDau;
                item.GioKetThuc = suatchieu.GioKetThuc;
                item.MaPhim = suatchieu.MaPhim;
                item.MaPhong = suatchieu.MaPhong;

                _dbContext.SuatChieu.AddOrUpdate(item);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                
            }
            return RedirectToAction("List", "ShowtimesManage");
        }

        public ActionResult Delete(SuatChieu suatchieu)
        {
            var item = _dbContext.SuatChieu.Find(suatchieu.MaSuatChieu);
            if (item == null)
            {
                return HttpNotFound();
            }
            _dbContext.SuatChieu.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction("List", "ShowtimesManage");
        }
    }
}