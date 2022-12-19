using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MotoStore.Resources
{
    public class ChucVuList : List<string>
    {
        public ChucVuList()
        {
            this.Add("NVTuVan");
            this.Add("NVSuaXe");
            this.Add("NVBanHang");
            this.Add("NVQuanLy");
        }
    }
}