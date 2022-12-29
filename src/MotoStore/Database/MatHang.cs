using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class MatHang
{
    public int Id { get; set; }

    public string MaMh { get; set; } = null!;

    public string? TenMh { get; set; }

    public int? SoPhanKhoi { get; set; }

    public decimal? GiaNhapMh { get; set; }

    public decimal? GiaBanMh { get; set; }

    public int? SoLuongTonKho { get; set; }

    public string HangSx { get; set; } = null!;

    public string XuatXu { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool DaXoa { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();

    public virtual NhaSanXuat NhaSanXuat { get; set; } = null!;

    public virtual ICollection<ThongTinBaoHanh> ThongTinBaoHanhs { get; } = new List<ThongTinBaoHanh>();
}
