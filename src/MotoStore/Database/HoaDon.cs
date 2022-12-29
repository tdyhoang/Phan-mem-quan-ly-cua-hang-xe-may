using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class HoaDon
{
    public int Id { get; set; }

    public string MaHd { get; set; } = null!;

    public string MaMh { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public DateTime? NgayLapHd { get; set; }

    public int? SoLuong { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual MatHang MaMhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
