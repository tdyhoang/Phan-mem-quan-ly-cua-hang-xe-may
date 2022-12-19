using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string? HoTenKh { get; set; }

    public DateTime? NgSinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? LoaiKh { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();

    public virtual ICollection<ThongTinBaoHanh> ThongTinBaoHanhs { get; } = new List<ThongTinBaoHanh>();
}
