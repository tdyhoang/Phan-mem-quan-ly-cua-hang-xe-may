﻿using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class KhachHang
{
    public int Id { get; set; }

    public string MaKh { get; set; } = null!;

    public string HoTenKh { get; set; } = null!;

    public DateTime? NgSinh { get; set; }

    public string GioiTinh { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string LoaiKh { get; set; } = null!;

    public bool DaXoa { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; } = new List<DonDatHang>();

    public virtual ICollection<HoaDon> HoaDons { get; } = new List<HoaDon>();
}
