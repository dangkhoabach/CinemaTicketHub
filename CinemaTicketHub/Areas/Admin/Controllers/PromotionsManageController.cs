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
            ViewBag.Promotions = _dbContext.KhuyenMai.ToList();
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

                //Nếu ID RE thì thêm vào phần thưởng
/*                if (khuyenmai.IdKM.StartsWith("RE"))
                {
                    PhanThuong phanThuong = new PhanThuong();
                    phanThuong.IdKM = khuyenmai.IdKM;
                    phanThuong.Diem = 0;
                    phanThuong.HinhAnhQua = "";

                    _dbContext.PhanThuong.Add(phanThuong);
                    _dbContext.SaveChanges();
                }*/
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Generate", "PromotionsManage");
        }

        public ActionResult DeletePromotion(KhuyenMai khuyenMai)
        {
            var promo = _dbContext.KhuyenMai.Find(khuyenMai.IdKM);
            if (promo == null)
            {
                return HttpNotFound();
            }
            var promodetail = _dbContext.CT_KhuyenMai.Where(x => x.IdKM == khuyenMai.IdKM).ToList();
            foreach (var item in promodetail)
            {
                _dbContext.CT_KhuyenMai.Remove(item);
            }

            _dbContext.KhuyenMai.Remove(promo);
            _dbContext.SaveChanges();
            return RedirectToAction("Generate", "PromotionsManage");
        }

        public ActionResult Detail(string IdKM)
        {
            ViewBag.PromotionName = _dbContext.KhuyenMai.Where(x => x.IdKM == IdKM).FirstOrDefault();
            ViewBag.PromotionDetail = _dbContext.CT_KhuyenMai.Where(x => x.IdKM == IdKM).ToList();
            return View();
        }

        public ActionResult ChangeStatus(string IdKM, string MaKM)
        {
            var prostatus = _dbContext.CT_KhuyenMai.Where(x => x.IdKM == IdKM && x.MaKM == MaKM).FirstOrDefault();
            if (prostatus != null)
            {
                if(prostatus.TrangThai == true)
                {
                    prostatus.TrangThai = false;
                }
                else
                {
                    prostatus.TrangThai = true;
                }
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Detail", "PromotionsManage", new { IdKM = IdKM });
        }
    }
}