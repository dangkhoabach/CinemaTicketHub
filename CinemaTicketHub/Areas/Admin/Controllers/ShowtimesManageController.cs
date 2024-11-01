﻿using CinemaTicketHub.API_Calling;
using CinemaTicketHub.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            var suatChieuList = _dbContext.SuatChieu.OrderByDescending(o => o.NgayChieu).ThenBy(t => t.GioBatDau).ToList();

            // Create a list to store movie names
            List<string> movieNames = new List<string>();

            foreach (var suatChieu in suatChieuList)
            {
                string tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{suatChieu.MaPhim}?api_key={ApiUtility.Key}&language={ApiUtility.Language}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(tmdbApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string data = response.Content.ReadAsStringAsync().Result;

                        Movie movie = JsonConvert.DeserializeObject<Movie>(data);
                        movieNames.Add(movie.Title); // Add movie name to the list
                    }
                    else
                    {
                        movieNames.Add("Không thể lấy thông tin phim"); // Add a placeholder if there's an error
                    }
                }
            }

            // Pass both suatChieuList and movieNames to the view
            ViewBag.MovieNames = movieNames;
            return View(suatChieuList);
        }


        public async Task<ActionResult> Create()
        {
            string apiUrl = $"https://api.themoviedb.org/3/movie/now_playing?api_key={ApiUtility.Key}&language={ApiUtility.Language}&region={ApiUtility.Region}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(jsonResult);

                    List<Phim> phimList = new List<Phim>();
                    foreach (var phim in data.results)
                    {
                        Phim phimItem = new Phim
                        {
                            MaPhim = phim.id,
                            TenPhim = phim.title,
                        };
                        phimList.Add(phimItem);
                    }


                    ViewBag.Phim = phimList;
                }
                else
                {
                    ViewBag.Error = "Không thể lấy danh sách phim từ API.";
                }
            }
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
                    ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();
                    return View("Create", suatchieu);
                }
                _dbContext.SuatChieu.Add(suatchieu);

                //Thêm ghế theo suất chiếu
                for (int i = 1; i <= 5; i++)
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        Ghe ghe = new Ghe
                        {
                            Cot = j,
                            Day = ((char)('A' + i - 1)).ToString(),
                            MaSuatChieu = suatchieu.MaSuatChieu,
                            TrangThai = false
                        };

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

        public async Task<ActionResult> Details(int MaSuatChieu)
        {
            SuatChieu suatchieu = _dbContext.SuatChieu.Find(MaSuatChieu);

            string apiUrl = $"https://api.themoviedb.org/3/movie/now_playing?api_key={ApiUtility.Key}&language={ApiUtility.Language}&region={ApiUtility.Region}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(jsonResult);

                    List<Phim> phimList = new List<Phim>();
                    foreach (var phim in data.results)
                    {
                        Phim phimItem = new Phim
                        {
                            MaPhim = phim.id,
                            TenPhim = phim.title,
                        };
                        phimList.Add(phimItem);
                    }
                    ViewBag.Phim = phimList;
                }
                else
                {
                    ViewBag.Error = "Không thể lấy danh sách phim từ API.";
                }
            }
            ViewBag.PhongChieu = _dbContext.PhongChieu.ToList();
            return View(suatchieu);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SuatChieu suatchieu)
        {
            var item = _dbContext.SuatChieu.Find(suatchieu.MaSuatChieu);
            try
            {
                if (item == null)
                {
                    return HttpNotFound();
                }

                string apiUrl = $"https://api.themoviedb.org/3/movie/now_playing?api_key={ApiUtility.Key}&language={ApiUtility.Language}&region={ApiUtility.Region}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(jsonResult);

                        List<Phim> phimList = new List<Phim>();
                        foreach (var phim in data.results)
                        {
                            Phim phimItem = new Phim
                            {
                                MaPhim = phim.id,
                                TenPhim = phim.title,
                            };
                            phimList.Add(phimItem);
                        }
                        ViewBag.Phim = phimList;
                    }
                    else
                    {
                        ViewBag.Error = "Không thể lấy danh sách phim từ API.";
                    }
                }
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

/*            var ve = _dbContext.Ve.Where(x => x.MaSuatChieu == suatchieu.MaSuatChieu).ToList();
            foreach (var itemve in ve)
            {
                itemve.MaGhe = null;
            }*/

            _dbContext.SuatChieu.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction("List", "ShowtimesManage");
        }
    }
}