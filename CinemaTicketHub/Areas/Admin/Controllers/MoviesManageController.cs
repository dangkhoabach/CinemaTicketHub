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
    [Authorize(Roles = "Admin")]
    public class MoviesManageController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Admin/MoviesManage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View(_dbContext.Phim.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.TheLoai = _dbContext.LoaiPhim.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult SaveMovies(Phim phim, HttpPostedFileBase HinhAnh)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.TheLoai = _dbContext.LoaiPhim.ToList();
                    return View("Create", phim);
                }

                string path = Path.Combine(Server.MapPath("/Content/images/poster_landscape/"),
                    Path.GetFileName(HinhAnh.FileName));
                HinhAnh.SaveAs(path);
                phim.HinhAnh = "/Content/images/poster_landscape/" + Path.GetFileName(HinhAnh.FileName);

                var List = _dbContext.Phim.ToList();
                Phim item = List.LastOrDefault();

                string MaPhim = "";
                
                MatchCollection matches = Regex.Matches(item.MaPhim, @"\d+");
                foreach (Match match in matches)
                {
                    MaPhim += match.Value;
                }

                int count = int.Parse(MaPhim.ToString()) + 1;
                phim.MaPhim = "MV" + count;

                phim.TrangThai = true;

                _dbContext.Phim.Add(phim);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.FileStatus = "Error update images";
            }
            return RedirectToAction("List", "MoviesManage");
        }

        public ActionResult Details(string MaPhim)
        {
            Phim phim = _dbContext.Phim.Find(MaPhim);
            ViewBag.TheLoai = _dbContext.LoaiPhim.ToList();
            return View(phim);
        }

        [HttpPost]
        public ActionResult Edit(Phim phim, HttpPostedFileBase HinhAnh)
        {
            var item = _dbContext.Phim.Find(phim.MaPhim);
            try
            {
                if (item == null)
                {
                    return HttpNotFound();
                }
                ViewBag.TheLoai = _dbContext.LoaiPhim.ToList();
                if (HinhAnh != null)
                {
                    string path = Path.Combine(Server.MapPath("/Content/images/poster_landscape/"),
                        Path.GetFileName(HinhAnh.FileName));
                    HinhAnh.SaveAs(path);
                    item.HinhAnh = "/Content/images/poster_landscape/" + Path.GetFileName(HinhAnh.FileName);
                }

                item.MoTa = phim.MoTa;
                item.TenPhim = phim.TenPhim;
                item.MaLoai = phim.MaLoai;
                item.DoiTuong = phim.DoiTuong;
                item.NgayKhoiChieu = phim.NgayKhoiChieu;
                item.Trailer = phim.Trailer;
                item.ThoiLuong = phim.ThoiLuong;
                item.NgonNgu = phim.NgonNgu;

                _dbContext.Phim.AddOrUpdate(item);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.FileStatus = "Eror update images";
            }
            return RedirectToAction("List", "MoviesManage");
        }

        public ActionResult Delete(Phim phim)
        {
            var item = _dbContext.Phim.Find(phim.MaPhim);
            if (item == null)
            {
                return HttpNotFound();
            }
            _dbContext.Phim.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction("List", "MoviesManage");
        }

        public ActionResult ChangeStatus(Phim phim)
        {
            var item = _dbContext.Phim.Find(phim.MaPhim);

            if (item.TrangThai == true)
            {
                item.TrangThai = false;
            }
            else
            {
                item.TrangThai = true;
            }    

            _dbContext.SaveChanges();

            return RedirectToAction("List", "MoviesManage");
        }
    }
}