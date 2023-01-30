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
            CanClick = true;

            decimal[] arrDoanhThu = new decimal[1000];
            ChartValues<decimal> ListDoanhThu = new();
            Labels = new();
            con.Open();
            //Nên đổi doanh thu Tháng Này thành doanh thu 30 ngày gần nhất
            //để lúc nào nó cũng luôn có dữ liệu
            string lastdate = DateTime.Today.AddDays(-30.0).ToString("dd/MM/yyyy");
            for (DateTime date = DateTime.Today.AddDays(-29.0); date < DateTime.Now; date = date.AddDays(1.0))
            {
                decimal money = mdb.HoaDons.Where(u => u.NgayLapHd == date).Select(u => u.ThanhTien).Sum();
                ListDoanhThu.Add(money);
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
              sau đó đổ lại dữ liệu vào.*/
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
            string maxDate = ngaylonnhat.ToString("d/M/yyyy"); //dd/MM/yyyy
            DateTime ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault() ?? DateTime.Today;
            string minDate = ngaynhonhat.ToString("d/M/yyyy");
            if (!IsValidDateTimeTest(txtTuNgay.Text))
            {
                MessageBox.Show("Ô Từ Ngày Chứa Ngày Không Hợp Lệ!\nGiới hạn ngày nằm trong khoảng từ 01/01/1900 đến 06/06/2079");
                txtTuNgay.Foreground = Brushes.Red;
            }
            else  //txt Từ Ngày chứa NGÀY HỢP LỆ
            {
                txtTuNgay.Foreground = Brushes.Black;
                if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) > ngaylonnhat)
                    MessageBox.Show("Các ngày sau ngày " + maxDate + " CHƯA CÓ DỮ LIỆU\nDoanh Thu mặc định của các ngày đó sẽ = 0");
                if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) < ngaynhonhat)
                    MessageBox.Show("Các ngày trước ngày " + minDate + " CHƯA CÓ DỮ LIỆU\nDoanh Thu mặc định của các ngày đó sẽ = 0");
            }
        }

        private void txtDenNgay_LostFocus(object sender, RoutedEventArgs e)
        {
            DateTime ngaylonnhat = mdb.HoaDons.OrderByDescending(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault() ?? DateTime.Today;
            string maxDate = ngaylonnhat.ToString("d/M/yyyy"); //dd/MM/yyyy
            DateTime ngaynhonhat = mdb.HoaDons.OrderBy(u => u.NgayLapHd).Select(u => u.NgayLapHd).FirstOrDefault() ?? DateTime.Today;
            string minDate = ngaynhonhat.ToString("d/M/yyyy");
            if (!IsValidDateTimeTest(txtDenNgay.Text))
            {
                MessageBox.Show("Ô Đến Ngày Chứa Ngày Không Hợp Lệ!\nGiới hạn ngày nằm trong khoảng từ 01/01/1900 đến 06/06/2079");
                txtDenNgay.Foreground = Brushes.Red;
            }
            if (IsValidDateTimeTest(txtDenNgay.Text))
            {
                txtDenNgay.Foreground = Brushes.Black;
                if (DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) > ngaylonnhat)
                    MessageBox.Show("Các ngày sau ngày " + maxDate + " CHƯA CÓ DỮ LIỆU\nDoanh Thu mặc định của các ngày đó sẽ = 0");
                if (DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) < ngaynhonhat)
                    MessageBox.Show("Các ngày trước ngày " + minDate + " CHƯA CÓ DỮ LIỆU\nDoanh Thu mặc định của các ngày đó sẽ = 0");
            }
            /*Khi ô Đến Ngày LostFocus, ta sẽ check nó có phải ngày hợp lệ hay kh,
              và check xem nó có bé hơn hoặc = Từ Ngày hay kh*/
        }

        private void btnXem_Click(object sender, RoutedEventArgs e)
        {
            /*Chỉ có duy nhất Lựa chọn hàm này là người dùng đc 
              phép Zoom, nên sẽ tìm MaxValue ở đây để tránh tình trạng
              Zoom quá mức nó sẽ ra giá trị Rác*/

            ChartValues<decimal> ChartVal = new();
            if (string.IsNullOrEmpty(txtTuNgay.Text) || string.IsNullOrEmpty(txtDenNgay.Text))
                MessageBox.Show("Vui Lòng Nhập Đầy Đủ 2 Ngày");
            else if (!IsValidDateTimeTest(txtTuNgay.Text) || !IsValidDateTimeTest(txtDenNgay.Text))
                MessageBox.Show("Không Thể Xem Vì Có Chứa Ngày Không Hợp Lệ\nHãy Kiểm Tra Lại!");
            else if (DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture))
            {
                MessageBox.Show("Từ Ngày Không Được Phép Lớn Hơn Hoặc Bằng Đến Ngày!\nHãy Nhập Lại!");
                txtTuNgay.Foreground = Brushes.Red;
                txtDenNgay.Foreground = Brushes.Red;
            }
            else //Thoả mãn hết các điều kiện => được phép xem 
            {
                lblSeries.Content = "         Ngày";
                lblDTThgNay.Content = "So Sánh Doanh Thu(Đơn Vị: VNĐ)";
                CanClick = true; //Cho phép bấm vào DataPoint
                while (dothi.Series.Count > 0)
                    dothi.Series.RemoveAt(0);
                Labels.Clear();
                /*3 dòng trên để Clear hết các dữ liệu cũ,
                  Dọn chỗ cho dữ liệu mới*/

                dothi.Zoom = ZoomingOptions.X; //Cho phép Zoom trục hoành
                dothi.Pan = PanningOptions.X;  //Cho phép Lia trục hoành
                dothi.FontSize = 15;
                TrucHoanhX.FontSize = 12;
                con.Open();

                for (DateTime date = DateTime.ParseExact(txtTuNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture); date <= DateTime.ParseExact(txtDenNgay.Text, "d/M/yyyy", CultureInfo.InvariantCulture); date = date.AddDays(1.0))
                {
                    decimal money = mdb.HoaDons.Where(u => u.NgayLapHd == date).Select(u => u.ThanhTien).Sum();
                    ChartVal.Add(money);
                    Labels.Add(date.ToString("d-M-yyyy"));
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
                if ((double)ChartVal.Count / 15 - ChartVal.Count / 15 > 0)
                    TrucHoanhX.Separator.Step = ChartVal.Count / 15 + 1;
                else if ((double)ChartVal.Count / 15 - ChartVal.Count / 15 == 0)
                    TrucHoanhX.Separator.Step = ChartVal.Count / 15;
                //ĐK if else ở trên để tăng bước trục hoành dựa vào khoảng ngày
                //0<NGÀY<30: step = 2, 30<NGÀY<60: step = 2 , ...  

                dothi.AxisY[0].MaxValue = (double)maxVal * 1.2;
                /*1 dòng trên để set max value trục tung cho đồ thị,
                  tránh nó nhận giá trị RÁC khi Zoom quá*/
                Values = value => value.ToString("N");
                lblZoomIn.Visibility = Visibility.Visible;
            }
        }

        private void btnXemNam_Click(object sender, RoutedEventArgs e)
        {
            CanClick = false;
            if (luachon == 1)
            {
                if (string.IsNullOrEmpty(namNhat.Text) || string.IsNullOrEmpty(namHai.Text))
                    MessageBox.Show("Có trường dữ liệu rỗng, vui lòng kiểm tra lại!");
                else
                {
                    lblSeries.Content = "         Tháng";
                    lblDTThgNay.Content = "     So Sánh Doanh Thu 2 Năm";
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

                    for (int i = 1; i <= 12; i++)
                    {
                        StartDate2 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2 = i + 1;
                        if (i == 12)
                            EndDate2 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2 = "1/" + thangsau2 + "/" + namNhat.Text;

                        decimal money2 = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate2,"d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd <= DateTime.ParseExact(EndDate2, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
                        arrVal2[i - 1] = money2;

                        StartDate1 = "1/" + i + "/" + namHai.Text;
                        int thangsau1 = i + 1;
                        if (i == 12)
                            EndDate1 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate1 = "1/" + thangsau1 + "/" + namHai.Text;
                        
                        decimal money1 = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate1, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd <= DateTime.ParseExact(EndDate1, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
                        arrVal1[i - 1] = money1;    
                    }

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
                    lblSeries.Content = "         Tháng";
                    lblDTThgNay.Content = "     So Sánh Doanh Thu 3 Năm";
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
                    decimal[] arrVal0 = new decimal[12];
                    decimal maxVal = 0;
                    string StartDate2;
                    string EndDate2;
                    string StartDate1;
                    string EndDate1;
                    string StartDate0;
                    string EndDate0;

                    for (int i = 1; i <= 12; i++)
                    {
                        StartDate2 = "1/" + i + "/" + namNhat.Text;
                        int thangsau2 = i + 1;
                        if (i == 12)
                            EndDate2 = "1/1/" + (int.Parse(namNhat.Text) + 1).ToString();
                        else
                            EndDate2 = "1/" + thangsau2 + "/" + namNhat.Text;

                        decimal money2 = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate2, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd <= DateTime.ParseExact(EndDate2, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
                        arrVal2[i - 1] = money2;

                        StartDate1 = "1/" + i + "/" + namHai.Text;
                        int thangsau1 = i + 1;
                        if (i == 12)
                            EndDate1 = "1/1/" + (int.Parse(namHai.Text) + 1).ToString();
                        else
                            EndDate1 = "1/" + thangsau1 + "/" + namHai.Text;

                        decimal money1 = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate1, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd <= DateTime.ParseExact(EndDate1, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
                        arrVal1[i - 1] = money1;

                        StartDate0 = "1/" + i + "/" + namBa.Text;
                        int thangsau2020 = i + 1;
                        if (i == 12)
                            EndDate0 = "1/1/" + (int.Parse(namBa.Text) + 1).ToString();
                        else
                            EndDate0 = "1/" + thangsau2020 + "/" + namBa.Text;

                        decimal money0 = mdb.HoaDons.Where(u => u.NgayLapHd >= DateTime.ParseExact(StartDate0, "d/M/yyyy", CultureInfo.InvariantCulture) && u.NgayLapHd <= DateTime.ParseExact(EndDate0, "d/M/yyyy", CultureInfo.InvariantCulture)).Select(u => u.ThanhTien).Sum();
                        arrVal0[i - 1] = money0;
                    }

                    maxVal = arrVal2[0];
                    for (int j = 1; j < arrVal2.Length; j++)
                        if (arrVal2[j] > maxVal)
                            maxVal = arrVal2[j];
                    for (int j = 0; j < arrVal1.Length; j++)
                        if (arrVal1[j] > maxVal)
                            maxVal = arrVal1[j];
                    for (int j = 0; j < arrVal0.Length; j++)
                        if (arrVal0[j] > maxVal)
                            maxVal = arrVal0[j];

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
                        Values = new ChartValues<decimal> { arrVal0[0], arrVal0[1], arrVal0[2], arrVal0[3], arrVal0[4], arrVal0[5], arrVal0[6], arrVal0[7], arrVal0[8], arrVal0[9], arrVal0[10], arrVal0[11] },
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
              Tuy nhiên vẫn còn chút lỗi hiển thị*/
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
            if(CanClick)
            {
                string getNgay = TrucHoanhX.Labels[(int)chartPoint.X];
                WindowInformation wd = new WindowInformation(getNgay);
                wd.ShowDialog();
            }
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
