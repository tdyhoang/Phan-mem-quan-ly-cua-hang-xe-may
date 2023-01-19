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
using System.Windows.Navigation;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        private readonly MainDatabase mdb = new();
        static public string tenXeBanChay;
        static public int SoLgXeBanChay;
        public ReportPage()
        {
            InitializeComponent();

            PointLabel = chartPoint =>
            string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            //Lấy ra top 5 sp bán chạy

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay,
                Values = new ChartValues<int> { SoLgXeBanChay },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.Red
            });

            /* var query = from item in mdb.HoaDons
                         group item by item.MaMh into g
                         orderby g.Sum(x => x.SoLuong) descending
                         select g;
             var topFive = query.Take(5); */

            var XeBanChay2 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(1).Take(1).Single();
            var tenXeBanChay2 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay2.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay2,
                Values = new ChartValues<int> { XeBanChay2.Tong },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.Purple
            });

            var XeBanChay3 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(2).Take(1).Single();
            var tenXeBanChay3 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay3.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay3,
                Values = new ChartValues<int> { XeBanChay3.Tong },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.SkyBlue
            });

            var XeBanChay4 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(3).Take(1).Single();
            var tenXeBanChay4 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay4.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay4,
                Values = new ChartValues<int> { XeBanChay4.Tong },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.Orange
            });

            var XeBanChay5 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(4).Take(1).Single();
            var tenXeBanChay5 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay5.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay5,
                Values = new ChartValues<int> { XeBanChay5.Tong },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.ForestGreen
            });

            bieudoTron.Series = SrC;
            //Các dòng trên của biểu đồ Tròn

            //Các dòng dưới của biểu đồ Cột
            decimal[] arrTienKhach = new decimal[5];
            string[] arrMaKhach = new string[5];
            string[] arrTenKhach = new string[5];
            con.Open();
            for (int i = 0; i < 5; i++)
            {
                SqlCommand cmd = new("SELECT MaKH FROM  HoaDon Group by MaKH ORDER BY sum(ThanhTien) desc OFFSET @i ROWS FETCH NEXT 1 ROWS ONLY;", con);
                cmd.Parameters.Add("@i", System.Data.SqlDbType.Int);
                cmd.Parameters["@i"].Value = i;
                SqlDataReader SDA = cmd.ExecuteReader();
                if (SDA.Read())
                    if (SDA[0] != DBNull.Value)
                        arrMaKhach[i] = (string)SDA[0];

                SqlCommand cmd2 = new("Select Sum(ThanhTien) from HoaDon Where MaKH = @ThamSoMaKH", con);
                cmd2.Parameters.Add("@ThamSoMaKH", System.Data.SqlDbType.Char);
                cmd2.Parameters["@ThamSoMaKH"].Value = arrMaKhach[i];
                SqlDataReader sda2 = cmd2.ExecuteReader();
                if (sda2.Read())
                    if (sda2[0] != DBNull.Value)
                        arrTienKhach[i] = (decimal)sda2[0];

                SqlCommand cmd3 = new("Select HoTenKH from KhachHang where MaKH = @thamsoMaKH", con);
                cmd3.Parameters.Add("@thamsoMaKH", System.Data.SqlDbType.NVarChar);
                cmd3.Parameters["@thamsoMaKH"].Value = arrMaKhach[i];
                SqlDataReader sda3 = cmd3.ExecuteReader();
                if (sda3.Read())
                    if (sda3[0] != DBNull.Value)
                        arrTenKhach[i] = (string)sda3[0];
                LabelsKhach.Add(arrTenKhach[i]);
            }
            con.Close();

            SeriesColKhach.Add(new ColumnSeries
            {
                Title = arrTenKhach[0],
                Values = new ChartValues<decimal> { arrTienKhach[0] },
                Fill = Brushes.Red
            });
            SeriesColKhach.Add(new ColumnSeries
            {
                Title = arrTenKhach[1],
                Values = new ChartValues<decimal> { arrTienKhach[1] },
                Fill = Brushes.Green
            });
            SeriesColKhach.Add(new ColumnSeries
            {
                Title = arrTenKhach[2],
                Values = new ChartValues<decimal> { arrTienKhach[2] },
                Fill = Brushes.Yellow
            });
            SeriesColKhach.Add(new ColumnSeries
            {
                Title = arrTenKhach[3],
                Values = new ChartValues<decimal> { arrTienKhach[3] },
                Fill = Brushes.Blue
            });
            SeriesColKhach.Add(new ColumnSeries
            {
                Title = arrTenKhach[4],
                Values = new ChartValues<decimal> { arrTienKhach[4] },
                Fill = Brushes.Purple
            });

            Values = value => value.ToString("N");
            DataContext = this;

            /*Tìm Tên Xe Bán Chạy Nhất trên SQL:
              SELECT TENMH
              FROM MATHANG 
              WHERE MaMH = (SELECT TOP(1) MaMH FROM HOADON Group by MaMH Order by SUM(SoLuong) DESC)
             */
            var XeBanChay = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            tenXeBanChay = mdb.MatHangs.Where(i => i.MaMh == XeBanChay.IdXe).Select(i => i.TenMh).FirstOrDefault();
            txtblThgTinMHBanChay.Text = tenXeBanChay + "\nMã Mặt Hàng:\n" + XeBanChay.IdXe;
            //3 Dòng trên để tìm ra mặt hàng bán chạy nhất
            SoLgXeBanChay = XeBanChay.Tong;

            var NVNgSuat = mdb.HoaDons.GroupBy(u => u.MaNv).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdNv = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            var tenNVNgSuat = mdb.NhanViens.Where(i => i.MaNv == NVNgSuat.IdNv).Select(i => i.HoTenNv).FirstOrDefault();
            txtblThgTinNVNgSuat.Text = tenNVNgSuat + "\nMã Nhân Viên: " + NVNgSuat.IdNv;
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

            var KhVIP = mdb.HoaDons.GroupBy(u => u.MaKh).Select(u => new { Tong = u.Sum(u => u.ThanhTien), IdKhach = u.Key }).OrderByDescending(u => u.Tong).FirstOrDefault();
            var tenKhVIP = mdb.KhachHangs.Where(u => u.MaKh == KhVIP.IdKhach).Select(u => u.HoTenKh).FirstOrDefault();
            txtblThgTinKHVIP.Text = tenKhVIP + "\nMã Khách Hàng:\n" + KhVIP.IdKhach;
            //đồ thị check if DBNULL nếu null thì cho = 0
        }

        public SeriesCollection SrC { get; set; } = new(); //của biểu đồ Tròn
        public Func<ChartPoint, string> PointLabel { get; set; } //của biểu đồ Tròn
        public SeriesCollection SeriesColKhach { get; set; } = new();
        public List<string> LabelsKhach { get; set; } = new();

        public Func<decimal, string> Values { get; set; }

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

        private void bordertop5KH_MouseMove(object sender, MouseEventArgs e)
        {
            bordertop5KH.Opacity = 1;
        }

        private void bordertop5KH_MouseLeave(object sender, MouseEventArgs e)
        {
            bordertop5KH.Opacity = 0.8;
        }
    }
}
