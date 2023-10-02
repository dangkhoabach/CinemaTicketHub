﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Models
{
    public class TicketViewModel
    {
        //mã hóa đơn
        public int Id { get; set; }
        public string tenphim { get; set; }
        public string phongchieu { get; set; }
        public TimeSpan? giobatdau { get; set; }
        public DateTime? ngaychieu { get; set; }
        public double? tongtien { get; set; }
        public string payment { get; set; }
        public List<BapNuocViewModel> bapNuoc { get; set; }
        public List<GheViewModel> ghe { get; set; }
        public TicketViewModel() { 

        }
    }
}