using Antlr.Runtime.Tree;
using CinemaTicketHub.Models;
using CinemaTicketHub.Payment;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace CinemaTicketHub.Controllers
{
    [Authorize]
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
            ViewBag.moviesdetail = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == MaSuatChieu).FirstOrDefault();
            SelectSeatDataModel selectSeatDataModel = new SelectSeatDataModel();
            selectSeatDataModel.listcheck = lstCheckList(MaSuatChieu);
            return View(selectSeatDataModel);
        }

        [HttpPost]
        public ActionResult SeatSelection(SelectSeatDataModel selectSeatDataModel, int MaSC)
        {
            foreach (var item in selectSeatDataModel.listcheck)
            {
                if (item.Selected == true)
                {
                    var seatselected = _dbContext.Ghe.Where(p => p.MaSuatChieu == item.MaSuatChieu && p.MaGhe == item.MaGhe).FirstOrDefault();
                    List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                    if (lstGhe == null)
                    {
                        lstGhe = new List<Ghe>();
                        Session["Cart"] = lstGhe;
                    }
                    lstGhe.Add(seatselected);
                }
            }
            return RedirectToAction("Cart", "Booking", new { masc = MaSC });
        }

        public List<CheckListModel> lstCheckList(int MaSuatChieu)
        {
            var seat = _dbContext.Ghe.Where(p => p.MaSuatChieu == MaSuatChieu).OrderBy(o => o.Day).ThenBy(t => t.Cot).ToList();
            List<CheckListModel> lst = seat.Select((s, index) => new CheckListModel
            {
                SeatId = index + 1,
                Name = s.Day + s.Cot,
                Status = s.TrangThai,
                MaGhe = s.MaGhe,
                MaSuatChieu = s.MaSuatChieu
            }).ToList();
            return lst;
        }

        public ActionResult Cart(int masc)
        {
            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            if (lstGhe == null)
            {
                lstGhe = new List<Ghe>();
                Session["Cart"] = lstGhe;
            }
            ViewBag.Popcorn = _dbContext.BapNuoc.OrderBy(o => o.MaMon).ToList();
            ViewBag.moviesdetail = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == masc).FirstOrDefault();

            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            if (lstMonAn == null)
            {
                lstMonAn = new List<Cart>();
                Session["Cart2"] = lstMonAn;
            }
            ViewBag.CartMonAn = lstMonAn;
            double? tienbapnuoc = lstMonAn.Sum(x => x.ThanhTien);
            double tiengiave = lstGhe.Count * 100000;
            ViewBag.TongTien = tienbapnuoc + tiengiave;
            return View(lstGhe);
        }

        public ActionResult AddToCart(int ID, string strURL)
        {
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            Cart foodnDrinkCart = lstMonAn.Find(n => n.MaMon == ID);
            if (foodnDrinkCart == null)
            {
                foodnDrinkCart = new Cart(ID);
                lstMonAn.Add(foodnDrinkCart);
                return Redirect(strURL);
            }
            else
            {
                foodnDrinkCart.SoLuong++;
                return Redirect(strURL);
            }
        }

        public ActionResult UpdateCart(int ID, string strURL)
        {
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            Cart foodnDrinkCart = lstMonAn.Find(n => n.MaMon == ID);
            if (foodnDrinkCart.SoLuong == 1)
            {
                lstMonAn.Remove(foodnDrinkCart);
            }
            else
            {
                foodnDrinkCart.SoLuong--;
            }
            return Redirect(strURL);
        }

        public ActionResult Reselect(string maphim)
        {
            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            if (lstGhe != null || lstMonAn != null)
            {
                lstGhe.Clear();
                lstMonAn.Clear();
            }
            return RedirectToAction("Details", "Movies", new { MaPhim = maphim });
        }

        public ActionResult VNPay()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            double? tienbapnuoc = lstMonAn.Sum(x => x.ThanhTien);
            double tiengiave = lstGhe.Count * 100000;
            double? total = tienbapnuoc + tiengiave;

            VNPayLib pay = new VNPayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (total * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", VNPayUtil.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult VNPayConfirmed()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                VNPayLib pay = new VNPayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;

                        HoaDon hoaDon = new HoaDon();
                        hoaDon.NgayLap = DateTime.Now;
                        double Total = double.Parse(pay.GetResponseData("vnp_Amount"));
                        hoaDon.TongTien = Total / 100;
                        hoaDon.Id = User.Identity.GetUserId();
                        _dbContext.HoaDon.Add(hoaDon);
                        _dbContext.SaveChanges();

                        List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                        List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;

                        foreach (var seat in lstGhe)
                        {
                            Ghe ghe = _dbContext.Ghe.Where(x => x.MaGhe == seat.MaGhe).FirstOrDefault();
                            if(ghe == null)
                            {
                                return HttpNotFound();
                            }    
                            ghe.TrangThai = true;

                            Ve ve = new Ve();
                            ve.MaGhe = seat.MaGhe;
                            ve.MaSuatChieu = seat.MaSuatChieu ?? 0;
                            ve.GiaVe = 100000;
                            ve.MaHoaDon = hoaDon.MaHoaDon;
                            _dbContext.Ve.Add(ve);

                            _dbContext.SaveChanges();
                        }

                        foreach (var mon in lstMonAn)
                        {
                            CT_HoaDon ctHoaDon = new CT_HoaDon();
                            ctHoaDon.MaHoaDon = hoaDon.MaHoaDon;
                            ctHoaDon.MaMon = mon.MaMon;
                            ctHoaDon.SoLuong = mon.SoLuong;
                            ctHoaDon.GiaMon = mon.GiaMon;
                            _dbContext.CT_HoaDon.Add(ctHoaDon);
                            _dbContext.SaveChanges();
                        }

                        lstGhe.Clear();
                        lstMonAn.Clear();
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                        List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
                        lstGhe.Clear();
                        lstMonAn.Clear();
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }
    }
}