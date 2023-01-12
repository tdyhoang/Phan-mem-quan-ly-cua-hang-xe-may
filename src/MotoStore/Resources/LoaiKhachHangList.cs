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
            Add("Vip");
            Add("Thuong");
            Add("Than quen");
        }
    }
}