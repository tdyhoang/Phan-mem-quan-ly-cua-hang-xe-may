using System;
using System.Collections.Generic;

namespace MotoStore.Database;
public partial class LenLich
{
    public Guid? MaNV { get; set; }
    public DateTime? NgLenLichBD { get; set; }
    public DateTime? NgLenLichKT { get; set; }
    public string? NoiDungLenLich { get; set; }
    //public virtual NhanVien? MaNvNavigation { get; set; }
}
