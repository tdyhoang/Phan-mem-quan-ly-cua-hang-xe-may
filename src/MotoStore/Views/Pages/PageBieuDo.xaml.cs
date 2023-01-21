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
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
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
            string now = DateTime.Now.ToString("dd/MM/yyyy");
            string lastdate = DateTime.Now.AddDays(-30.0).ToString("dd/MM/yyyy");
            for (DateTime date = DateTime.Parse(now).AddDays(-29.0); date < DateTime.Now; date = date.AddDays(1.0))
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
            DateTime ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault().Value;
            string minDate = ngaynhonhat.ToString("d-MM-yyyy");
            if (!IsValidDateTimeTest(txtTuNgay.Text))
            {
                MessageBox.Show("Ô Từ Ngày Chứa Ngày Không Hợp Lệ, hãy nhập ngày theo format(ngày/tháng/năm)!");
                txtTuNgay.Clear();
            }
            else if (DateTime.Parse(txtTuNgay.Text) > DateTime.Now)
                MessageBox.Show("Ô Từ Ngày CHƯA CÓ DỮ LIỆU, Doanh Thu mặc định từ ngày " + txtTuNgay.Text + " trở đi sẽ = 0");
            else if (DateTime.Parse(txtTuNgay.Text) < DateTime.Parse(minDate))
                MessageBox.Show("Các ngày trước ngày " + minDate + " CHƯA CÓ DỮ LIỆU, Doanh Thu mặc định của các ngày đó sẽ = 0");
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
            DateTime ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault().Value;
            string minDate = ngaynhonhat.ToString("d-MM-yyyy");
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
                MessageBox.Show("Chưa có dữ liệu ở Ô Đến Ngày, Doanh Thu mặc định từ ngày " + DateTime.Now.ToString("d-MM-yyyy") + " đến " + txtDenNgay.Text + " sẽ = 0");
            else if (DateTime.Parse(txtDenNgay.Text) < DateTime.Parse(minDate))
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

                    decimal[] arrVal2022 = new decimal[12];
                    decimal[] arrVal2021 = new decimal[12];
                    decimal maxVal = 0;
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
                        StartDate2022 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2022 = i + 1;
                        if (i == 12)
                            EndDate2022 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2022 = "1/" + thangsau2022 + "/" + namNhat.Text;

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

                        StartDate2021 = "1/" + i + "/" + namHai.Text;
                        int thangsau2021 = i + 1;
                        if (i == 12)
                            EndDate2021 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate2021 = "1/" + thangsau2021 + "/" + namHai.Text;
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

                    maxVal = arrVal2022[0];
                    for (int j = 1; j < arrVal2022.Length; j++)
                        if (arrVal2022[j] > maxVal)
                            maxVal = arrVal2022[j];
                    for (int j = 0; j < arrVal2021.Length; j++)
                        if (arrVal2021[j] > maxVal)
                            maxVal = arrVal2021[j];

                    //Hàng dưới thêm dữ liệu vào đồ thị
                    SrC.Add(new ColumnSeries
                    {
                        Title = namNhat.Text,
                        Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                        Fill = Brushes.Red
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = namHai.Text,
                        Values = new ChartValues<decimal> { arrVal2021[0], arrVal2021[1], arrVal2021[2], arrVal2021[3], arrVal2021[4], arrVal2021[5], arrVal2021[6], arrVal2021[7], arrVal2021[8], arrVal2021[9], arrVal2021[10], arrVal2021[11] },
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

                    decimal[] arrVal2022 = new decimal[12];
                    decimal[] arrVal2021 = new decimal[12];
                    decimal[] arrVal2020 = new decimal[12];
                    decimal maxVal = 0;
                    string StartDate2022;
                    string EndDate2022;
                    string StartDate2021;
                    string EndDate2021;
                    string StartDate2020;
                    string EndDate2020;

                    con.Open();
                    for (int i = 1; i <= 12; i++)
                    {
                        StartDate2022 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2022 = i + 1;
                        if (i == 12)
                            EndDate2022 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2022 = "1/" + thangsau2022 + "/" + namNhat.Text;

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

                        StartDate2021 = "1/" + i + "/" + namHai.Text;
                        int thangsau2021 = i + 1;
                        if (i == 12)
                            EndDate2021 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate2021 = "1/" + thangsau2021 + "/" + namHai.Text;

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

                    maxVal = arrVal2022[0];
                    for (int j = 1; j < arrVal2022.Length; j++)
                        if (arrVal2022[j] > maxVal)
                            maxVal = arrVal2022[j];
                    for (int j = 0; j < arrVal2021.Length; j++)
                        if (arrVal2021[j] > maxVal)
                            maxVal = arrVal2021[j];
                    for (int j = 0; j < arrVal2020.Length; j++)
                        if (arrVal2020[j] > maxVal)
                            maxVal = arrVal2020[j];

                    SrC.Add(new ColumnSeries
                    {
                        Title = namNhat.Text,
                        Values = new ChartValues<decimal> { arrVal2022[0], arrVal2022[1], arrVal2022[2], arrVal2022[3], arrVal2022[4], arrVal2022[5], arrVal2022[6], arrVal2022[7], arrVal2022[8], arrVal2022[9], arrVal2022[10], arrVal2022[11] },
                        Fill = Brushes.Red
                    });
                    SrC.Add(new ColumnSeries
                    {
                        Title = namHai.Text,
                        Values = new ChartValues<decimal> { arrVal2021[0], arrVal2021[1], arrVal2021[2], arrVal2021[3], arrVal2021[4], arrVal2021[5], arrVal2021[6], arrVal2021[7], arrVal2021[8], arrVal2021[9], arrVal2021[10], arrVal2021[11] },
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
            if (CanClick)
            {
                string getNgay = TrucHoanhX.Labels[(int)chartPoint.X];
                //Xử Lý công đoạn lấy ngày ở đây
                /* int getDayLB0 = DateTime.Parse(Labels[0]).Day;
                 int getMonthLB0 = DateTime.Parse(Labels[0]).Month;
                 int getYearLB0 = DateTime.Parse(Labels[0]).Year;
                 string Ngay = TrucHoanhX.Labels[(int)chartPoint.X];
                 int GetNgay = int.Parse(Ngay.Substring(0, 2)); //Lấy cái ngày của dataPoint vừa bấm
                 if (getDayLB0 < GetNgay) 
                 {
                     //Tức là getNgay cùng tháng của Labels[0] và nó lớn hơn ngày Labels[0]
                     Ngay = GetNgay.ToString() + "-" + getMonthLB0 + "-" + getYearLB0; //Ghép lại thành ngày hoàn chỉnh
                 }
                 else
                 {
                  //Có quá nhiều công đoạn ở đây, ... zzzz
                 }


                 //if (getDayLB0 > )

                 //if (getNgay>=Labels[0])
                 //if getNgay nam trong nam nao thi them nam va thang do dang sau */
                WindowInformation wd = new WindowInformation(getNgay);
                wd.ShowDialog();
            }
        }

    }
}
