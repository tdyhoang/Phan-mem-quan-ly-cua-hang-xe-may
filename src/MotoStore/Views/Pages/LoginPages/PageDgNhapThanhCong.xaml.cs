using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MotoStore.Views.Pages.LoginPages
{
    /// <summary>
    /// Interaction logic for PageDgNhapThanhCong.xaml
    /// </summary>
    public partial class PageDgNhapThanhCong : Page
    {
        private PageChinh pgChinh;
        public PageDgNhapThanhCong()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public PageDgNhapThanhCong(PageChinh pgC)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            pgChinh = pgC;
        }

        private void PageDgNhapThanhCong_Loaded(object sender, RoutedEventArgs e)
        {
            MainDatabase mdb = new MainDatabase();
            DateTime dt = DateTime.Now;
            switch(PageChinh.getTK)
            {
                case "Ngquanly1":
                    lblXinChao.Content = "Xin Chào, Trung";
                    txtblSoNV.Text = "   Số Nhân Viên\n   Bạn Quản Lý:\n"+"".PadRight(12) + (mdb.NhanViens.Select(d => d.MaNv).Count() - 1).ToString();                  
                    txtblSoXe.Text= "".PadRight(9)+ "Số Xe\n"+"".PadRight(5)+"Trong Kho:\n"+"".PadRight(11)+ mdb.MatHangs.Sum(d => d.SoLuongTonKho).ToString();
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\anh3.png"));
                    break;
                case "Nhvien1":
                    lblXinChao.Content = "Xin Chào, Thương";
                    //2 dòng dưới để lấy ngày vào làm của nhân viên và tính số ngày từ đó đến nay
                    var dx = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                    int d3 = (int)(dt - dx).Value.TotalDays;

                    txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d3.ToString() + " Ngày";
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNu.png"));
                    break;
                case "Nhvien2":
                    lblXinChao.Content = "Xin Chào, Sang";
                    var dq = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                    int d4 = (int)(dt - dq).Value.TotalDays;
                    txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d4.ToString() + " Ngày";
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNam.png"));
                    break;
            }   
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblGioHeThong.Content = "Bây giờ là: " + DateTime.Now.ToLongTimeString();
        }

        private void btnDgXuat_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(pgChinh);



        }
    }
}
