using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private readonly SqlConnection con = new(Properties.Settings.Default.ConnectionString);
        private readonly MainDatabase mdb = new();
        static public string tenXeBanChay;
        static public int SoLgXeBanChay;
        public ReportPage()
        {
            InitializeComponent();

            /*Tìm Tên Xe Bán Chạy Nhất trên SQL:
              SELECT TENMH
              FROM MATHANG 
              WHERE MaMH = (SELECT TOP(1) MaMH FROM HOADON Group by MaMH Order by SUM(SoLuong) DESC)
             */
            var XeBanChay = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            tenXeBanChay = mdb.MatHangs.Where(i => i.MaMh == XeBanChay.IdXe).Select(i => i.TenMh).FirstOrDefault();
            txtblThgTinMHBanChay.Text = $"{tenXeBanChay}\nMã Nhân Viên: {XeBanChay.IdXe}";
            //3 Dòng trên để tìm ra mặt hàng bán chạy nhất
            SoLgXeBanChay = XeBanChay.Tong;

            var NVNgSuat = mdb.HoaDons.GroupBy(u => u.MaNv).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdNv = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            var tenNVNgSuat = mdb.NhanViens.Where(i => i.MaNv == NVNgSuat.IdNv).Select(i => i.HoTenNv).FirstOrDefault();
            txtblThgTinNVNgSuat.Text = $"{tenNVNgSuat}\nMã Nhân Viên: {NVNgSuat.IdNv}";
            //3 Dòng trên để tìm ra nhân viên năng suất nhất

            string StartDate = "1/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            string EndDate;
            if (DateTime.Now.Month == 12)
            {
                int namsau = DateTime.Now.Year + 1;
                EndDate = "1/" + 1 + "/" + namsau;
            }
            else
            {
                int thangsau = DateTime.Now.Month + 1;
                EndDate = "1/" + thangsau + "/" + DateTime.Now.Year;
            }
            
            string commandText = "Set Dateformat dmy\nSelect Sum(SoLuong) from HoaDon Where NgayLapHD >=@StartDate and NgayLapHD < @EndDate";
            SqlCommand command = new(commandText, con);
            command.Parameters.Add("@StartDate", System.Data.SqlDbType.SmallDateTime);
            command.Parameters["@StartDate"].Value = StartDate;
            command.Parameters.Add("@EndDate", System.Data.SqlDbType.SmallDateTime);
            command.Parameters["@EndDate"].Value = EndDate;
            con.Open();
            int soxebandc = 0;
            SqlDataReader sda = command.ExecuteReader();
            if (sda.Read())
            {
                if (sda[0] != DBNull.Value)
                    soxebandc = (int)sda[0];
                else
                    soxebandc = 0;
            }
            txtblThgTinSoXeBanDc.Text = soxebandc.ToString();

            string commandTextMoney = "Set Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon Where NgayLapHD >=@StartDate and NgayLapHD < @EndDate";           
            SqlCommand commandMoney = new(commandTextMoney, con);
            commandMoney.Parameters.Add("@StartDate", System.Data.SqlDbType.SmallDateTime);
            commandMoney.Parameters["@StartDate"].Value = StartDate;
            commandMoney.Parameters.Add("@EndDate", System.Data.SqlDbType.SmallDateTime);
            commandMoney.Parameters["@EndDate"].Value = EndDate;
            decimal sotien = 0;
            sda=commandMoney.ExecuteReader();
            if (sda.Read())
            {
                if (sda[0] != DBNull.Value)
                    sotien = (decimal)sda[0];
                else
                    sotien = 0;
            }
            txtblDoanhThu.Text = sotien.ToString() + "$";

            int prevMonth = DateTime.Now.AddMonths(-1).Month;
            int Year = DateTime.Now.Year;
            decimal DTThgTrc = 0;
            if (prevMonth == 12)
                Year -= 1;
            string StartDateThangTrc = "1/" + prevMonth + "/" + Year;
            string EndDateThangTrc = "1/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            string commandTextDTThgTrc = "Set Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon Where NgayLapHD >=@StartDateThangTrc and NgayLapHD < @EndDateThangTrc";
            SqlCommand commandDTThgTrc = new(commandTextDTThgTrc, con);
            commandDTThgTrc.Parameters.Add("@StartDateThangTrc", System.Data.SqlDbType.SmallDateTime);
            commandDTThgTrc.Parameters["@StartDateThangTrc"].Value = StartDateThangTrc;
            commandDTThgTrc.Parameters.Add("@EndDateThangTrc", System.Data.SqlDbType.SmallDateTime);
            commandDTThgTrc.Parameters["@EndDateThangTrc"].Value = EndDateThangTrc;
            sda = commandDTThgTrc.ExecuteReader();
            if (sda.Read())
            {
                if (sda[0] != DBNull.Value)
                    DTThgTrc = (decimal)sda[0];
                else
                    DTThgTrc = 0;
            }
            decimal Diff = DTThgTrc - sotien;
            if (Diff > 0)
            {
                txtblThgTinMHBanE.Text = "Giảm\n" + Diff.ToString() + "$";
                //AnhMHBanE.ImageSource = new BitmapImage(new(@"pack://application:,,,/Images/IconLowSales.png"));
            }
            else if (Diff == 0)
            {
                txtblThgTinMHBanE.Text = "Giữ Nguyên";
                //AnhMHBanE.ImageSource = new BitmapImage(new(@"pack://application:,,,/Images/stb-removebg-preview.png"));
            }
            else
            {
                txtblThgTinMHBanE.Text = "Tăng\n" + Math.Abs(Diff).ToString() + "$";
                //AnhMHBanE.ImageSource = new BitmapImage(new(@"pack://application:,,,/Images/highSaleIcon.png"));
                AnhMHBanE.ImageSource = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\highSaleIcon.png"));
            }

            var KhVIP = mdb.HoaDons.GroupBy(u => u.MaKh).Select(u => new { Tong = u.Sum(u => u.ThanhTien), IdKhach = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            var tenKhVIP = mdb.KhachHangs.Where(u => u.MaKh == KhVIP.IdKhach).Select(u => u.HoTenKh).FirstOrDefault();
            txtblThgTinKHVIP.Text = $"{tenKhVIP}\nMã Nhân Viên: {KhVIP.IdKhach}";
            //đồ thị check if DBNULL nếu null thì cho = 0
        }

        private void brdNVNgSuat_MouseMove(object sender, MouseEventArgs e)
        {
            //brdNVNgSuat.Margin = new Thickness(300, 3, 300, 217);
        }

        private void brdNVNgSuat_MouseLeave(object sender, MouseEventArgs e)
        {
            //brdNVNgSuat.Margin = new Thickness(300, 33, 300, 217);
        }

        private void brdSoXeBanDcThgNay_MouseMove(object sender, MouseEventArgs e)
        {
            //brdSoXeBanDcThgNay.Margin = new Thickness(576, 3, 24, 217);
        }

        private void brdSoXeBanDcThgNay_MouseLeave(object sender, MouseEventArgs e)
        {
            //brdSoXeBanDcThgNay.Margin = new Thickness(576, 33, 24, 217);
        }

        private void brdMHBanChay_MouseMove(object sender, MouseEventArgs e)
        {
            //brdMHBanChay.Margin = new Thickness(27, 3, 573, 217);
        }

        private void brdMHBanChay_MouseLeave(object sender, MouseEventArgs e)
        {
            //brdMHBanChay.Margin = new Thickness(27, 33, 573, 217);
        }

        private void brdMHBanE_MouseMove(object sender, MouseEventArgs e)
        {
            //brdMHBanE.Margin = new Thickness(27, 196, 573, 24);
        }

        private void brdMHBanE_MouseLeave(object sender, MouseEventArgs e)
        {
            //brdMHBanE.Margin = new Thickness(27, 226, 573, 24);
        }

        private void brdDoanhThuThgNay_MouseMove(object sender, MouseEventArgs e)
        {
            //brdDoanhThuThgNay.Margin = new Thickness(576, 197, 24, 23);
        }

        private void brdDoanhThuThgNay_MouseLeave(object sender, MouseEventArgs e)
        {
           // brdDoanhThuThgNay.Margin = new Thickness(576, 227, 24, 23);
        }

        private void brdKHVIP_MouseMove(object sender, MouseEventArgs e)
        {
            //brdKHVIP.Margin = new Thickness(300, 196, 300, 24);
        }

        private void brdKHVIP_MouseLeave(object sender, MouseEventArgs e)
        {
           // brdKHVIP.Margin = new Thickness(300, 226, 300, 24);
        }

        private void btnBieuDo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PageBieuDo());
        }

        private void border6ThgTin_MouseMove(object sender, MouseEventArgs e)
        {
            border6ThgTin.Opacity = 1;
        }

        private void border6ThgTin_MouseLeave(object sender, MouseEventArgs e)
        {
            border6ThgTin.Opacity = 0.9;
        }

    }
}
