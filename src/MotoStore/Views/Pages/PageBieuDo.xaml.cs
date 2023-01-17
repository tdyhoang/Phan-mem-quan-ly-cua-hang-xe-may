using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using MotoStore.Database;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for PageBieuDo.xaml
    /// </summary>
    public partial class PageBieuDo : Page
    {
        private readonly MainDatabase mdb = new();
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        public PageBieuDo()
        {
            InitializeComponent();
            SrC = new();
            dothi.ChartLegend.Visibility = Visibility.Collapsed;
            gridChonNgay.Visibility = Visibility.Collapsed;
            MenuTuChon.Visibility = Visibility.Collapsed;

            decimal[] arrDoanhThu = new decimal[1000];
            ChartValues<decimal> ListDoanhThu = new();
            Labels = new();
            con.Open();
            //Nên đổi doanh thu Tháng Này thành doanh thu 30 ngày gần nhất
            //để lúc nào nó cũng luôn có dữ liệu
            string now = DateTime.Now.ToString("dd/MM/yyyy");
            string lastdate = DateTime.Now.AddDays(-30.0).ToString("dd/MM/yyyy");
            for (DateTime date = DateTime.Parse(now).AddDays(-29.0); date < DateTime.Now; date = date.AddDays(1.0))
            {
                SqlCommand cmd = new("Set dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD = @Today", con);
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
                if (date == DateTime.Parse(now)) //Ngày đầu
                    Labels.Add(date.ToString("d-M-yyyy"));
                else if (date == DateTime.Parse(now).AddDays(-29.0))//date == DateTime.Parse(lastdate)) //Ngày cuối, có chút vấn đề ở đây
                    Labels.Add(date.ToString("d-M-yyyy"));
                else if (date.Day == 1)
                    Labels.Add(date.ToString("d-M"));
                else if (int.Parse(date.ToString("dd")) == DateTime.DaysInMonth(date.Year, date.Month))
                    Labels.Add(date.ToString("d-M"));
                else //Ngày thường
                    Labels.Add(date.ToString("dd"));
            }
                    
            SrC.Add(new LineSeries
            {
                Title= "VNĐ",
                Values = ListDoanhThu,
                Stroke = Brushes.White,
                StrokeThickness=2,
                Fill = null
            }); 
            con.Close();  
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

        static int solanbam = 0; //biến dùng để ẩn và hiện menu Tự Chọn
        private void btnTuChon_Click(object sender, RoutedEventArgs e)
        {
            if (solanbam % 2 == 0)
                MenuTuChon.Visibility = Visibility.Visible;
            else
                MenuTuChon.Visibility = Visibility.Collapsed;
            solanbam++;
            gridChonNgay.Visibility = Visibility.Collapsed;
        }

        private void subitemNamTrc_Click(object sender, RoutedEventArgs e)
        {
            /*Mỗi lần nhấn menu Năm Trước này, ta sẽ clear hết các trường
              dữ liệu cũ, clear luôn thanh chọn ngày xem(Nếu có)
              sau đó đổ lại dữ liệu vào.
             */
            gridChonNgay.Visibility = Visibility.Collapsed;
            lblZoomIn.Visibility = Visibility.Collapsed;
            lblSeries.Content = "Tháng";
            lblDTThgNay.Content = "So Sánh Doanh Thu(Đơn Vị: VNĐ)";
            dothi.ChartLegend.Visibility = Visibility.Visible;
            borderHuongDan.Visibility = Visibility.Collapsed;

            while (dothi.Series.Count > 0) 
                dothi.Series.RemoveAt(0);  //Clear dữ liệu cũ
            Labels.Clear();  //Clear Nhãn cũ

            for (int i = 1; i <= 12; i++)
                Labels.Add(i.ToString()); //Đổ 12 tháng vào Nhãn
            dothi.AxisX[0].MinValue = 0;  //kết thúc Zoom hiện tại(nếu có), trả về Zoom vốn có
            dothi.AxisX[0].MaxValue = 12; //.....

            dothi.FontSize = 20;
            TrucHoanhX.FontSize = 20;
            dothi.Zoom = ZoomingOptions.None; //Không cho Zoom
            dothi.Pan = PanningOptions.None;  //Không cho Pan(Lia đồ thị)
            TrucHoanhX.Separator.Step = 1; //Set step Trục hoành = 1 để nhìn rõ 12 Tháng

            decimal[] arrVal2022 = new decimal[12];
            decimal[] arrVal2021 = new decimal[12];
            string StartDate2022;
            string EndDate2022;
            string StartDate2021;
            string EndDate2021;
            /*Mảng doanh thu từng tháng của 2 năm 22 và 21,
              cùng với đó là các tham số để truyền vào câu 
              lệnh Query trên C#
              */

            con.Open(); //<Mở kết nối để đọc dữ liệu vào 2 mảng
            for (int i = 1; i <= 12; i++)
            {
                StartDate2022 = "1/" + i + "/2022";
                int thangsau2022 = i + 1;
                if (i == 12)
                    EndDate2022 = "1/1/2023";
                else
                    EndDate2022 = "1/" + thangsau2022 + "/2022";

                SqlCommand cmd = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2022 and NgayLapHD < @EndDate2022", con);
                cmd.Parameters.Add("@StartDate2022", System.Data.SqlDbType.SmallDateTime);
                cmd.Parameters["@StartDate2022"].Value = StartDate2022;
                cmd.Parameters.Add("@EndDate2022", System.Data.SqlDbType.SmallDateTime);
                cmd.Parameters["@EndDate2022"].Value = EndDate2022;

                SqlDataReader sda = cmd.ExecuteReader();
                if (sda.Read())
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
                SqlCommand cmd2 = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2021 and NgayLapHD < @EndDate2021", con);
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

            //Hàng dưới thêm dữ liệu vào đồ thị
            SrC.Add(new ColumnSeries
            {
                Title = "2021",
                Values = new ChartValues<decimal> { arrVal2021[0], arrVal2021[1], arrVal2021[2], arrVal2021[3], arrVal2021[4], arrVal2021[5], arrVal2021[6], arrVal2021[7], arrVal2021[8], arrVal2021[9], arrVal2021[10], arrVal2021[11] },
                Fill = Brushes.DeepSkyBlue
            });
            SrC.Add(new ColumnSeries
            {
                Title = "2022",
                Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                Fill = Brushes.Red
            });

            Values = value => value.ToString("N");
        }

        private void subitem2NamTrc_Click(object sender, RoutedEventArgs e)
        {
            gridChonNgay.Visibility = Visibility.Collapsed;
            lblZoomIn.Visibility = Visibility.Collapsed;
            lblSeries.Content = "Tháng";
            lblDTThgNay.Content = "So Sánh Doanh Thu(Đơn Vị: VNĐ)";
            dothi.ChartLegend.Visibility = Visibility.Visible;
            borderHuongDan.Visibility = Visibility.Collapsed;

            while (dothi.Series.Count > 0)
                dothi.Series.RemoveAt(0);
            Labels.Clear();
            for (int i = 1; i <= 12; i++)
                Labels.Add(i.ToString());
            dothi.AxisX[0].MinValue = 0;
            dothi.AxisX[0].MaxValue = 12;

            dothi.FontSize = 20;
            TrucHoanhX.FontSize = 20;
            dothi.Zoom = ZoomingOptions.None;
            dothi.Pan = PanningOptions.None;
            TrucHoanhX.Separator.Step = 1;

            decimal[] arrVal2022 = new decimal[12];
            decimal[] arrVal2021 = new decimal[12];
            string StartDate2022;
            string EndDate2022;
            string StartDate2021;
            string EndDate2021;

            con.Open();
            for (int i = 1; i <= 12; i++)
            {
                StartDate2022 = "1/" + i + "/2022";
                int thangsau2022 = i + 1;
                if (i == 12)
                    EndDate2022 = "1/1/2023";
                else
                    EndDate2022 = "1/" + thangsau2022 + "/2022";

                SqlCommand cmd = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2022 and NgayLapHD <= @EndDate2022", con);
                cmd.Parameters.Add("@StartDate2022", System.Data.SqlDbType.SmallDateTime);
                cmd.Parameters["@StartDate2022"].Value = StartDate2022;
                cmd.Parameters.Add("@EndDate2022", System.Data.SqlDbType.SmallDateTime);
                cmd.Parameters["@EndDate2022"].Value = EndDate2022;

                SqlDataReader sda = cmd.ExecuteReader();
                if (sda.Read())
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
                SqlCommand cmd2 = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2021 and NgayLapHD < @EndDate2021", con);
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
                Fill = Brushes.DeepSkyBlue
            });
            SrC.Add(new ColumnSeries
            {
                Title = "2022",
                Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                Fill = Brushes.Red
            });

            //Add dữ liệu 2020 ở phía dưới
            /*SrC.Add(new ColumnSeries
            {
                Title = "2020",
                Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                Fill = Brushes.Green
            });*/
        }

        private void subitemChonNgayXem_Click(object sender, RoutedEventArgs e)
        {
            gridChonNgay.Visibility = Visibility.Visible;
            //Hiện mục chọn ngày mỗi khi click vào menu Chọn Ngày Xem
        }

        private void txtTuNgay_TextChanged(object sender, TextChangedEventArgs e)
        {
            for(int i=0;i<txtTuNgay.Text.Length;i++)
            {
                if (!(txtTuNgay.Text[i] >= 47 && txtTuNgay.Text[i] <= 57))
                {
                    MessageBox.Show("Ô Từ Ngày chứa kí tự không hợp lệ!");
                    txtTuNgay.Text = txtTuNgay.Text.Substring(0, txtTuNgay.Text.Length - 1);
                    txtTuNgay.SelectionStart = txtTuNgay.Text.Length;
                    break;
                }
            }
            //Hàm này kiểm tra ô Textbox Từ Ngày có phải ngày hợp lệ
        }
        public bool IsValidDateTimeTest(string dateTime)
        {
            string[] formats = { "d/M/yyyy" };
            return DateTime.TryParseExact(dateTime, formats, new CultureInfo("vi-VN"),
                                           DateTimeStyles.None, out _);
            //Hàm kiểm tra ngày có hợp lệ hay không
        }

        private void txtTuNgay_LostFocus(object sender, RoutedEventArgs e)
        {
            string ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault().ToString();
            if (!IsValidDateTimeTest(txtTuNgay.Text))
            {
                MessageBox.Show("Ô Từ Ngày Chứa Ngày Không Hợp Lệ, hãy nhập ngày theo format(ngày/tháng/năm)!");
                txtTuNgay.Clear();
            }
            else if (DateTime.Parse(txtTuNgay.Text) > DateTime.Now)
            {
                MessageBox.Show("Chưa có dữ liệu ở ngày này, hãy nhập lại");
                txtTuNgay.Clear();
            }
            else if (DateTime.Parse(txtTuNgay.Text) < DateTime.Parse(ngaynhonhat))
            {
                MessageBox.Show("Chưa có dữ liệu ở ngày này, hãy nhập lại");
                txtTuNgay.Clear();
            }
            //Nếu ô Từ Ngày bị LostFocus mà trong ô đó chứa ngày kh hợp lệ thì xoá text ô đó
        }

        private void txtDenNgay_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < txtDenNgay.Text.Length; i++)
                if (!(txtDenNgay.Text[i] >= 47 && txtDenNgay.Text[i] <= 57))
                {
                    MessageBox.Show("Ô Đến Ngày chứa kí tự không hợp lệ!");
                    txtDenNgay.Text = txtDenNgay.Text.Substring(0, txtDenNgay.Text.Length - 1);
                    txtDenNgay.SelectionStart = txtDenNgay.Text.Length;
                    break;
                }
            /*Hàm này sẽ kiểm tra ô Textbox Đến Ngày có phải ngày hợp lệ
              trước khi bấm Xem. Kí tự '-' kh đc coi là 1 phần của ngày hợp lệ*/
        }

        private void txtDenNgay_LostFocus(object sender, RoutedEventArgs e)
        {
            string ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault().ToString();
            if (!IsValidDateTimeTest(txtDenNgay.Text))
            {
                MessageBox.Show("Ô Đến Ngày Chứa Ngày Không Hợp Lệ, Hãy Nhập Ngày Theo Format(Ngày/Tháng/Năm)!");
                txtDenNgay.Clear();
            }
            else if (DateTime.Parse(txtTuNgay.Text) >= DateTime.Parse(txtDenNgay.Text))
            {
                MessageBox.Show("Từ Ngày không được phép lớn hơn hoặc bằng Đến Ngày, Hãy Nhập Lại!");
                txtDenNgay.Clear();
            }
            else if (DateTime.Parse(txtDenNgay.Text) > DateTime.Now)
            {
                MessageBox.Show("Chưa có dữ liệu ở ô đến ngày, hãy nhập lại");
                txtDenNgay.Clear();
            }
            else if (DateTime.Parse(txtDenNgay.Text) < DateTime.Parse(ngaynhonhat))
            {
                MessageBox.Show("Chưa có dữ liệu ở ngày này, hãy nhập lại");
                txtDenNgay.Clear();
            }
            /*Khi ô Đến Ngày LostFocus, ta sẽ check nó có phải ngày hợp lệ hay kh,
              và check xem nó có bé hơn hoặc = Từ Ngày hay kh
             */
        }

        private void btnXem_Click(object sender, RoutedEventArgs e)
        {
            /*Chỉ có duy nhất Lựa chọn hàm này là người dùng đc 
              phép Zoom, nên sẽ tìm MaxValue ở đây để tránh tình trạng
              Zoom quá mức nó sẽ ra giá trị Rác*/

            ChartValues<decimal> ChartVal = new();
            if (string.IsNullOrEmpty(txtTuNgay.Text) || string.IsNullOrEmpty(txtDenNgay.Text))
                MessageBox.Show("Vui Lòng Nhập Đầy Đủ 2 Ngày");
            else //Thoả mãn hết các điều kiện => được phép xem 
            {
                while (dothi.Series.Count > 0)
                    dothi.Series.RemoveAt(0);
                Labels.Clear();
                /*3 dòng trên để Clear hết các dữ liệu cũ,
                  Dọn chỗ cho dữ liệu mới*/

                lblSeries.Content = "         Ngày";
                dothi.Zoom = ZoomingOptions.X; //Cho phép Zoom trục hoành
                dothi.Pan = PanningOptions.X;  //Cho phép Lia trục hoành
                dothi.FontSize = 15;
                TrucHoanhX.FontSize = 12;
                con.Open();

                for (DateTime date = DateTime.Parse(txtTuNgay.Text); date <= DateTime.Parse(txtDenNgay.Text); date = date.AddDays(1.0))
                {
                    SqlCommand cmd = new("Select Sum(ThanhTien) from HoaDon where NgayLapHD = @Today", con);
                    cmd.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                    cmd.Parameters["@Today"].Value = date;
                    SqlDataReader sda = cmd.ExecuteReader();
                    if (sda.Read())
                    {
                        if (sda[0] != DBNull.Value)
                            ChartVal.Add((decimal)sda[0]);
                        else
                            ChartVal.Add(0);
                    }
                    if (date == DateTime.Parse(txtTuNgay.Text))
                        Labels.Add(date.ToString("d-M-yyyy")); //Ngày đầu của txtTuNgay(thêm NĂM sau đuôi)
                    else if (date.Day == 1)
                    {
                        if (date.Month != 1)
                            Labels.Add(date.ToString("d-M")); //Ngày đầu tháng(Thêm tháng đằng sau)
                        else
                            Labels.Add(date.ToString("d-M-yyyy")); //Ngày đầu tháng 1(Thêm năm đằng sau)
                    }
                    else if (int.Parse(date.ToString("dd")) == DateTime.DaysInMonth(date.Year, date.Month))
                        Labels.Add(date.ToString("d-M-yyyy")); //Ngày cuối Tháng (Thêm tháng đằng sau)
                    else if (date == DateTime.Parse(txtDenNgay.Text))
                        Labels.Add(date.ToString("d-M-yyyy")); //Ngày cuối của txtDenNgay(thêm NĂM sau đuôi)
                    else
                        Labels.Add(date.Day.ToString()); //Ngày thường
                    //Cần ngày đầu tháng
                }
                con.Close();
                SrC.Add(new LineSeries
                {
                    Title = "VNĐ",
                    Values = ChartVal,
                    Stroke = Brushes.White,
                    StrokeThickness = 2,
                    Fill = null
                });

                 decimal maxVal = ChartVal[0];
                 for (int i = 1; i < ChartVal.Count; i++)
                     if (ChartVal[i] > maxVal)
                         maxVal = ChartVal[i];

                 dothi.AxisX[0].MinValue = 0;  
                 dothi.AxisX[0].MaxValue = ChartVal.Count;
                //kết thúc Zoom hiện tại(nếu có), trả về Zoom ban đầu
                TrucHoanhX.Separator.Step = 1; //Đặt giá trị của step mặc định là 1
                 if ((double)ChartVal.Count / 30 - ChartVal.Count / 30 > 0)
                     TrucHoanhX.Separator.Step = ChartVal.Count / 30 + 1;
                 else if((double)ChartVal.Count / 30 - ChartVal.Count / 30 == 0)
                     TrucHoanhX.Separator.Step = ChartVal.Count / 30;
                //ĐK if else ở trên để tăng bước trục hoành dựa vào khoảng ngày
                //0<NGÀY<30: step = 1, 30<NGÀY<60: step = 2 , ...  

                dothi.AxisY[0].MaxValue = (double)maxVal * 1.1;
                /*1 dòng trên để set max value trục tung cho đồ thị,
                  tránh nó nhận giá trị RÁC khi Zoom quá*/
                Values = value => value.ToString("N");
                lblZoomIn.Visibility = Visibility.Visible;
            }
        }

        private void TrucHoanhX_PreviewRangeChanged(LiveCharts.Events.PreviewRangeChangedEventArgs eventArgs)
        {
            //if less than -0.5, cancel
            if (eventArgs.PreviewMinValue < -0.5) eventArgs.Cancel = true;

            //if greater than the number of items on our array plus a 0.5 offset, stay on max limit
            //if (eventArgs.PreviewMaxValue > dothi.Series.Count - 0.5) eventArgs.Cancel = true;

            //finally if the axis range is less than 1, cancel the event
            if (eventArgs.PreviewMaxValue - eventArgs.PreviewMinValue < 1) eventArgs.Cancel = true;

            /*Hàm Sự Kiện này để hạn chế ng dùng 
              Zoom vào khoảng thời gian nằm ngoài
              2 cái Textbox Từ ngày và Đến ngày,
              Tuy nhiên vẫn còn chút lỗi hiển thị
             */
        }

        private void btnChamHoi_Click(object sender, RoutedEventArgs e)
        {
            borderHuongDan.Visibility = Visibility.Visible;
        }

        private void btnUnderstand_Click(object sender, RoutedEventArgs e)
        {
            borderHuongDan.Visibility = Visibility.Collapsed;
        }
    }
}
