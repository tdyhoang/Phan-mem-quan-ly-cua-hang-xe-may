using System;

namespace MotoStore.Database;

public partial class LenLich
{
    public Guid LenLichId { get; set; }

    public string MaNv { get; set; } = null!;

    public DateTime NgLenLichBd { get; set; }

    public DateTime NgLenLichKt { get; set; }

    public string? NoiDungLenLich { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
