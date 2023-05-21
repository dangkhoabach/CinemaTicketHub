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
    [Authorize(Roles = "Admin, Master")]
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
            return View(_dbContext.SuatChieu.OrderBy(o => o.NgayChieu).ThenBy(t => t.GioBatDau).ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Phim = _dbContext.Phim.Where(p => p.TrangThai == true && p.NgayKhoiChieu < DateTime.Now).OrderBy(o => o.TenPhim).ToList();
            ViewBag.PhongChieu = _dbContext.PhongChieu.OrderBy(o => o.TenPhong).ToList();
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

                Ghe ghe;
                for(int i = 1; i <= 5; i++)
                {
                    for(int j = 1; j <= 10; j++)
                    {
                        ghe = new Ghe();
                        ghe.Cot = j;
                        if(i == 1)
                        {
                            ghe.Day = "A";
                        }
                        else
                        {
                            if(i == 2)
                            {
                                ghe.Day = "B";
                            }
                            else
                            {
                                if(i == 3)
                                {
                                    ghe.Day = "C";
                                }    
                                else
                                {
                                    if(i == 4)
                                    {
                                        ghe.Day = "D";
                                    }    
                                    else
                                    {
                                        ghe.Day = "E";
                                    }    
                                }    
                            }
                        }
                        ghe.MaSuatChieu = suatchieu.MaSuatChieu;
                        ghe.TrangThai = false;
                        _dbContext.Ghe.Add(ghe);
                    }
                }

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
            var ghe = _dbContext.Ghe.Where(x => x.MaSuatChieu == suatchieu.MaSuatChieu).ToList();
            foreach(var itemghe in ghe)
            {
                _dbContext.Ghe.Remove(itemghe);
            }    
            _dbContext.SuatChieu.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction("List", "ShowtimesManage");
        }
    }
}