using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class DonDatHang
{
    public int Id { get; set; }

    public string MaDdh { get; set; } = null!;

    public string MaMh { get; set; } = null!;

    public int? SoLuongHang { get; set; }

    public string MaKh { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public DateTime? Ngdh { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual MatHang MaMhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
