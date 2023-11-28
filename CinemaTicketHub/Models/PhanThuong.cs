namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhanThuong")]
    public partial class PhanThuong
    {
        [Key]
        [Column(Order = 0)]
        public int MaPT { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string IdKM { get; set; }

        public int? Diem { get; set; }

        public string HinhAnhQua { get; set; }

        public virtual KhuyenMai KhuyenMai { get; set; }
    }
}
