namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ghe")]
    public partial class Ghe
    {
        [Key]
        public int MaGhe { get; set; }

        [StringLength(5)]
        public string Day { get; set; }

        public int? Cot { get; set; }

        public int? MaSuatChieu { get; set; }

        public bool? TrangThai { get; set; }

        public virtual SuatChieu SuatChieu { get; set; }
    }
}
