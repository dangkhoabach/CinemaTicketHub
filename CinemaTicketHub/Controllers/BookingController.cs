﻿using Antlr.Runtime.Tree;
using CinemaTicketHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaTicketHub.Controllers
{
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
            return RedirectToAction("Cart", "Booking", new { masc = MaSC});
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

            ViewBag.moviesdetail = _dbContext.SuatChieu.Where(y => y.MaSuatChieu == masc).FirstOrDefault();
            return View(lstGhe);
        }
    }
}