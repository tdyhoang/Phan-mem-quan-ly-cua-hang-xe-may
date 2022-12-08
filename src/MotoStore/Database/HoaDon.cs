using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class HoaDon
{
    public Guid MaHd { get; set; }

    public Guid? MaMh { get; set; }

    public Guid? MaKh { get; set; }

    public Guid? MaNv { get; set; }

    public string? HoTenNv { get; set; }

    public DateTime? NgayLapHd { get; set; }

    public int? SoLuong { get; set; }

    public double? GiamGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual MatHang? MaMhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
