namespace MotoStore.Database;

public partial class UserApp
{
    public string MaNv { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
