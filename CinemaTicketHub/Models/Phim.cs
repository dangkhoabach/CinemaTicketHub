namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Phim")]
    public partial class Phim
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Phim()
        {
            SuatChieu = new HashSet<SuatChieu>();
        }

        [Key]
        [StringLength(10)]
        public string MaPhim { get; set; }

        [Required]
        [StringLength(128)]
        public string TenPhim { get; set; }

        [Column(TypeName = "date")]
        public DateTime NgayKhoiChieu { get; set; }

        public int MaLoai { get; set; }

        [StringLength(50)]
        public string DoiTuong { get; set; }

        public string HinhAnh { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public string Trailer { get; set; }

        public bool? TrangThai { get; set; }

        public int? ThoiLuong { get; set; }

        [StringLength(50)]
        public string NgonNgu { get; set; }

        public virtual LoaiPhim LoaiPhim { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuatChieu> SuatChieu { get; set; }
    }
}
