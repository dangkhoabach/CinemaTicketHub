namespace CinemaTicketHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BapNuoc")]
    public partial class BapNuoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BapNuoc()
        {
            CT_HoaDon = new HashSet<CT_HoaDon>();
        }

        [Key]
        public int MaMon { get; set; }

        [StringLength(128)]
        public string TenMon { get; set; }

        public double? GiaMon { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public string HinhAnhMon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CT_HoaDon> CT_HoaDon { get; set; }
    }
}
