using System;
using System.Collections.Generic;

namespace MotoStore.Databases;

public partial class NhaSanXuat
{
    public string TenNsx { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string NuocSx { get; set; } = null!;

    public virtual ICollection<MatHang> MatHangs { get; } = new List<MatHang>();
}
