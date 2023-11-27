namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CT_KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CT_KhuyenMai()
        {
            ViKhuyenMai = new HashSet<ViKhuyenMai>();
        }

        [Required]
        [StringLength(3)]
        public string IdKM { get; set; }

        [Key]
        [StringLength(10)]
        public string MaKM { get; set; }

        public bool? TrangThai { get; set; }

        public virtual KhuyenMai KhuyenMai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ViKhuyenMai> ViKhuyenMai { get; set; }
    }
}
