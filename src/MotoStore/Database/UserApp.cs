using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class UserApp
{
    public Guid? MaNv { get; set; }

    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string? Email { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }
}
