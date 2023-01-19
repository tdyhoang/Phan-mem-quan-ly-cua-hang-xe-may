using System;

namespace MotoStore.Database;

public partial class LichSuHoatDong
{
    public Guid LshdId { get; set; }

    public string MaNv { get; set; } = null!;

    public DateTime? ThoiGian { get; set; }

    public string? HoatDong { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
