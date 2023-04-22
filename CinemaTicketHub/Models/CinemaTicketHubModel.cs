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

        public virtual DbSet<Ghe> Ghe { get; set; }
        public virtual DbSet<LoaiPhim> LoaiPhim { get; set; }
        public virtual DbSet<Phim> Phim { get; set; }
        public virtual DbSet<PhongChieu> PhongChieu { get; set; }
        public virtual DbSet<SuatChieu> SuatChieu { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Ve> Ve { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ghe>()
                .HasMany(e => e.Ve)
                .WithRequired(e => e.Ghe)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiPhim>()
                .HasMany(e => e.Phim)
                .WithRequired(e => e.LoaiPhim)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Phim>()
                .HasMany(e => e.SuatChieu)
                .WithRequired(e => e.Phim)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhongChieu>()
                .HasMany(e => e.Ghe)
                .WithRequired(e => e.PhongChieu)
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

            modelBuilder.Entity<SuatChieu>()
                .HasMany(e => e.Ve)
                .WithRequired(e => e.SuatChieu)
                .WillCascadeOnDelete(false);
        }
    }
}
