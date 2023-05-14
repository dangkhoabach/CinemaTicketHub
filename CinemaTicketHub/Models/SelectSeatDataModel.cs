using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Models
{
    public class SelectSeatDataModel
    {
        public int SeatId { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public bool Selected { get; set; }
        public int MaGhe { get; set; }
        public int? MaSuatChieu { get; set; }

        public List<CheckListModel> listcheck { get; set; }
    }

    public class CheckListModel
    {
        public int SeatId { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public bool Selected { get; set; }
        public int MaGhe { get; set; }
        public int? MaSuatChieu { get; set; }
    }
}