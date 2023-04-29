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
            return View(_dbContext.Phim.OrderByDescending(o => o.TrangThai).ThenBy(t => t.NgayKhoiChieu).ToList());
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

                string fileName = Path.GetFileName(HinhAnh.FileName);
                string path = Path.Combine(Server.MapPath("/Content/images/poster_landscape/"), fileName);

                if (System.IO.File.Exists(path))
                {
                    string extension = Path.GetExtension(fileName);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    int i = 1;
                    while (System.IO.File.Exists(path))
                    {
                        fileName = fileNameWithoutExtension + "_" + i + extension;
                        path = Path.Combine(Server.MapPath("/Content/images/poster_landscape/"), fileName);
                        i++;
                    }
                }

                HinhAnh.SaveAs(path);
                phim.HinhAnh = "/Content/images/poster_landscape/" + fileName;


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
                    /*var oldImagePath = _dbContext.Phim.Where(x => x.MaPhim == phim.MaPhim).FirstOrDefault().HinhAnh;
                    string newImagePath = "/Content/images/poster_landscape/" + Path.GetFileName(HinhAnh.FileName);

                    if (System.IO.File.Exists(Server.MapPath(oldImagePath)))
                    {
                        System.IO.File.Delete(Server.MapPath(oldImagePath));
                    }

                    item.HinhAnh = newImagePath;
                    HinhAnh.SaveAs(Server.MapPath(newImagePath));*/

                    var oldImagePath = _dbContext.Phim.Where(x => x.MaPhim == phim.MaPhim).FirstOrDefault().HinhAnh;
                    string fileName = Path.GetFileName(HinhAnh.FileName);
                    string newImagePath = "/Content/images/poster_landscape/" + fileName;

                    if (System.IO.File.Exists(Server.MapPath(newImagePath)))
                    {
                        string extension = Path.GetExtension(fileName);
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        int i = 1;
                        while (System.IO.File.Exists(Server.MapPath("/Content/images/poster_landscape/" + fileNameWithoutExtension + "_" + i + extension)))
                        {
                            i++;
                        }
                        fileName = fileNameWithoutExtension + "_" + i + extension;
                        newImagePath = "/Content/images/poster_landscape/" + fileName;
                    }

                    if (System.IO.File.Exists(Server.MapPath(oldImagePath)))
                    {
                        System.IO.File.Delete(Server.MapPath(oldImagePath));
                    }

                    item.HinhAnh = newImagePath;
                    HinhAnh.SaveAs(Server.MapPath(newImagePath));

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
            var oldImagePath = _dbContext.Phim.Where(x => x.MaPhim == phim.MaPhim).FirstOrDefault().HinhAnh;

            if (item == null)
            {
                return HttpNotFound();
            }

            if (System.IO.File.Exists(Server.MapPath(oldImagePath)))
            {
                System.IO.File.Delete(Server.MapPath(oldImagePath));
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