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
using MotoStore.Views.Windows;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for PageBieuDo.xaml
    /// </summary>
    public partial class PageBieuDo : Page
    {
        private readonly MainDatabase mdb = new();
        private readonly SqlConnection con = new(Properties.Settings.Default.ConnectionString);
        private int luachon;
        private static bool CanClick = true;
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
            string lastdate = DateTime.Today.AddDays(-30.0).ToString("dd/MM/yyyy");
            for (DateTime date = DateTime.Today.AddDays(-29.0); date < DateTime.Now; date = date.AddDays(1.0))
            {
                SqlCommand cmd = new("Set dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD = @Today", con);
                //Bình Thường trên SQL sẽ là NgayLapHD = '@Today' nhưng ở Linq này kh được chứa ''
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
                /* if (date == DateTime.Parse(now)) //Ngày đầu
                     Labels.Add(date.ToString("d-M-yyyy"));
                 else if (date == DateTime.Parse(now).AddDays(-29.0))//date == DateTime.Parse(lastdate)) //Ngày cuối, có chút vấn đề ở đây
                     Labels.Add(date.ToString("d-M-yyyy"));
                 else if (date.Day == 1)
                     Labels.Add(date.ToString("d-M-yyyy"));
                 else if (int.Parse(date.ToString("d-M-yyyy")) == DateTime.DaysInMonth(date.Year, date.Month))
                     Labels.Add(date.ToString("d-M-yyyy"));
                 else //Ngày thường
                     Labels.Add(date.ToString("d-M-yyyy")); */
                Labels.Add(date.ToString("d-M-yyyy"));
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
            dothi.AxisY[0].MinValue = 0;
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

        private void subitemNamTrc_Click(object sender, RoutedEventArgs e) //ss 2 năm
        {
            /*Mỗi lần nhấn menu Năm Trước này, ta sẽ clear hết các trường
              dữ liệu cũ, clear luôn thanh chọn ngày xem(Nếu có)
              sau đó đổ lại dữ liệu vào.
             */
            gridChonNgay.Visibility = Visibility.Collapsed;
            lblZoomIn.Visibility = Visibility.Collapsed;
            lblNhapNam.Content = "Nhập 2 năm muốn so sánh:";
            dothi.ChartLegend.Visibility = Visibility.Visible;
            borderHuongDan.Visibility = Visibility.Collapsed;
            gridchonNam.Visibility = Visibility.Visible;
            namBa.Visibility = Visibility.Collapsed;
            luachon = 1;
            subitem2NamTrc.IsChecked = false;
            CanClick = false;
        }

        private void subitem2NamTrc_Click(object sender, RoutedEventArgs e)  //ss 3 năm
        {
            gridChonNgay.Visibility = Visibility.Collapsed;
            lblZoomIn.Visibility = Visibility.Collapsed;
            lblNhapNam.Content = "Nhập 3 năm muốn so sánh:";
            dothi.ChartLegend.Visibility = Visibility.Visible;
            borderHuongDan.Visibility = Visibility.Collapsed;
            gridchonNam.Visibility = Visibility.Visible;
            namBa.Visibility = Visibility.Visible;
            luachon = 2;
            subitemNamTrc.IsChecked = false;
            CanClick = false;
        }

        private void subitemChonNgayXem_Click(object sender, RoutedEventArgs e)
        {
            gridChonNgay.Visibility = Visibility.Visible;
            gridchonNam.Visibility = Visibility.Collapsed;
            //Hiện mục chọn ngày mỗi khi click vào menu Chọn Ngày Xem
        }
        public static bool IsValidDateTimeTest(string dateTime)
        {
            string[] formats = { "d/M/yyyy" };
            if (DateTime.TryParseExact(dateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                if (date < new DateTime(1900, 1, 1) || date > new DateTime(2079, 6, 6))
                    return false;
                return true;
            }
            return false;
            //Hàm kiểm tra ngày có hợp lệ hay không
        }

        private void txtTuNgay_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime ngaylonnhat = mdb.HoaDons.OrderByDescending(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault() ?? DateTime.Today;
            string maxDate = ngaylonnhat.ToString("dd/MM/yyyy");
            if (!IsValidDateTimeTest(txtTuNgay.Text))
            {
                MessageBox.Show("Ngày Không Hợp Lệ, hãy nhập ngày theo format(dd/MM/yyyy)!\nGiới hạn nằm trong khoảng từ 01/01/1900 đến 06/06/2079");
                e.Handled = true;
                txtTuNgay.Focus();
                return;
            }
            if (IsValidDateTimeTest(txtDenNgay.Text))
            {
                if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture))
                {
                    e.Handled = true;
                    MessageBox.Show("Từ Ngày không được phép lớn hơn hoặc bằng Đến Ngày, Hãy Nhập Lại!");
                    txtTuNgay.Focus();
                    return;
                }
            }
            if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) > ngaylonnhat)
                MessageBox.Show("Các ngày sau ngày " + maxDate + " CHƯA CÓ DỮ LIỆU, Doanh Thu mặc định của các ngày đó sẽ = 0");
        }

        private void txtDenNgay_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault() ?? DateTime.Today;
            string minDate = ngaynhonhat.ToString("dd/MM/yyyy");
            if (!IsValidDateTimeTest(txtDenNgay.Text))
            {
                MessageBox.Show("Ngày Không Hợp Lệ, hãy nhập ngày theo format(dd/MM/yyyy)!\nGiới hạn nằm trong khoảng từ 01/01/1900 đến 06/06/2079");
                e.Handled = true;
                txtDenNgay.Focus();
                return;
            }
            if (IsValidDateTimeTest(txtTuNgay.Text))
            {
                if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture))
                {
                    e.Handled = true;
                    MessageBox.Show("Từ Ngày không được phép lớn hơn hoặc bằng Đến Ngày, Hãy Nhập Lại!");
                    txtTuNgay.Focus();
                    return;
                }
            }
            if (DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) < ngaynhonhat)
                MessageBox.Show("Các ngày trước ngày " + minDate + " CHƯA CÓ DỮ LIỆU, Doanh Thu mặc định của các ngày đó sẽ = 0");
            /*Khi ô Đến Ngày LostFocus, ta sẽ check nó có phải ngày hợp lệ hay kh,
              và check xem nó có bé hơn hoặc = Từ Ngày hay kh
             */
        }

        private void btnXem_Click(object sender, RoutedEventArgs e)
        {
            lblSeries.Content = "Tháng";
            lblDTThgNay.Content = "So Sánh Doanh Thu(Đơn Vị: VNĐ)";
            /*Chỉ có duy nhất Lựa chọn hàm này là người dùng đc 
              phép Zoom, nên sẽ tìm MaxValue ở đây để tránh tình trạng
              Zoom quá mức nó sẽ ra giá trị Rác*/

            ChartValues<decimal> ChartVal = new();
            if (string.IsNullOrEmpty(txtTuNgay.Text) || string.IsNullOrEmpty(txtDenNgay.Text))
                MessageBox.Show("Vui Lòng Nhập Đầy Đủ 2 Ngày");
            else //Thoả mãn hết các điều kiện => được phép xem 
            {
                CanClick = true; //Cho phép bấm vào DataPoint
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

                for (DateTime date = DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture); date <= DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture); date = date.AddDays(1.0))
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
                    Labels.Add(date.ToString("d-M-yyyy"));
                   /* if (date == DateTime.Parse(txtTuNgay.Text))
                        Labels.Add(date.ToString("dd-M-yyyy")); //Ngày đầu của txtTuNgay(thêm NĂM sau đuôi)
                    else if (date.Day == 1)
                    {
                        if (date.Month != 1)
                            Labels.Add(date.ToString("dd-M")); //Ngày đầu tháng(Thêm tháng đằng sau)
                        else
                            Labels.Add(date.ToString("dd-M-yyyy")); //Ngày đầu tháng 1(Thêm năm đằng sau)
                    }
                    else if (int.Parse(date.ToString("dd")) == DateTime.DaysInMonth(date.Year, date.Month))
                        Labels.Add(date.ToString("dd-M-yyyy")); //Ngày cuối Tháng (Thêm tháng đằng sau)
                    else if (date == DateTime.Parse(txtDenNgay.Text))
                        Labels.Add(date.ToString("dd-M-yyyy")); //Ngày cuối của txtDenNgay(thêm NĂM sau đuôi)
                    else
                        Labels.Add(date.ToString("dd")); //Ngày thường
                    //Cần ngày đầu tháng */
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

                dothi.AxisY[0].MaxValue = (double)maxVal * 1.2;
                /*1 dòng trên để set max value trục tung cho đồ thị,
                  tránh nó nhận giá trị RÁC khi Zoom quá*/
                Values = value => value.ToString("N");
                lblZoomIn.Visibility = Visibility.Visible;
            }
        }

        private void btnXemNam_Click(object sender, RoutedEventArgs e)
        {
            if (luachon == 1)
            {
                if (string.IsNullOrEmpty(namNhat.Text) || string.IsNullOrEmpty(namHai.Text))
                    MessageBox.Show("Có trường dữ liệu rỗng, vui lòng kiểm tra lại!");
                else
                {
                    lblSeries.Content = "         Tháng";
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

                    decimal[] arrVal2 = new decimal[12];
                    decimal[] arrVal1 = new decimal[12];
                    decimal maxVal = 0;
                    string StartDate2;
                    string EndDate2;
                    string StartDate1;
                    string EndDate1;
                    /*Mảng doanh thu từng tháng của 2 năm 22 và 21,
                      cùng với đó là các tham số để truyền vào câu 
                      lệnh Query trên C#
                      */

                    con.Open(); //<Mở kết nối để đọc dữ liệu vào 2 mảng
                    for (int i = 1; i <= 12; i++)
                    {
                        StartDate2 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2 = i + 1;
                        if (i == 12)
                            EndDate2 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2 = "1/" + thangsau2 + "/" + namNhat.Text;

                        SqlCommand cmd = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2 and NgayLapHD < @EndDate2", con);
                        cmd.Parameters.Add("@StartDate2", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@StartDate2"].Value = StartDate2;
                        cmd.Parameters.Add("@EndDate2", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@EndDate2"].Value = EndDate2;

                        SqlDataReader sda = cmd.ExecuteReader();
                        if (sda.Read())
                        {
                            if (sda[0] != DBNull.Value)
                                arrVal2[i - 1] = (decimal)sda[0];
                            else
                                arrVal2[i - 1] = 0;
                        }

                        StartDate1 = "1/" + i + "/" + namHai.Text;
                        int thangsau1 = i + 1;
                        if (i == 12)
                            EndDate1 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate1 = "1/" + thangsau1 + "/" + namHai.Text;
                        SqlCommand cmd2 = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate1 and NgayLapHD < @EndDate1", con);
                        cmd2.Parameters.Add("@StartDate1", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@StartDate1"].Value = StartDate1;
                        cmd2.Parameters.Add("@EndDate1", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@EndDate1"].Value = EndDate1;
                        SqlDataReader sda2 = cmd2.ExecuteReader();
                        if (sda2.Read())
                        {
                            if (sda2[0] != DBNull.Value)
                                arrVal1[i - 1] = (decimal)sda2[0];
                            else
                                arrVal1[i - 1] = 0;
                        }
                    }
                    con.Close();

                    maxVal = arrVal2[0];
                    for (int j = 1; j < arrVal2.Length; j++)
                        if (arrVal2[j] > maxVal)
                            maxVal = arrVal2[j];
                    for (int j = 0; j < arrVal1.Length; j++)
                        if (arrVal1[j] > maxVal)
                            maxVal = arrVal1[j];

                    //Hàng dưới thêm dữ liệu vào đồ thị
                    SrC.Add(new ColumnSeries
                    {
                        Title = namNhat.Text,
                        Values = new ChartValues<decimal> { arrVal2[0], arrVal2[1], arrVal2[2], arrVal2[3], arrVal2[4], arrVal2[5], arrVal2[6], arrVal2[7], arrVal2[8], arrVal2[9], arrVal2[10], arrVal2[11] },
                        Fill = Brushes.Red
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = namHai.Text,
                        Values = new ChartValues<decimal> { arrVal1[0], arrVal1[1], arrVal1[2], arrVal1[3], arrVal1[4], arrVal1[5], arrVal1[6], arrVal1[7], arrVal1[8], arrVal1[9], arrVal1[10], arrVal1[11] },
                        Fill = Brushes.DeepSkyBlue
                    });

                    dothi.AxisY[0].MaxValue = (double)maxVal * 1.2;
                    Values = value => value.ToString("N");
                }
            }
            else //SS 3 nam
            {
                if (string.IsNullOrEmpty(namNhat.Text) || string.IsNullOrEmpty(namHai.Text) || string.IsNullOrEmpty(namBa.Text))
                    MessageBox.Show("Có trường dữ liệu rỗng, vui lòng kiểm tra lại!");
                else
                {
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

                    decimal[] arrVal2 = new decimal[12];
                    decimal[] arrVal1 = new decimal[12];
                    decimal[] arrVal2020 = new decimal[12];
                    decimal maxVal = 0;
                    string StartDate2;
                    string EndDate2;
                    string StartDate1;
                    string EndDate1;
                    string StartDate2020;
                    string EndDate2020;

                    con.Open();
                    for (int i = 1; i <= 12; i++)
                    {
                        StartDate2 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2 = i + 1;
                        if (i == 12)
                            EndDate2 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2 = "1/" + thangsau2 + "/" + namNhat.Text;

                        SqlCommand cmd = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2 and NgayLapHD <= @EndDate2", con);
                        cmd.Parameters.Add("@StartDate2", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@StartDate2"].Value = StartDate2;
                        cmd.Parameters.Add("@EndDate2", System.Data.SqlDbType.SmallDateTime);
                        cmd.Parameters["@EndDate2"].Value = EndDate2;

                        SqlDataReader sda = cmd.ExecuteReader();
                        if (sda.Read())
                        {
                            if (sda[0] != DBNull.Value)
                                arrVal2[i - 1] = (decimal)sda[0];
                            else
                                arrVal2[i - 1] = 0;
                        }

                        StartDate1 = "1/" + i + "/" + namHai.Text;
                        int thangsau1 = i + 1;
                        if (i == 12)
                            EndDate1 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate1 = "1/" + thangsau1 + "/" + namHai.Text;

                        SqlCommand cmd2 = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate1 and NgayLapHD < @EndDate1", con);
                        cmd2.Parameters.Add("@StartDate1", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@StartDate1"].Value = StartDate1;
                        cmd2.Parameters.Add("@EndDate1", System.Data.SqlDbType.SmallDateTime);
                        cmd2.Parameters["@EndDate1"].Value = EndDate1;
                        SqlDataReader sda2 = cmd2.ExecuteReader();
                        if (sda2.Read())
                        {
                            if (sda2[0] != DBNull.Value)
                                arrVal1[i - 1] = (decimal)sda2[0];
                            else
                                arrVal1[i - 1] = 0;
                        }

                        StartDate2020 = "1/" + i + "/" + namBa.Text;
                        int thangsau2020 = i + 1;
                        if (i == 12)
                            EndDate2020 = "1/1/" + (int.Parse(namBa.Text) + 1).ToString();
                        else
                            EndDate2020 = "1/" + thangsau2020 + "/" + namBa.Text;

                        SqlCommand cmd3 = new("SET Dateformat dmy\nSelect Sum(ThanhTien) from HoaDon where NgayLapHD >= @StartDate2020 and NgayLapHD < @EndDate2020", con);
                        cmd3.Parameters.Add("@StartDate2020", System.Data.SqlDbType.SmallDateTime);
                        cmd3.Parameters["@StartDate2020"].Value = StartDate2020;
                        cmd3.Parameters.Add("@EndDate2020", System.Data.SqlDbType.SmallDateTime);
                        cmd3.Parameters["@EndDate2020"].Value = EndDate2020;
                        SqlDataReader sda3 = cmd3.ExecuteReader();
                        if (sda3.Read())
                        {
                            if (sda3[0] != DBNull.Value)
                                arrVal2020[i - 1] = (decimal)sda3[0];
                            else
                                arrVal2020[i - 1] = 0;
                        }
                    }
                    con.Close();

                    maxVal = arrVal2[0];
                    for (int j = 1; j < arrVal2.Length; j++)
                        if (arrVal2[j] > maxVal)
                            maxVal = arrVal2[j];
                    for (int j = 0; j < arrVal1.Length; j++)
                        if (arrVal1[j] > maxVal)
                            maxVal = arrVal1[j];
                    for (int j = 0; j < arrVal2020.Length; j++)
                        if (arrVal2020[j] > maxVal)
                            maxVal = arrVal2020[j];

                    SrC.Add(new ColumnSeries
                    {
                        Title = namNhat.Text,
                        Values = new ChartValues<decimal> { arrVal2[0], arrVal2[1], arrVal2[2], arrVal2[3], arrVal2[4], arrVal2[5], arrVal2[6], arrVal2[7], arrVal2[8], arrVal2[9], arrVal2[10], arrVal2[11] },
                        Fill = Brushes.Red
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = namHai.Text,
                        Values = new ChartValues<decimal> { arrVal1[0], arrVal1[1], arrVal1[2], arrVal1[3], arrVal1[4], arrVal1[5], arrVal1[6], arrVal1[7], arrVal1[8], arrVal1[9], arrVal1[10], arrVal1[11] },
                        Fill = Brushes.DeepSkyBlue
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = namBa.Text,
                        Values = new ChartValues<decimal> { arrVal2020[0], arrVal2020[1], arrVal2020[2], arrVal2020[3], arrVal2020[4], arrVal2020[5], arrVal2020[6], arrVal2020[7], arrVal2020[8], arrVal2020[9], arrVal2020[10], arrVal2020[11] },
                        Fill = Brushes.Green
                    });
                    dothi.AxisY[0].MaxValue = (double)maxVal * 1.2;
                    Values = value => value.ToString("N");
                }                   
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

        private void dothi_DataClick(object sender, ChartPoint chartPoint)
        {
            string getNgay = TrucHoanhX.Labels[(int)chartPoint.X];
            WindowInformation wd = new WindowInformation(getNgay);
            wd.ShowDialog();
        }

        // Dùng cho 2 textbox namNhat và namHai để giới hạn số năm nhập đc
        private void TextBoxNam_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tbx)
            {
                if (int.TryParse(tbx.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var i))
                {
                    if (i < 1900)
                    {
                        MessageBox.Show("Năm nhỏ nhất là 1900!");
                        e.Handled = true;
                        tbx.Focus();
                        return;
                    }
                    if (i > 2078)
                    {
                        MessageBox.Show("Năm lớn nhất là 2078!");
                        e.Handled = true;
                        tbx.Focus();
                        return;
                    }    
                }
                else
                {
                    MessageBox.Show("Năm lớn nhất là 2078!");
                    e.Handled = true;
                    tbx.Focus();
                    return;
                }
            }
        }
    }
}
