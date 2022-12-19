using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class HoaDon
{
    public string MaHd { get; set; } = null!;

    public string? MaMh { get; set; }

    public string? MaKh { get; set; }

    public string? MaNv { get; set; }

    public DateTime? NgayLapHd { get; set; }

    public int? SoLuong { get; set; }

    public double? GiamGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual MatHang? MaMhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
