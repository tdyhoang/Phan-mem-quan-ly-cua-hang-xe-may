using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class DonDatHang
{
    public Guid MaDonDh { get; set; }

    public Guid MaMh { get; set; }

    public int? SoLuongHang { get; set; }

    public Guid MaKh { get; set; }

    public Guid MaNv { get; set; }

    public DateTime? Ngdh { get; set; }

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual MatHang MaMhNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
