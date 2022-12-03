using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class ThongTinBaoHanh
{
    public string MaBh { get; set; } = null!;

    public string? MaMh { get; set; }

    public string? MaKh { get; set; }

    public string? MaNv { get; set; }

    public DateTime? ThoiGian { get; set; }

    public string? GhiChu { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual MatHang? MaMhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
