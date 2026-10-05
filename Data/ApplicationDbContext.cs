using Microsoft.EntityFrameworkCore;
using QuanLyDeTaiDoAn.Models;

namespace QuanLyDeTaiDoAn.Data
{
    // Ghi chú: DbContext là tài sản CHUNG của cả nhóm - cả 4 Module cùng dùng.
    // Khi thêm/sửa Entity, cả nhóm cần thống nhất trước rồi mới tạo Migration mới
    // (xem mục 10.1 đề bài) để tránh xung đột Migration giữa các thành viên.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LinhVuc> LinhVucs { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<DeTai> DeTais { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<DangKyDeTai> DangKyDeTais { get; set; }
        public DbSet<PhanCongHuongDan> PhanCongHuongDans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= TaiKhoan =================
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // Quan hệ 1-1: TaiKhoan <-> SinhVien
            modelBuilder.Entity<SinhVien>()
                .HasOne(sv => sv.TaiKhoan)
                .WithOne(tk => tk.SinhVien)
                .HasForeignKey<SinhVien>(sv => sv.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SinhVien>()
                .HasIndex(sv => sv.MaTaiKhoan)
                .IsUnique();

            modelBuilder.Entity<SinhVien>()
                .HasIndex(sv => sv.Email)
                .IsUnique();

            // ================= LinhVuc =================
            modelBuilder.Entity<LinhVuc>()
                .HasIndex(lv => lv.TenLinhVuc)
                .IsUnique();

            // ================= GiangVien =================
            modelBuilder.Entity<GiangVien>()
                .HasOne(gv => gv.LinhVuc)
                .WithMany(lv => lv.GiangViens)
                .HasForeignKey(gv => gv.MaLinhVuc)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa LinhVuc nếu còn GiangVien

            modelBuilder.Entity<GiangVien>()
                .HasIndex(gv => gv.Email)
                .IsUnique();

            // ================= DeTai =================
            modelBuilder.Entity<DeTai>()
                .HasOne(dt => dt.LinhVuc)
                .WithMany(lv => lv.DeTais)
                .HasForeignKey(dt => dt.MaLinhVuc)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa LinhVuc nếu còn DeTai

            // Tên đề tài không trùng trong phạm vi Năm học + Học kỳ
            modelBuilder.Entity<DeTai>()
                .HasIndex(dt => new { dt.TenDeTai, dt.NamHoc, dt.HocKy })
                .IsUnique();

            // ================= DangKyDeTai (Module 3) =================
            modelBuilder.Entity<DangKyDeTai>()
                .HasOne(dk => dk.SinhVien)
                .WithMany(sv => sv.DangKyDeTais)
                .HasForeignKey(dk => dk.MaSinhVien)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa SinhVien nếu còn DangKyDeTai

            modelBuilder.Entity<DangKyDeTai>()
                .HasOne(dk => dk.DeTai)
                .WithMany(dt => dt.DangKyDeTais)
                .HasForeignKey(dk => dk.MaDeTai)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa DeTai nếu còn DangKyDeTai

            // ================= PhanCongHuongDan (Module 4) =================
            modelBuilder.Entity<PhanCongHuongDan>()
                .HasOne(pc => pc.DangKyDeTai)
                .WithMany(dk => dk.PhanCongHuongDans)
                .HasForeignKey(pc => pc.MaDangKy)
                .OnDelete(DeleteBehavior.Restrict); // Giữ lịch sử, không cho xóa DangKyDeTai

            modelBuilder.Entity<PhanCongHuongDan>()
                .HasOne(pc => pc.GiangVien)
                .WithMany(gv => gv.PhanCongHuongDans)
                .HasForeignKey(pc => pc.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
