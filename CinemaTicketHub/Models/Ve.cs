namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ve")]
    public partial class Ve
    {
        [Key]
        public int MaVe { get; set; }

        public int? MaGhe { get; set; }

        public int? MaSuatChieu { get; set; }

        public double? GiaVe { get; set; }

        [StringLength(255)]
        public string MaHoaDon { get; set; }

        public virtual HoaDon HoaDon { get; set; }

        public virtual SuatChieu SuatChieu { get; set; }
    }
}
