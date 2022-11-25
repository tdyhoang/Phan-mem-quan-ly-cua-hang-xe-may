using System;
using System.Collections.Generic;

namespace MotoStore.Models;

public partial class User
{
    public string MaNv { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? ChucVu { get; set; }

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
