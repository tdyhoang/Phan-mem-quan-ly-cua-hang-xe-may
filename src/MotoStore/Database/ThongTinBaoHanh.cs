using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class ThongTinBaoHanh
{
    public Guid MaBh { get; set; }

    public Guid? MaMh { get; set; }

    public Guid? MaKh { get; set; }

    public Guid? MaNv { get; set; }

    public DateTime? ThoiGian { get; set; }

    public string? GhiChu { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual MatHang? MaMhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
