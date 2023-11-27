namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViKhuyenMai")]
    public partial class ViKhuyenMai
    {
        [Key]
        [Column(Order = 0)]
        public string id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string MaKM { get; set; }

        [StringLength(3)]
        public string IdKM { get; set; }

        public virtual CT_KhuyenMai CT_KhuyenMai { get; set; }

        public virtual KhuyenMai KhuyenMai { get; set; }
    }
}
