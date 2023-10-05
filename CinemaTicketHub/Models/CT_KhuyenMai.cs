namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CT_KhuyenMai
    {
        [Required]
        [StringLength(5)]
        public string IdKM { get; set; }

        [Key]
        [StringLength(10)]
        public string MaKM { get; set; }

        public bool? TrangThai { get; set; }

        public virtual KhuyenMai KhuyenMai { get; set; }
    }
}
