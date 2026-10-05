using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LinhVuc> LinhVucs { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<DeTai> DeTais { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<DangKyDeTai> DangKyDeTais { get; set; }
        public DbSet<PhanCongHuongDan> PhanCongHuongDans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ràng buộc không trùng
            modelBuilder.Entity<TaiKhoan>().HasIndex(t => t.TenDangNhap).IsUnique();
            modelBuilder.Entity<LinhVuc>().HasIndex(l => l.TenLinhVuc).IsUnique();
            modelBuilder.Entity<GiangVien>().HasIndex(g => g.Email).IsUnique();
            modelBuilder.Entity<SinhVien>().HasIndex(s => s.Email).IsUnique();

            // TaiKhoan (1) - (1) SinhVien
            modelBuilder.Entity<SinhVien>()
                .HasOne(s => s.TaiKhoan)
                .WithOne(t => t.SinhVien)
                .HasForeignKey<SinhVien>(s => s.MaTaiKhoan);

            // Tắt Cascade Delete cho các quan hệ có thể gây multiple cascade paths
            modelBuilder.Entity<GiangVien>()
                .HasOne(g => g.LinhVuc)
                .WithMany(l => l.DanhSachGiangVien)
                .HasForeignKey(g => g.MaLinhVuc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeTai>()
                .HasOne(d => d.LinhVuc)
                .WithMany(l => l.DanhSachDeTai)
                .HasForeignKey(d => d.MaLinhVuc)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DangKyDeTai>()
                .HasOne(dk => dk.SinhVien)
                .WithMany(sv => sv.DanhSachDangKy)
                .HasForeignKey(dk => dk.MaSinhVien)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DangKyDeTai>()
                .HasOne(dk => dk.DeTai)
                .WithMany(dt => dt.DanhSachDangKy)
                .HasForeignKey(dk => dk.MaDeTai)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhanCongHuongDan>()
                .HasOne(pc => pc.DangKyDeTai)
                .WithMany(dk => dk.DanhSachPhanCong)
                .HasForeignKey(pc => pc.MaDangKy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PhanCongHuongDan>()
                .HasOne(pc => pc.GiangVien)
                .WithMany(g => g.DanhSachPhanCong)
                .HasForeignKey(pc => pc.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}