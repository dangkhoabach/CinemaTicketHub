using CinemaTicketHub.API_Calling;
using CinemaTicketHub.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CinemaTicketHub.Areas.Admin.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TicketAPIController : ApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public TicketAPIController()
        {
            _dbContext = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("api/TicketAPI/GetByID/{id}")]
        public IHttpActionResult GetByID(string id)
        {
            try
            {
                var hoadon = _dbContext.HoaDon.FirstOrDefault(x => x.MaHoaDon == id);

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
                            string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{suatchieu.MaPhim}?api_key={APIKey.Key}&language={APIKey.Language}";

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

                    return Ok(ticket);

                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
