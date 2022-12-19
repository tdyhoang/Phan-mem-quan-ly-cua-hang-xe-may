using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class MatHang
{
    public string MaMh { get; set; } = null!;

    public string? TenMh { get; set; }

    public decimal? GiaNhapMh { get; set; }

    public decimal? GiaBanMh { get; set; }

    public int? SoLuongTonKho { get; set; }

    public string? HangSx { get; set; }

    public string? XuatXu { get; set; }

    public string? MoTa { get; set; }

    public string? TinhTrang { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();

    public virtual NhaSanXuat? NhaSanXuat { get; set; }

    public virtual ICollection<ThongTinBaoHanh> ThongTinBaoHanhs { get; } = new List<ThongTinBaoHanh>();
}
