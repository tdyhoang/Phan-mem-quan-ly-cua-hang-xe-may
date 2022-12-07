using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MotoStore.Resources
{
    public class LoaiKhachHangList : List<string>
    {
        public LoaiKhachHangList()
        {
            this.Add("Vip");
            this.Add("Thuong");
            this.Add("Than quen");
        }
    }
}