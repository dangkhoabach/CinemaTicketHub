using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CinemaTicketHub.Models
{
    public partial class CinemaTicketHubModel : DbContext
    {
        public CinemaTicketHubModel()
            : base("name=CinemaTicketHubModel")
        {
        }

        public virtual DbSet<KhachHang> KhachHang { get; set; }
        public virtual DbSet<LoaiPhim> LoaiPhim { get; set; }
        public virtual DbSet<Phim> Phim { get; set; }
        public virtual DbSet<PhongChieu> PhongChieu { get; set; }
        public virtual DbSet<Ve> Ve { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KhachHang>()
                .HasMany(e => e.Ve)
                .WithRequired(e => e.KhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiPhim>()
                .HasMany(e => e.Phim)
                .WithRequired(e => e.LoaiPhim)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Phim>()
                .HasMany(e => e.Ve)
                .WithRequired(e => e.Phim)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhongChieu>()
                .HasMany(e => e.Ve)
                .WithRequired(e => e.PhongChieu)
                .WillCascadeOnDelete(false);
        }
    }
}
