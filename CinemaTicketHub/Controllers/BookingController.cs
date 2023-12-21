using Antlr.Runtime.Tree;
using CinemaTicketHub.API_Calling;
using CinemaTicketHub.Helper;
using CinemaTicketHub.Models;
using CinemaTicketHub.Payment;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Optimization;
using ZXing;

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
            var suatChieu = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == MaSuatChieu).FirstOrDefault();

            if (suatChieu != null)
            {
                string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{suatChieu.MaPhim}?api_key={ApiUtility.Key}&language={ApiUtility.Language}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(tmdbApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;

                        Movie movie = JsonConvert.DeserializeObject<Movie>(data);
                        ViewBag.MovieName = movie.Title;
                    }
                    else
                    {
                        ViewBag.MovieName = "Không thể lấy thông tin phim";
                    }
                }

                SelectSeatDataModel selectSeatDataModel = new SelectSeatDataModel();
                selectSeatDataModel.listcheck = lstCheckList(MaSuatChieu);
                ViewBag.moviesdetail = suatChieu;

                return View(selectSeatDataModel);
            }
            else
            {
                return HttpNotFound();
            }
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

        [HttpPost]
        public JsonResult PromotionCheck(string promocode)
        {
            var exists = _dbContext.CT_KhuyenMai.Any(x => x.MaKM == promocode.Substring(3) && x.TrangThai == true && x.KhuyenMai.ThoiHan >= DateTime.Now && x.IdKM == promocode.Substring(0, 3));

            if (exists)
            {
                TempData["Promocode"] = promocode;
                return Json(new { success = true, message = "Mã khuyến mãi đã được áp dụng." });
            }
            else
            {
                TempData["Promocode"] = null;
                return Json(new { success = false, message = "Mã khuyến mãi không hợp lệ hoặc đã hết hạn." });
            }
        }

        public ActionResult Cart(int masc)
        {
            var userId = User.Identity.GetUserId();
            List<ViKhuyenMai> khuyenMaiList = _dbContext.ViKhuyenMai.Where(x => x.id == userId).ToList();
            ViewBag.KhuyenMaiList = khuyenMaiList;

            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            if (lstGhe == null)
            {
                lstGhe = new List<Ghe>();
                Session["Cart"] = lstGhe;
            }
            ViewBag.Popcorn = _dbContext.BapNuoc.OrderBy(o => o.MaMon).ToList();
            ViewBag.moviesdetail = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == masc).FirstOrDefault();

            int? maPhim = ViewBag.moviesdetail?.MaPhim;

            if (maPhim.HasValue)
            {
                string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{maPhim}?api_key={ApiUtility.Key}&language={ApiUtility.Language}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(tmdbApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;

                        Movie movie = JsonConvert.DeserializeObject<Movie>(data);
                        ViewBag.MovieName = movie.Title;
                    }
                    else
                    {
                        ViewBag.MovieName = "Không thể lấy thông tin phim";
                    }
                }
            }

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
                TempData["Promocode"] = null;
                lstGhe.Clear();
                lstMonAn.Clear();
            }
            return RedirectToAction("Details", "Movies", new { MaPhim = maphim });
        }

        public ActionResult VNPay()
        {
            //Xử lý khuyến mãi
            string promocode = TempData["Promocode"] as string;
            int? phantramgiam;
            double? sotiengiam;
            if (promocode != null)
            {
                var promotion = _dbContext.CT_KhuyenMai.Where(x => x.MaKM == promocode.Substring(3)).FirstOrDefault();

                if (promotion.KhuyenMai.PhanTram == null)
                {
                    phantramgiam = 0;
                }
                else
                {
                    phantramgiam = promotion.KhuyenMai.PhanTram;
                }

                if (promotion.KhuyenMai.SoTienGiam == null)
                {
                    sotiengiam = 0;
                }
                else
                {
                    sotiengiam = promotion.KhuyenMai.SoTienGiam;
                }
            }
            else
            {
                phantramgiam = 0;
                sotiengiam = 0;
            }

            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            //Lấy tổng tiền trong giỏ hàng
            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            double? tienbapnuoc = lstMonAn.Sum(x => x.ThanhTien);
            double tiengiave = lstGhe.Count * 100000;
            double? total = tienbapnuoc + tiengiave;
            double? finalprice = total - sotiengiam - total / 100 * phantramgiam;

            VNPayLib pay = new VNPayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", (finalprice * 100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", VNPayUtil.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan Cinema Ticket Hub"); //Thông tin mô tả nội dung thanh toán
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
                        ViewBag.Message = "Hóa đơn: " + orderId;
                        HoaDon hoaDon = new HoaDon();
                        hoaDon.MaHoaDon = orderId.ToString();
                        hoaDon.NgayLap = DateTime.Now;
                        double Total = double.Parse(pay.GetResponseData("vnp_Amount"));
                        hoaDon.TongTien = Total / 100;
                        hoaDon.Id = User.Identity.GetUserId();
                        hoaDon.Payment = "VNPay";
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

                        //Cộng điểm tích lũy (10k = 1 điểm)
                        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        var user = userManager.FindById(User.Identity.GetUserId());
                        if (user != null)
                        {
                            user.Point += (int)Math.Floor((double)Total / 1000000);

                            userManager.Update(user);
                        }

                        // Gửi email với button "Xem chi tiết vé"
                        /*string callbackUrl = "https://localhost:44351/Manage/Ticket?mahoadon=" + orderId;
                        string emailContent = $"<br/><br/>" + $"<a href=\"{callbackUrl}\" style=\"padding: 15px 25px; font-weight: bold; font-size: 16px; text-align: center; text-transform: uppercase; transition: 0.5s; background-size: 200% auto; color: white; border-radius: 10px; display: inline-block; border: 0px; box-shadow: 0px 0px 14px -7px #f09819; background-image: linear-gradient(45deg, #FF512F 0%, #F09819 51%, #FF512F 100%); cursor: pointer; user-select: none; -webkit-user-select: none; touch-action: manipulation; text-decoration: none;\">Xem chi tiết vé</a>";
                        SendMail.SendEmail(user.Email, "Thanh toán thành công - Cinema Ticket Hub", "Vui lòng ấn vào nút bên dưới để xem chi tiết vé!" + emailContent + "", "");*/

                        // Gửi email với button "Mở app React Native Expo "
                        string callbackUrl = "http://" + NetworkUtility.GetIPv4Address() + ":8081/two?content=" + orderId;
                        string emailContent = $"<br/><br/>" + $"<a href=\"{callbackUrl}\" style=\"padding: 15px 25px; font-weight: bold; font-size: 16px; text-align: center; text-transform: uppercase; transition: 0.5s; background-size: 200% auto; color: white; border-radius: 10px; display: inline-block; border: 0px; box-shadow: 0px 0px 14px -7px #f09819; background-image: linear-gradient(45deg, #FF512F 0%, #F09819 51%, #FF512F 100%); cursor: pointer; user-select: none; -webkit-user-select: none; touch-action: manipulation; text-decoration: none;\">Mở App</a>";
                        SendMail.SendEmail(user.Email, "Thanh toán thành công - Cinema Ticket Hub", "Vui lòng ấn vào nút bên dưới để mở App!" + emailContent + "", "");


                        TempData["Promocode"] = null;
                        lstGhe.Clear();
                        lstMonAn.Clear();
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        ViewBag.VNPayStatus = "error";
                        List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                        List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
                        TempData["Promocode"] = null;
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

        public ActionResult Momo()
        {
            //Xử lý khuyến mãi
            string promocode = TempData["Promocode"] as string;
            int? phantramgiam;
            double? sotiengiam;
            if (promocode != null)
            {
                var promotion = _dbContext.CT_KhuyenMai.Where(x => x.MaKM == promocode.Substring(3)).FirstOrDefault();

                if (promotion.KhuyenMai.PhanTram == null)
                {
                    phantramgiam = 0;
                }    
                else
                {
                    phantramgiam = promotion.KhuyenMai.PhanTram;
                }
                
                if (promotion.KhuyenMai.SoTienGiam == null)
                {
                    sotiengiam = 0;
                }    
                else
                {
                    sotiengiam = promotion.KhuyenMai.SoTienGiam;
                }    
            }
            else
            {
                phantramgiam = 0;
                sotiengiam = 0;
            }

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toan Cinema Ticket Hub";
            string returnUrl = Utility.domain + "/Booking/MomoConfirmed";
            string notifyurl = Utility.domain +  "/Booking/MomoSavePayment";

            //Lấy tổng tiền trong giỏ hàng
            List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
            List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
            double? tienbapnuoc = lstMonAn.Sum(x => x.ThanhTien);
            double tiengiave = lstGhe.Count * 100000;
            double? total = tienbapnuoc + tiengiave;
            double? finalprice = total - sotiengiam - total / 100 * phantramgiam;

            string amount = finalprice.ToString();
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = MomoPaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult MomoConfirmed(Payment.MomoResult result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            ViewBag.MomoStatus = rErrorCode;
            ViewBag.Message = "Hóa đơn: " + rOrderId;

            if(rErrorCode == "0")
            {
                //Thanh toán thành công
                HoaDon hoaDon = new HoaDon();
                hoaDon.NgayLap = DateTime.Now;

                //Lấy tổng tiền trong giỏ hàng
                List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
                double? tienbapnuoc = lstMonAn.Sum(x => x.ThanhTien);
                double tiengiave = lstGhe.Count * 100000;
                double? total = tienbapnuoc + tiengiave;

                hoaDon.MaHoaDon = result.orderId;
                hoaDon.TongTien = total;
                hoaDon.Id = User.Identity.GetUserId();
                hoaDon.Payment = "Momo";
                _dbContext.HoaDon.Add(hoaDon);
                _dbContext.SaveChanges();

                foreach (var seat in lstGhe)
                {
                    Ghe ghe = _dbContext.Ghe.Where(x => x.MaGhe == seat.MaGhe).FirstOrDefault();
                    if (ghe == null)
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

                //Cộng điểm tích lũy (10k = 1 điểm)
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = userManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    user.Point += (int)Math.Floor((double)total / 10000);

                    userManager.Update(user);
                }

                // Gửi email với button "Xem chi tiết vé"
                /*string callbackUrl = "https://localhost:44351/Manage/Ticket?mahoadon=" + result.orderId;
                string emailContent = $"<br/><br/>" + $"<a href=\"{callbackUrl}\" style=\"padding: 15px 25px; font-weight: bold; font-size: 16px; text-align: center; text-transform: uppercase; transition: 0.5s; background-size: 200% auto; color: white; border-radius: 10px; display: inline-block; border: 0px; box-shadow: 0px 0px 14px -7px #f09819; background-image: linear-gradient(45deg, #FF512F 0%, #F09819 51%, #FF512F 100%); cursor: pointer; user-select: none; -webkit-user-select: none; touch-action: manipulation; text-decoration: none;\">Xem chi tiết vé</a>";
                SendMail.SendEmail(user.Email, "Thanh toán thành công - Cinema Ticket Hub", "Vui lòng ấn vào nút bên dưới để xem chi tiết vé!" + emailContent + "", "");*/

                // Gửi email với button "Mở app React Native Expo "
                string callbackUrl = "http://" + NetworkUtility.GetIPv4Address() + ":8081/two?content=" + result.orderId;
                string emailContent = $"<br/><br/>" + $"<a href=\"{callbackUrl}\" style=\"padding: 15px 25px; font-weight: bold; font-size: 16px; text-align: center; text-transform: uppercase; transition: 0.5s; background-size: 200% auto; color: white; border-radius: 10px; display: inline-block; border: 0px; box-shadow: 0px 0px 14px -7px #f09819; background-image: linear-gradient(45deg, #FF512F 0%, #F09819 51%, #FF512F 100%); cursor: pointer; user-select: none; -webkit-user-select: none; touch-action: manipulation; text-decoration: none;\">Mở App</a>";
                SendMail.SendEmail(user.Email, "Thanh toán thành công - Cinema Ticket Hub", "Vui lòng ấn vào nút bên dưới để mở App!" + emailContent + "", "");

                TempData["Promocode"] = null;
                lstGhe.Clear();
                lstMonAn.Clear();
            }
            else
            {
                //Thanh toán không thành công
                List<Ghe> lstGhe = Session["Cart"] as List<Ghe>;
                List<Cart> lstMonAn = Session["Cart2"] as List<Cart>;
                TempData["Promocode"] = null;
                lstGhe.Clear();
                lstMonAn.Clear();
            }    
            return View();
        }

        [HttpPost]
        public void MomoSavePayment()
        {
            //cập nhật dữ liệu vào db
            String a = "";
        }
    }
}