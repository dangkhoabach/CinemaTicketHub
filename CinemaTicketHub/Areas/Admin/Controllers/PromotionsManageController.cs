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

        // Hàm tạo chuỗi ngẫu nhiên
        private string Generatepromotionscode(int length)
        {
            /*const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";*/
            const string chars = "0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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
                _dbContext.KhuyenMai.Add(khuyenmai);
                _dbContext.SaveChanges();

                for (int i = 1; i <= khuyenmai.SoLuong; i++)
                {
                    CT_KhuyenMai ctKhuyenMai = new CT_KhuyenMai();
                    ctKhuyenMai.IdKM = khuyenmai.IdKM;
                    ctKhuyenMai.TrangThai = true;

                    // Tạo chuỗi ngẫu nhiên với độ dài 10 ký tự
                    string promotionscode = Generatepromotionscode(10);
                    ctKhuyenMai.MaKM = promotionscode;

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