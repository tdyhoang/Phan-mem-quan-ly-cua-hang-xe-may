using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MotoStore.Database;

public partial class MainDatabase : DbContext
{
    public MainDatabase()
    {
    }

    public MainDatabase(DbContextOptions<MainDatabase> options)
        : base(options)
    {
    }

    public virtual DbSet<DonDatHang> DonDatHangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LenLich> LenLichs { get; set; }

    public virtual DbSet<LichSuHoatDong> LichSuHoatDongs { get; set; }

    public virtual DbSet<MatHang> MatHangs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<ThongTinBaoHanh> ThongTinBaoHanhs { get; set; }

    public virtual DbSet<UserApp> UserApps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DonDatHang>(entity =>
        {
            entity.HasKey(e => e.MaDdh).HasName("PK_MaDonDH");

            entity.ToTable("DonDatHang");

            entity.Property(e => e.MaDdh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('DH'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaDDH");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MaKh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaMh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Ngdh)
                .HasColumnType("smalldatetime")
                .HasColumnName("NGDH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonDatHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaKHDDH");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.DonDatHangs)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaMHDDH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.DonDatHangs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaNVDDH");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK_MaHD");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('HD'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaHD");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MaKh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaMh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayLapHd)
                .HasColumnType("smalldatetime")
                .HasColumnName("NgayLapHD");
            entity.Property(e => e.ThanhTien).HasColumnType("money");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaKH");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaMH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaNV");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK_MaKH");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('KH'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(40);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(3);
            entity.Property(e => e.HoTenKh)
                .HasMaxLength(30)
                .HasColumnName("HoTenKH");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.LoaiKh)
                .HasMaxLength(10)
                .HasColumnName("LoaiKH");
            entity.Property(e => e.NgSinh).HasColumnType("smalldatetime");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<LenLich>(entity =>
        {
            entity.HasKey(e => e.LenLichId).HasName("PK_LenLichID");

            entity.ToTable("LenLich");

            entity.Property(e => e.LenLichId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("LenLichID");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.NgLenLichBd)
                .HasColumnType("smalldatetime")
                .HasColumnName("NgLenLichBD");
            entity.Property(e => e.NgLenLichKt)
                .HasColumnType("smalldatetime")
                .HasColumnName("NgLenLichKT");
            entity.Property(e => e.NoiDungLenLich).HasMaxLength(200);

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.LenLichs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKLL_MaNV");
        });

        modelBuilder.Entity<LichSuHoatDong>(entity =>
        {
            entity.HasKey(e => e.LshdId).HasName("PK_LshdID");

            entity.ToTable("LichSuHoatDong");

            entity.Property(e => e.LshdId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("LshdID");
            entity.Property(e => e.HoatDong).HasMaxLength(200);
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.ThoiGian).HasColumnType("datetime");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.LichSuHoatDongs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKLSHD_MaNV");
        });

        modelBuilder.Entity<MatHang>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK_MaMH");

            entity.ToTable("MatHang");

            entity.Property(e => e.MaMh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('MH'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
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
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MaNcc)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNCC");
            entity.Property(e => e.Mau).HasMaxLength(15);
            entity.Property(e => e.MoTa).HasMaxLength(75);
            entity.Property(e => e.TenMh)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("TenMH");
            entity.Property(e => e.XuatXu).HasMaxLength(15);

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.MatHangs)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MH");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK_NNCC");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNcc)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('CC'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(40);
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNcc)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("TenNCC");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK_MaNV");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('NV'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaNV");
            entity.Property(e => e.ChucVu).HasMaxLength(10);
            entity.Property(e => e.DiaChi).HasMaxLength(40);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(3);
            entity.Property(e => e.HoTenNv)
                .HasMaxLength(30)
                .HasColumnName("HoTenNV");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Luong).HasColumnType("money");
            entity.Property(e => e.NgSinh).HasColumnType("smalldatetime");
            entity.Property(e => e.NgVl)
                .HasColumnType("smalldatetime")
                .HasColumnName("NgVL");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<ThongTinBaoHanh>(entity =>
        {
            entity.HasKey(e => e.MaBh).HasName("PK_MaBH");

            entity.ToTable("ThongTinBaoHanh");

            entity.Property(e => e.MaBh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComputedColumnSql("('BH'+right('000'+CONVERT([varchar](3),[ID]),(3)))", true)
                .HasColumnName("MaBH");
            entity.Property(e => e.GhiChu).HasMaxLength(60);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MaKh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaKH");
            entity.Property(e => e.MaMh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.ThoiGian).HasColumnType("smalldatetime");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TTBH_MaKH");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TTBH_MaMH");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.ThongTinBaoHanhs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TTBH_MaNV");
        });

        modelBuilder.Entity<UserApp>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK_UserName");

            entity.ToTable("UserApp");

            entity.Property(e => e.UserName)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MaNV");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.UserApps)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UA_MaNV");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
