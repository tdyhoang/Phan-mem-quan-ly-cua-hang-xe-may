using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MotoStore.Models;

namespace MotoStore.Databases;

public partial class MainDatabase : DbContext
{
    public MainDatabase()
    {
    }

    public MainDatabase(DbContextOptions<MainDatabase> options)
        : base(options)
    {
    }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<MatHang> MatHangs { get; set; }

    public virtual DbSet<NhaSanXuat> NhaSanXuats { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<ThongTinBaoHanh> ThongTinBaoHanhs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=QLYCHBANXEMAY;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK_MaHD");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaHD");
            entity.Property(e => e.MaKh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaMh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaMH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayLapHd)
                .HasColumnType("smalldatetime")
                .HasColumnName("NgayLapHD");
            entity.Property(e => e.ThanhTien).HasColumnType("money");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_MaKH");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaMh)
                .HasConstraintName("FK_MaMH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_MaNV");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK_MaKH");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.HoTenKh)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("HoTenKH");
            entity.Property(e => e.LoaiKh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("LoaiKH");
            entity.Property(e => e.NgSinh).HasColumnType("smalldatetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<MatHang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK_MaMH");

            entity.ToTable("MatHang");

            entity.Property(e => e.MaMh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaMH");
            entity.Property(e => e.GiaBanMh)
                .HasColumnType("money")
                .HasColumnName("GiaBanMH");
            entity.Property(e => e.GiaNhapMh)
                .HasColumnType("money")
                .HasColumnName("GiaNhapMH");
            entity.Property(e => e.HangSx)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("HangSX");
            entity.Property(e => e.MoTa)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenMh)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("TenMH");
            entity.Property(e => e.TinhTrang)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.XuatXu)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.NhaSanXuat).WithMany(p => p.MatHangs)
                .HasForeignKey(d => new { d.HangSx, d.XuatXu })
                .HasConstraintName("FK_MH");
        });

        modelBuilder.Entity<NhaSanXuat>(entity =>
        {
            entity.HasKey(e => new { e.TenNsx, e.NuocSx }).HasName("PK_NSX");

            entity.ToTable("NhaSanXuat");

            entity.Property(e => e.TenNsx)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("TenNSX");
            entity.Property(e => e.NuocSx)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("NuocSX");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK_MaNV");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.ChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiaChi)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.HoTenNv)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("HoTenNV");
            entity.Property(e => e.Luong).HasColumnType("money");
            entity.Property(e => e.NgSinh).HasColumnType("smalldatetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Thuong).HasColumnType("money");
        });

        modelBuilder.Entity<ThongTinBaoHanh>(entity =>
        {
            entity.HasKey(e => e.MaBh).HasName("PK_MaBH");

            entity.ToTable("ThongTinBaoHanh");

            entity.Property(e => e.MaBh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaBH");
            entity.Property(e => e.GhiChu)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MaKh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaMh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaMH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.ThoiGian).HasColumnType("smalldatetime");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_TTBH_MaKH");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaMh)
                .HasConstraintName("FK_TTBH_MaMH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_TTBH_MaNV");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK_Users_MaNV");

            entity.Property(e => e.MaNv)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.ChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaNvNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_MaNV");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
