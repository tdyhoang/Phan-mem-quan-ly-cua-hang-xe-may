using System.Collections.Generic;

namespace MotoStore.Database;

public partial class NhaCungCap
{
    public int Id { get; set; }

    public string MaNcc { get; set; } = null!;

    public string TenNcc { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public bool DaXoa { get; set; }

    public virtual ICollection<MatHang> MatHangs { get; } = new List<MatHang>();
}
