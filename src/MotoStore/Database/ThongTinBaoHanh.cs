using System;

namespace MotoStore.Database;

public partial class ThongTinBaoHanh
{
    public int Id { get; set; }

    public string MaBh { get; set; } = null!;

    public string MaHd { get; set; } = null!;

    public DateTime? ThoiGian { get; set; }

    public string? GhiChu { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
