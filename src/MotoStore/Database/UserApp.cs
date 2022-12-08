using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class UserApp
{
    public Guid UserId { get; set; }

    public Guid? MaNv { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }
}
