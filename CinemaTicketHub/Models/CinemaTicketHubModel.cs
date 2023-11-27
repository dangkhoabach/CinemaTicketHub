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

        public virtual DbSet<BapNuoc> BapNuoc { get; set; }
        public virtual DbSet<CT_HoaDon> CT_HoaDon { get; set; }
        public virtual DbSet<CT_KhuyenMai> CT_KhuyenMai { get; set; }
        public virtual DbSet<Ghe> Ghe { get; set; }
        public virtual DbSet<HoaDon> HoaDon { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMai { get; set; }
        public virtual DbSet<LoaiPhim> LoaiPhim { get; set; }
        public virtual DbSet<Phim> Phim { get; set; }
        public virtual DbSet<PhongChieu> PhongChieu { get; set; }
        public virtual DbSet<SuatChieu> SuatChieu { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Ve> Ve { get; set; }
        public virtual DbSet<ViKhuyenMai> ViKhuyenMai { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BapNuoc>()
                .HasMany(e => e.CT_HoaDon)
                .WithRequired(e => e.BapNuoc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CT_KhuyenMai>()
                .HasMany(e => e.ViKhuyenMai)
                .WithRequired(e => e.CT_KhuyenMai)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HoaDon>()
                .HasMany(e => e.CT_HoaDon)
                .WithRequired(e => e.HoaDon)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KhuyenMai>()
                .HasMany(e => e.CT_KhuyenMai)
                .WithRequired(e => e.KhuyenMai)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiPhim>()
                .HasMany(e => e.Phim)
                .WithRequired(e => e.LoaiPhim)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhongChieu>()
                .HasMany(e => e.SuatChieu)
                .WithRequired(e => e.PhongChieu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SuatChieu>()
                .Property(e => e.GioBatDau)
                .HasPrecision(0);

            modelBuilder.Entity<SuatChieu>()
                .Property(e => e.GioKetThuc)
                .HasPrecision(0);
        }
    }
}
