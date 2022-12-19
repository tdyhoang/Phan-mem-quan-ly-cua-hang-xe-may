using System;
using System.Collections.Generic;

namespace MotoStore.Database;
public partial class LichSuHoatDong
{
    public Guid? MaNV { get; set; }

    public DateTime? ThoiGian { get; set; }
    public string? HoatDong { get; set; }
}
