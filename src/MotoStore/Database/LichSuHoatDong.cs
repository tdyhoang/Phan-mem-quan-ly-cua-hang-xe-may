using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class LichSuHoatDong
{
    public Guid LshdId { get; set; }

    public Guid MaNv { get; set; }

    public DateTime? ThoiGian { get; set; }

    public string? HoatDong { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
