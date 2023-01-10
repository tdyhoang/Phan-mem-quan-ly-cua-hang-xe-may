using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for PageChiTiet.xaml
    /// </summary>
    public partial class PageChiTiet : Page
    {
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        private MainDatabase mdb = new();
        public PageChiTiet()
        {
            InitializeComponent();
            PointLabel = chartPoint =>
            string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            //Lấy ra top 5 sp bán chạy
            
            SrC.Add(new PieSeries()
            {
                Title = ReportPage.tenXeBanChay,
                Values = new ChartValues<int> { ReportPage.SoLgXeBanChay },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill=Brushes.Red
            });

           /* var query = from item in mdb.HoaDons
                        group item by item.MaMh into g
                        orderby g.Sum(x => x.SoLuong) descending
                        select g;
            var topFive = query.Take(5); */

            var XeBanChay2 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(1).Take(1).Single();
            var tenXeBanChay2= mdb.MatHangs.Where(i => i.MaMh == XeBanChay2.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay2,
                Values = new ChartValues<int> { XeBanChay2.Tong.Value },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill=Brushes.Purple         
            });

            var XeBanChay3 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(2).Take(1).Single();
            var tenXeBanChay3 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay3.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay3,
                Values = new ChartValues<int> { XeBanChay3.Tong.Value },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.SkyBlue
            });

            var XeBanChay4 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(3).Take(1).Single();
            var tenXeBanChay4 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay4.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay4,
                Values = new ChartValues<int> { XeBanChay4.Tong.Value },
                LabelPoint = PointLabel,
                DataLabels = true,
                Fill = Brushes.Orange
            });

            var XeBanChay5 = mdb.HoaDons.GroupBy(u => u.MaMh).Select(u => new { Tong = u.Sum(u => u.SoLuong), IdXe = u.Key }).OrderByDescending(u => u.Tong).Skip(4).Take(1).Single();
            var tenXeBanChay5 = mdb.MatHangs.Where(i => i.MaMh == XeBanChay5.IdXe).Select(i => i.TenMh).FirstOrDefault();

            SrC.Add(new PieSeries()
            {
                Title = tenXeBanChay5,
                Values = new ChartValues<int> { XeBanChay5.Tong.Value },
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
            for(int i=0;i<5;i++)
            {
                SqlCommand cmd = new("SELECT MaKH FROM  HoaDon Group by MaKH ORDER BY sum(ThanhTien) desc OFFSET @i ROWS FETCH NEXT 1 ROWS ONLY;", con);
                cmd.Parameters.Add("@i", System.Data.SqlDbType.Int);
                cmd.Parameters["@i"].Value = i;
                SqlDataReader sda = cmd.ExecuteReader();
                if (sda.Read())
                    if (sda[0] != DBNull.Value)
                        arrMaKhach[i] = (string)sda[0];

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
        }

        public SeriesCollection SrC { get; set; } = new(); //của biểu đồ Tròn
        public Func<ChartPoint, string> PointLabel { get; set; } //của biểu đồ Tròn
        public SeriesCollection SeriesColKhach { get; set; } = new();
        public List<string> LabelsKhach { get; set; } = new();

        public Func<decimal, string> Values { get; set; }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }

        private void borderBieuDo_MouseMove(object sender, MouseEventArgs e)
        {
            borderBieuDo.Opacity = 1;
        }

        private void borderBieuDo_MouseLeave(object sender, MouseEventArgs e)
        {
            borderBieuDo.Opacity = 0.85;
        }

        private void brdRankKH_MouseMove(object sender, MouseEventArgs e)
        {
            brdRankKH.Opacity = 1;
        }

        private void brdRankKH_MouseLeave(object sender, MouseEventArgs e)
        {
            brdRankKH.Opacity = 0.85;
        }
    }
}
