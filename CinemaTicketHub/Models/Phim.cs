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
            Ve = new HashSet<Ve>();
        }

        [Key]
        [StringLength(50)]
        public string MaPhim { get; set; }

        [Required]
        public string TenPhim { get; set; }

        public DateTime NgayKhoiChieu { get; set; }

        public int MaLoai { get; set; }

        [StringLength(50)]
        public string DoiTuong { get; set; }

        public string HinhAnh { get; set; }

        public string MoTa { get; set; }

        public virtual LoaiPhim LoaiPhim { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ve> Ve { get; set; }
    }
}
