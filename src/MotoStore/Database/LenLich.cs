using System;
using System.Collections.Generic;

namespace MotoStore.Database;

public partial class LenLich
{
    public Guid? MaNv { get; set; }

    public DateTime? NgLenLichBd { get; set; }

    public DateTime? NgLenLichKt { get; set; }

    public string? NoiDungLenLich { get; set; }
}
