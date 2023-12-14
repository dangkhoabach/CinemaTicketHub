using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CinemaTicketHub.Models;
using System.Collections.Generic;
using System.Drawing;
using ZXing;
using Microsoft.AspNet.Identity.EntityFramework;
using CinemaTicketHub.API_Calling;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using ImageResizer;

namespace CinemaTicketHub.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext _dbContext = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return View("ChangePasswordSuccess");
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion

        public ActionResult UserProfile()
        {
            var userId = User.Identity.GetUserId();
            var hoadon = _dbContext.HoaDon.Where(x => x.Id == userId).ToList();
            List<TicketViewModel> list = new List<TicketViewModel>();
            foreach (var item in hoadon)
            {
                List<GheViewModel> lstghe = new List<GheViewModel>();
                List<BapNuocViewModel> lstbapnuoc = new List<BapNuocViewModel>();
                TicketViewModel ticket = new TicketViewModel();
                ticket.mahoadon = item.MaHoaDon;
                ticket.tongtien = item.TongTien;
                ticket.payment = item.Payment;

                var ve = _dbContext.Ve.Where(x => x.MaHoaDon == item.MaHoaDon).ToList();
                foreach (var ve2 in ve)
                {
                    var suatchieu = _dbContext.SuatChieu.Where(x => x.MaSuatChieu == ve2.MaSuatChieu).FirstOrDefault();

                    if (suatchieu != null)
                    {
                        string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{suatchieu.MaPhim}?api_key={ApiUtility.Key}&language={ApiUtility.Language}";

                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = client.GetAsync(tmdbApiUrl).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                string data = response.Content.ReadAsStringAsync().Result;

                                Movie movie = JsonConvert.DeserializeObject<Movie>(data);

                                ticket.giobatdau = suatchieu.GioBatDau;
                                ticket.ngaychieu = suatchieu.NgayChieu;
                                ticket.tenphim = movie.Title;
                                ticket.phongchieu = suatchieu.PhongChieu.TenPhong;

                                var ghes = _dbContext.Ghe.Where(x => x.MaSuatChieu == ve2.MaSuatChieu && x.MaGhe == ve2.MaGhe).FirstOrDefault();

                                if (ghes != null)
                                {
                                    GheViewModel ghe = new GheViewModel
                                    {
                                        maghe = ghes.MaGhe,
                                        day = ghes.Day,
                                        cot = ghes.Cot
                                    };

                                    lstghe.Add(ghe);
                                }
                            }
                        }
                    }
                }
                var ct = _dbContext.CT_HoaDon.Where(x => x.MaHoaDon == item.MaHoaDon).ToList();
                foreach (var ct2 in ct)
                {
                    BapNuocViewModel bapNuoc = new BapNuocViewModel();
                    bapNuoc.tenmon = ct2.BapNuoc.TenMon;
                    bapNuoc.soluongmon = ct2.SoLuong;
                    lstbapnuoc.Add(bapNuoc);
                }
                ticket.bapNuoc = lstbapnuoc;
                ticket.ghe = lstghe;
                list.Add(ticket);
            }

            list.Sort((x, y) => x.mahoadon.CompareTo(y.mahoadon));
            list.Reverse();
            ViewBag.hoadon = list;
            return View();
        }

        public ActionResult Ticket(string mahoadon)
        {
            var hoadon = _dbContext.HoaDon.FirstOrDefault(x => x.MaHoaDon == mahoadon);

            if (hoadon != null)
            {
                List<GheViewModel> lstghe = new List<GheViewModel>();
                List<BapNuocViewModel> lstbapnuoc = new List<BapNuocViewModel>();
                TicketViewModel ticket = new TicketViewModel();
                ticket.mahoadon = hoadon.MaHoaDon;
                ticket.tongtien = hoadon.TongTien;
                ticket.payment = hoadon.Payment;

                var ve = _dbContext.Ve.Where(x => x.MaHoaDon == hoadon.MaHoaDon).ToList();
                foreach (var ve2 in ve)
                {
                    var suatchieu = _dbContext.SuatChieu.FirstOrDefault(x => x.MaSuatChieu == ve2.MaSuatChieu);
                    if (suatchieu != null)
                    {
                        string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{suatchieu.MaPhim}?api_key={ApiUtility.Key}&language={ApiUtility.Language}";

                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = client.GetAsync(tmdbApiUrl).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                string data = response.Content.ReadAsStringAsync().Result;

                                dynamic movieData = JsonConvert.DeserializeObject(data);

                                ticket.giobatdau = suatchieu.GioBatDau;
                                ticket.gioketthuc = suatchieu.GioKetThuc;
                                ticket.ngaychieu = suatchieu.NgayChieu;
                                ticket.tenphim = movieData.original_title;
                                ticket.hinhanh = "https://image.tmdb.org/t/p/w500" + movieData.poster_path;
                                ticket.phongchieu = suatchieu.PhongChieu.TenPhong;

                                var ghes = _dbContext.Ghe.FirstOrDefault(x => x.MaSuatChieu == ve2.MaSuatChieu && x.MaGhe == ve2.MaGhe);
                                if (ghes != null)
                                {
                                    GheViewModel ghe = new GheViewModel();
                                    ghe.maghe = ghes.MaGhe;
                                    ghe.day = ghes.Day;
                                    ghe.cot = ghes.Cot;
                                    lstghe.Add(ghe);
                                }
                            }
                        }
                    }
                }

                var ct = _dbContext.CT_HoaDon.Where(x => x.MaHoaDon == hoadon.MaHoaDon).ToList();
                foreach (var ct2 in ct)
                {
                    BapNuocViewModel bapNuoc = new BapNuocViewModel();
                    bapNuoc.tenmon = ct2.BapNuoc.TenMon;
                    bapNuoc.soluongmon = ct2.SoLuong;
                    lstbapnuoc.Add(bapNuoc);
                }

                ticket.bapNuoc = lstbapnuoc;
                ticket.ghe = lstghe;

                ViewBag.hoadon = ticket;
            }
            else
            {
                ViewBag.hoadon = null;
            }

            return View();
        }

        public ActionResult GenerateQRCode(string content)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            // Cấu hình các thông số cho QR code (ví dụ: margin, width, height)
            ZXing.Common.EncodingOptions options = new ZXing.Common.EncodingOptions();
            options.Margin = 1; // Đặt margin thành 0 để giảm viền trắng
            options.Width = 200; // Đặt chiều rộng của QR code
            options.Height = 200; // Đặt chiều cao của QR code

            barcodeWriter.Options = options;

            Bitmap barcodeBitmap = barcodeWriter.Write(content);

            // Chuyển đổi Bitmap thành byte array để hiển thị trong trang Razor CSHTML
            ImageConverter converter = new ImageConverter();
            byte[] imageBytes = (byte[])converter.ConvertTo(barcodeBitmap, typeof(byte[]));

            // Trả về hình ảnh dưới dạng File hình ảnh
            return File(imageBytes, "image/png");
        }

        public ActionResult PromotionsWallet(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Error = error;
            }

            ViewBag.PromoWallet = _dbContext.ViKhuyenMai.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult SavePromotion(ViKhuyenMai vikhuyenmai)
        {
            var userId = User.Identity.GetUserId();

            try
            {
                var isMaKmValid = _dbContext.CT_KhuyenMai.Any(km => km.MaKM == vikhuyenmai.MaKM.Substring(3));

                if (!isMaKmValid)
                {
                    return RedirectToAction("PromotionsWallet", "Manage", new { error = "Mã khuyến mãi không hợp lệ!" });

                }
                else
                {
                    string idkm = vikhuyenmai.MaKM.Substring(0, 3);
                    var khuyenMai = _dbContext.KhuyenMai.FirstOrDefault(km => km.IdKM.StartsWith(idkm));

                    if (khuyenMai != null && DateTime.Today <= khuyenMai.ThoiHan)
                    {
                        vikhuyenmai.IdKM = vikhuyenmai.MaKM.Substring(0, 3);
                        vikhuyenmai.MaKM = vikhuyenmai.MaKM.Substring(3);
                        vikhuyenmai.id = userId;
                        _dbContext.ViKhuyenMai.Add(vikhuyenmai);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("PromotionsWallet", "Manage", new { error = "Mã khuyến mãi đã hết hạn sử dụng!" });
                    }
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("PromotionsWallet", "Manage");
        }

        public ActionResult DeletePromotion(ViKhuyenMai vikhuyenmai)
        {
            var item = _dbContext.ViKhuyenMai.SingleOrDefault(x => x.MaKM == vikhuyenmai.MaKM);

            if (item == null)
            {
                return HttpNotFound();
            }

            _dbContext.ViKhuyenMai.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction("PromotionsWallet", "Manage");
        }

        public ActionResult ExchangeRewards()
        {
            ViewBag.Reward = _dbContext.PhanThuong.ToList();
            return View();
        }

        public ActionResult Exchange(string idkm)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = userManager.FindById(User.Identity.GetUserId());

            var phanthuong = _dbContext.PhanThuong.FirstOrDefault(k => k.IdKM == idkm);

            if (user != null && user.Point >= phanthuong.Diem)
            {
                var maKM = _dbContext.CT_KhuyenMai.Where(ct => ct.IdKM == idkm && ct.TrangThai == true).Select(ct => ct.MaKM).FirstOrDefault();

                if (!string.IsNullOrEmpty(maKM))
                {
                    ViKhuyenMai viKhuyenMai = new ViKhuyenMai();
                    viKhuyenMai.id = user.Id;
                    viKhuyenMai.MaKM = maKM;
                    viKhuyenMai.IdKM = idkm;

                    _dbContext.ViKhuyenMai.Add(viKhuyenMai);
                    _dbContext.SaveChanges();

                    user.Point = user.Point - (int)phanthuong.Diem;
                    userManager.Update(user);
                }
            }
            return RedirectToAction("PromotionsWallet", "Manage");
        }

        [HttpPost]
        public ActionResult ChangeAvatar(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var user = userManager.FindById(User.Identity.GetUserId());

                    string uploadPath = Server.MapPath("~/Content/images/authenticate/useravatar/");
                    string fileName = Path.GetFileName(file.FileName);
                    string fullPath = Path.Combine(uploadPath, fileName);
                    file.SaveAs(fullPath);

                    string oldAvatarPath = Server.MapPath(user.Avatar);
                    if (oldAvatarPath != Server.MapPath("/Content/images/authenticate/blankavatar.jpg"))
                    {
                        System.IO.File.Delete(oldAvatarPath);
                    }

                    user.Avatar = "/Content/images/authenticate/useravatar/square_" + fileName;
                    userManager.Update(user);

                    string squareImagePath = Server.MapPath("~/Content/images/authenticate/useravatar/square_" + fileName);
                    ImageBuilder.Current.Build(new ImageJob(fullPath, squareImagePath, new Instructions("format=jpg&width=200&height=200&mode=crop")));

                    System.IO.File.Delete(fullPath);

                    return RedirectToAction("UserProfile", "Manage");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Lỗi: " + ex.Message;
                }
            }

            return View();
        }
    }
}