using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Models
{
    public class TicketViewModel
    {
        public string mahoadon { get; set; }
        public string tenphim { get; set; }
        public string hinhanh { get; set; }
        public string phongchieu { get; set; }
        public TimeSpan? giobatdau { get; set; }
        public TimeSpan? gioketthuc { get; set; }
        public DateTime? ngaychieu { get; set; }
        public double? tongtien { get; set; }
        public string payment { get; set; }
        public List<BapNuocViewModel> bapNuoc { get; set; }
        public List<GheViewModel> ghe { get; set; }
        public TicketViewModel() { 

        }
    }
}