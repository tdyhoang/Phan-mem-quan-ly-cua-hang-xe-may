using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string? HoTenNv { get; set; }

    public DateTime? NgSinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    public decimal? Luong { get; set; }

    public decimal? Thuong { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();

    public virtual ICollection<ThongTinBaoHanh> ThongTinBaoHanhs { get; } = new List<ThongTinBaoHanh>();

    public virtual ICollection<UserAdmin> UserAdmins { get; } = new List<UserAdmin>();
}
