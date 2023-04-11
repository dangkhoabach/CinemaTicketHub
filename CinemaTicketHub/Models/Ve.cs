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

        [Required]
        [StringLength(50)]
        public string Ghe { get; set; }

        public DateTime NgayBanVe { get; set; }

        public int MaKH { get; set; }

        [Required]
        [StringLength(50)]
        public string MaPhim { get; set; }

        public int MaPhong { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        public virtual Phim Phim { get; set; }

        public virtual PhongChieu PhongChieu { get; set; }
    }
}
