namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KhuyenMai")]
    public partial class KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuyenMai()
        {
            CT_KhuyenMai = new HashSet<CT_KhuyenMai>();
            PhanThuong = new HashSet<PhanThuong>();
            ViKhuyenMai = new HashSet<ViKhuyenMai>();
        }

        [Key]
        [StringLength(3)]
        public string IdKM { get; set; }

        [StringLength(255)]
        public string TenKM { get; set; }

        public int? SoLuong { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ThoiHan { get; set; }

        public int? PhanTram { get; set; }

        public double? SoTienGiam { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_KhuyenMai> CT_KhuyenMai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhanThuong> PhanThuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ViKhuyenMai> ViKhuyenMai { get; set; }
    }
}
