namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CT_HoaDon
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string MaHoaDon { get; set; }

        public int? SoLuong { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MaMon { get; set; }

        public double? GiaMon { get; set; }

        public virtual BapNuoc BapNuoc { get; set; }

        public virtual HoaDon HoaDon { get; set; }
    }
}
