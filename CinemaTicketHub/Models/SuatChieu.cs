namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SuatChieu")]
    public partial class SuatChieu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SuatChieu()
        {
            Ve = new HashSet<Ve>();
        }

        [Key]
        public int MaSuatChieu { get; set; }

        public TimeSpan? GioBatDau { get; set; }

        public TimeSpan? GioKetThuc { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayChieu { get; set; }

        [Required]
        [StringLength(10)]
        public string MaPhim { get; set; }

        public int MaPhong { get; set; }

        public virtual Phim Phim { get; set; }

        public virtual PhongChieu PhongChieu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ve> Ve { get; set; }
    }
}
