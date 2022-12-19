using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class DonDatHang
{
    public string MaDonDh { get; set; } = null!;

    public int? SoDonDh { get; set; }

    public string? MaMh { get; set; }

    public int? SoLuongHang { get; set; }

    public string? MaKh { get; set; }

    public string? MaNv { get; set; }

    public DateTime? Ngdh { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual MatHang? MaMhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
