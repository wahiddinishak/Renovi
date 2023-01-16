using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // uam
        public virtual DbSet<user> Users { get; set; }
        public virtual DbSet<role> Roles { get; set; }
        public virtual DbSet<menu> Menus { get; set; }
        public virtual DbSet<accessRight> AccessRights { get; set; }

        // master
        public DbSet<item> Items { get; set; }
        public DbSet<tukang> Tukangs { get; set; }

        // proyek
        public DbSet<Proyek> proyeks { get; set; }
        public DbSet<ActH> actHs { get; set; }
        public DbSet<ActD> actDs { get; set; }
        public DbSet<Material> materials { get; set; }
        public DbSet<Tagihan> tagihans { get; set; }
        public DbSet<analisa> analisas { get; set; }
        public DbSet<Personil> personils { get; set; }
        public DbSet<Jadwal> jadwals { get; set; }
        public DbSet<showAddButton> showAddButtons { get; set; }

        // absen
        public DbSet<Absen> absens { get; set; }
        public DbSet<AbsenDetail> absenDetails { get; set; }

        // laporan
        public DbSet<Laporan> laporans { get; set; }
        public DbSet<LaporanUsage> laporanUsages { get; set; }
        public DbSet<LaporanOverheadProfit> laporanOverheadProfits { get; set; }
        public DbSet<LaporanAttachment> laporanAttachments { get; set; }

    }
}
