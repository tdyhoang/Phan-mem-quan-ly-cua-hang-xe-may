using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MotoStore.Resources;
using MotoStore.Properties;
using System.Windows;

namespace MotoStore.Helpers
{
    public class BitmapImageFromFile
    {
        public static BitmapImage? BitmapImageFromString(string filepath) //string mamh
        {
            string file;
            if (File.Exists(filepath))
                file = filepath;
            if (File.Exists(filepath + ".png"))
                file = filepath + ".png";
            else if (File.Exists(filepath + ".jpg"))
                file = filepath + ".jpg";
            else if (File.Exists(filepath + ".jpeg"))
                file = filepath + ".jpeg";
            else
                return null;

            using Stream imageStreamSource = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            BitmapImage bi = new BitmapImage();
            Stream stream = new MemoryStream();
            imageStreamSource.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);
            bi.BeginInit();
            bi.StreamSource = stream;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }
        //đưa hàm cập nhật ảnh vào đây xử lý luôn
    }
}
