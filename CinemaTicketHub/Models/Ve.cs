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

        public int MaGhe { get; set; }

        public DateTime NgayBanVe { get; set; }

        [Required]
        [StringLength(128)]
        public string Id { get; set; }

        public int MaSuatChieu { get; set; }

        public double? GiaVe { get; set; }

        public virtual SuatChieu SuatChieu { get; set; }
    }
}
