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
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Win32;
using MotoStore.Views.Pages.LoginPages;
using System.IO;

namespace MotoStore.Views.Windows
{
    /// <summary>
    /// Interaction logic for WindowInformation.xaml
    /// </summary>
    public partial class WindowInformation : Window
    {
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        private Tuple<MatHang, string> mathang;
        private List<string> ListAnhSP = new();
        private IOSanPhamPage IOSPpg = new();
        public WindowInformation(Tuple<MatHang,string> thamso,IOSanPhamPage thamsoPage)
        {
            InitializeComponent();
            MainDatabase mdb = new();
            IOSPpg = thamsoPage;
            mathang = new(thamso.Item1, thamso.Item2);
            lblMaMH.Content = mathang.Item1.MaMh;
            lblTenMH.Content = mathang.Item1.TenMh;
            lblSoPK.Content = mathang.Item1.SoPhanKhoi;
            lblHangSX.Content = mathang.Item1.HangSx;
            lblXuatXu.Content = mathang.Item1.XuatXu;
            lblDaBan.Content = mdb.HoaDons.Where(u => u.MaMh == mathang.Item1.MaMh).Select(u => u.SoLuong).Sum().ToString() + " Chiếc";
            int SLtonkho = mdb.MatHangs.Select(u => u.SoLuongTonKho).FirstOrDefault().Value;
            int SLdaban = mdb.HoaDons.Where(u => u.MaMh == mathang.Item1.MaMh).Select(u => u.SoLuong).Sum().Value;
            txtTonKho.Text = (SLtonkho - SLdaban).ToString();
            if (mathang.Item1.GiaBanMh != null)
                txtGiaBan.Text = string.Format("{0:C}", mathang.Item1.GiaBanMh);
            if (mathang.Item1.Mau != null)
                txtMau.Text = mathang.Item1.Mau;
            else 
                txtMau.Text=string.Empty;
            if (mathang.Item2 != null)
                anhSP.Source = new BitmapImage(new Uri(mathang.Item2, UriKind.Relative));
            //else cập nhật ảnh xe default
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

        private void txtTonKho_GotFocus(object sender, RoutedEventArgs e)
        {
            txtTonKho.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtTonKho_LostFocus(object sender, RoutedEventArgs e)
        {
            txtTonKho.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void btnCapNhatAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new();
            OFD.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            //string destFile = $"/Products Images/{mathang.Item1.MaMh}.png.BKup";
            //string newPathToFile = $"/Products Images/{mathang.Item1.MaMh}.png";
            //dùng 2 dòng trên bị lỗi Could not find a part of the path 'C:\Products Images\ ... 

            string destFile ="C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\"+ mathang.Item1.MaMh + ".BKup";
            string newPathToFile = "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + mathang.Item1.MaMh + ".png";
            if (OFD.ShowDialog() == true)
            {
                if (File.Exists(newPathToFile)) //Nếu có 1 file ảnh khác tồn tại thì xoá nó đi và cập nhật file ảnh mới
                {
                    File.Move(newPathToFile, destFile); //Đổi tên File
                }
                File.Copy(OFD.FileName, newPathToFile); //Chỉnh tên File ảnh đc chọn
                BitmapImage image = new();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new(newPathToFile);
                image.EndInit();
                anhSP.Source = image;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(destFile); //Xoá file tạm đi
                MessageBox.Show("Cập nhật ảnh thành công!");
                IOSPpg.Refresh();
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
