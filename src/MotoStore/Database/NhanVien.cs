using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class NhanVien
{
    public int Id { get; set; }

    public string MaNv { get; set; } = null!;

    public string? HoTenNv { get; set; }

    public DateTime? NgSinh { get; set; }

    public string GioiTinh { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    public DateTime? NgVl { get; set; }

    public decimal? Luong { get; set; }

    public bool DaXoa { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();

    public virtual ICollection<LenLich> LenLichs { get; } = new List<LenLich>();

    public virtual ICollection<LichSuHoatDong> LichSuHoatDongs { get; } = new List<LichSuHoatDong>();

    public virtual UserApp? UserApp { get; set; }
}
