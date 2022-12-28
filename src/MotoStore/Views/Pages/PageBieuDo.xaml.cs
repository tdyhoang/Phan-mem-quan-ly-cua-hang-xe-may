using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
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
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for PageBieuDo.xaml
    /// </summary>
    public partial class PageBieuDo : Page
    {
        private MainDatabase mdb = new MainDatabase();
        private SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        public PageBieuDo()
        {
            InitializeComponent();
            SrC = new SeriesCollection();


            //DateTime ngaymin = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault().Value;
            //string strNgayMin = ngaymin.ToString("dd/MM/yyyy");
            //decimal[] arrDoanhThu = new decimal[1000];

            List<decimal> ListDoanhThu = new List<decimal>();

            //con.Open();
           /* int i = 0;

               for (DateTime date = DateTime.Parse("1/12/2022"); date<=DateTime.Today; date = date.AddDays(1.0))
               {
                   SqlCommand cmd = new SqlCommand("Select Sum(ThanhTien) from HoaDon where NgayLapHD = @Today",con);
                   cmd.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                   cmd.Parameters["@Today"].Value = date;
                   SqlDataReader sda = cmd.ExecuteReader();
                   if (sda.Read())
                   {
                    if (sda[0] != DBNull.Value)
                        ListDoanhThu.Add((decimal)sda[0]);
                    else
                        ListDoanhThu.Add(0);
                   }
               }   */

            ChartValues<decimal> columnData = new ChartValues<decimal>();
            Labels = new();
            
            using (con)
            {
                con.Open();
                //string dưới t thêm thủ công, ngày mua hàng xa nhất tính tới giờ là 29/1/2021
                string query = "SET Dateformat dmy\ndeclare @fromdate date = '1/12/2022'; \r\ndeclare @thrudate date = getdate();\r\nwith n as (select n from (values(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) t(n)), dates as\r\n(\r\n\tselect top (datediff(day, @fromdate, @thrudate)+1)\r\n\t[Date]=convert(date,dateadd(day,row_number() over(order by (select 1))-1,@fromdate))\r\n\tfrom n as deka cross join n as hecto cross join n as kilo cross join n as tenK cross join n as hundredK\r\n\torder by [Date]\r\n)\r\n\r\nselect Date, sum(ThanhTien) as DoanhThu\r\nfrom dates d left join HoaDon HD on d.Date = HD.NgayLapHD\r\nwhere Date between @fromdate and @thrudate\r\ngroup by Date";
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    //SqlDataAdapter adapter = new SqlDataAdapter(query,con);
                    //adapter.SelectCommand.CommandTimeout = 60;
                    // command.CommandTimeout = 180; nếu bị timeout thì thử thêm dòng này hoặc đóng mở kết nối con hợp lý
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Labels.Add(reader.GetDateTime(0).Day.ToString());
                            if (reader[1] == DBNull.Value)
                                columnData.Add(0);
                            else
                                columnData.Add(reader.GetDecimal(1));
                        }
                    }
                }
            }
            con.Close();
            SrC.Add(new LineSeries
            {
                Values = columnData,
                //Doanh thu tháng này
                Fill = null
            }); 
            //con.Close();  
            Values = value => value.ToString("N");
            DataContext = this;            
        }
        
        public SeriesCollection SrC { get; set; }   
        public List<string> Labels { get; set; }
        public Func<decimal,string>Values { get; set; }
        

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }

        private void cbxChonNam_DropDownClosed(object sender, EventArgs e)
        {
            con.Close();
            if(cbxChonNam.SelectedItem != null)
            {
                lblSeries.Content = "Năm";
                while (dothi.Series.Count > 0) { dothi.Series.RemoveAt(0); }
                if (cbxChonNam.SelectedIndex == 0)
                {
                    decimal[] arrVal2022 = new decimal[12];
                    decimal[] arrVal2021 = new decimal[12];
                    string StartDate2022;
                    string EndDate2022;

                    string StartDate2021;
                    string EndDate2021;
                    con.Open();
                    for (int i=1;i<=12;i++)
                    {
                        StartDate2022 = "1/" + i + "/2022";
                        int thangsau2022 = i + 1;
                        if (i == 12)
                            EndDate2022 = "1/1/2023";
                        else
                            EndDate2022 = "1/" + thangsau2022 + "/2022";

                        SqlCommand cmd = new SqlCommand("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2022 and NgayLapHD <= @EndDate2022", con);
                        cmd.Parameters.Add("@StartDate2022", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@StartDate2022"].Value = StartDate2022;
                        cmd.Parameters.Add("@EndDate2022", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@EndDate2022"].Value = EndDate2022;
                        
                        SqlDataReader sda = cmd.ExecuteReader();
                        if(sda.Read())
                        {
                            if (sda[0] != DBNull.Value)
                                arrVal2022[i - 1] = (decimal)sda[0];
                            else
                                arrVal2022[i - 1] = 0;
                        }

                        StartDate2021 = "1/" + i + "/2021";
                        int thangsau2021 = i + 1;
                        if (i == 12)
                            EndDate2021 = "1/1/2022";
                        else
                            EndDate2021 = "1/" + thangsau2021 + "/2021";
                        SqlCommand cmd2 = new SqlCommand("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2021 and NgayLapHD < @EndDate2021", con);
                        cmd2.Parameters.Add("@StartDate2021", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@StartDate2021"].Value = StartDate2021;
                        cmd2.Parameters.Add("@EndDate2021", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@EndDate2021"].Value = EndDate2021;
                        SqlDataReader sda2 = cmd2.ExecuteReader();
                        if (sda2.Read())
                        {
                            if (sda2[0] != DBNull.Value)
                                arrVal2021[i - 1] = (decimal)sda2[0];
                            else
                                arrVal2021[i - 1] = 0;
                        }
                    }
                    con.Close();

                    SrC.Add(new ColumnSeries
                    {
                        Title = "2021",
                        Values = new ChartValues<decimal> { arrVal2021[0], arrVal2021[1], arrVal2021[2], arrVal2021[3], arrVal2021[4], arrVal2021[5], arrVal2021[6], arrVal2021[7], arrVal2021[8], arrVal2021[9], arrVal2021[10], arrVal2021[11] },
                        Fill = Brushes.Blue
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = "2022",
                        Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                        Fill = Brushes.Red
                    });
                }               
            }
        }

        static int solanbam = 0;
        private void btnSSDoanhThu_Click(object sender, RoutedEventArgs e)
        {
            if (solanbam % 2 == 0)
                cbxChonNam.Visibility = Visibility.Visible;
            else
                cbxChonNam.Visibility = Visibility.Collapsed;
            solanbam++;
        }
    }
}
