using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Models
{
    public class FoodnDrinkCart
    {
        ApplicationDbContext _dbContext = new ApplicationDbContext();
        public int MaMon { get; set; }

        public string TenMon { get; set; }

        public double? GiaMon { get; set; }

        public string MoTa { get; set; }
        public int SoLuong { get; set; }


        public FoodnDrinkCart(int maMon)
        {
            MaMon = maMon;
            BapNuoc bapNuoc = _dbContext.BapNuoc.Single(n => n.MaMon == maMon);
            TenMon = bapNuoc.TenMon;
            GiaMon = bapNuoc.GiaMon;
            MoTa = bapNuoc.MoTa;
            SoLuong = 1;
        }
    }
}