using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Master")]
    public class PromotionsManageController : Controller
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        // GET: Admin/PromotionsManage
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PromotionGenerate(KhuyenMai khuyenmai)
        {
            try
            {
                if (khuyenmai.SoTienGiam == null)
                {
                    khuyenmai.SoTienGiam = 0;
                }
                else if (khuyenmai.PhanTram == null)
                {
                    khuyenmai.PhanTram = 0;
                }

                _dbContext.KhuyenMai.Add(khuyenmai);
                _dbContext.SaveChanges();

                Random random = new Random();
                for (int i = 1; i <= khuyenmai.SoLuong; i++)
                {
                    CT_KhuyenMai ctKhuyenMai = new CT_KhuyenMai();
                    ctKhuyenMai.IdKM = khuyenmai.IdKM;
                    ctKhuyenMai.TrangThai = true;

                    // Tạo chuỗi ngẫu nhiên gồm 10 số
                    string randomString = random.Next(1000000000, 2000000000).ToString();
                    ctKhuyenMai.MaKM = randomString;

                    _dbContext.CT_KhuyenMai.Add(ctKhuyenMai);
                    _dbContext.SaveChanges();
                }

            }
            catch (Exception)
            {

            }
            return RedirectToAction("Generate", "PromotionsManage");
        }
    }
}