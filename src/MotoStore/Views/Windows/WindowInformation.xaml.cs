using MotoStore.Database;
using System.Windows;
using System.Windows.Input;
using MotoStore.Views.Pages;
using MotoStore.Views.Pages.IOPagePages;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace MotoStore.Views.Windows
{
    /// <summary>
    /// Interaction logic for WindowInformation.xaml
    /// </summary>
    public partial class WindowInformation : Window
    {
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        private MainDatabase mdb = new();
        Product PD;
        string ma;
        string ten;
        decimal gia;
        string anh;
        public WindowInformation(Product thamsoSP)
        {
            InitializeComponent();
            ten = thamsoSP.NameProduct;
            gia = thamsoSP.ValueMoney.Value;
            //anh = $"/Products Images/{thamsoSP.ProductId}.png";
            PD = new Product(ten, gia, anh);
            foreach (var xe in mdb.MatHangs.ToList())
            {
                if (xe.TenMh == ten)
                {
                    lblMaMH.Content = xe.MaMh;
                    anh = "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + xe.MaMh + ".png";
                    lblTenMH.Content = xe.TenMh;
                    lblSoPK.Content = xe.SoPhanKhoi;
                    ma = xe.MaMh;
                    txtGiaBan.Text = xe.GiaBanMh.Value.ToString();
                    txtMau.Text = xe.Mau;

                    //Tìm số xe đã bán
                    con.Open();
                    var soxe = mdb.HoaDons.Where(u => u.MaMh == ma).Select(u => u.SoLuong).Sum();
                    lblDaBan.Content = soxe.ToString() + " Chiếc";
                    //ảnh
                    BitmapImage image = new();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new(anh);
                    image.EndInit();
                    anhSP.ImageSource = image;
                    break;
                }
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void WindowInformation_Initialized(object sender, System.EventArgs e)
        {
           
        }

        private void txtGiaBan_LostFocus(object sender, RoutedEventArgs e)
        {
            txtGiaBan.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void txtGiaBan_GotFocus(object sender, RoutedEventArgs e)
        {
            txtGiaBan.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtMau_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMau.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtMau_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMau.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }
    }
}
