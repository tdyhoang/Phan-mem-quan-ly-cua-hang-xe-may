using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;
using MotoStore.Views.Pages.LoginPages;
using Microsoft.Data.SqlClient;
using System.Windows.Markup;
using System.Globalization;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private MainDatabase mdb = new MainDatabase();
        private PageChinh pgChinh;
        private RichTextBox rtb;
        private ComboBox cbGioBD;
        private ComboBox cbPhutBD;
        private ComboBox cbGioKT;
        private ComboBox cbPhutKT;
        private DateTime dt = DateTime.Now;
        private int soSuKien = 0;  //Biến đếm số sự kiện trong tuần
        private int danhdau = 0;   //Biến đánh dấu lời nhắc số sự kiện
        public DashboardPage(PageChinh pgC)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            pgChinh = pgC;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(danhdau==0)  //danhdau=0 có nghĩa là DashboardPage lần đầu được khởi tạo

            foreach (var demngay in mdb.LenLichs.ToList())
            {
                //Một tuần tới không có sự kiện gì đáng chú ý
                //Nếu có nhiều hơn 1 sự kiện thì Tuần này có > 1 sự kiện đáng chú ý, xem chi tiết ở lịch
                //Nếu chỉ có một: Nhắc Bạn: Còn n ngày, Hôm nay 
                //Tuần này có 3 sự kiện đáng chú ý, xem chi tiết ở Lịch
                if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(demngay.NgLenLichBd.Value))
                {
                    soSuKien++;   //Vì nó là biến toàn cục nên cứ thế mà tăng 
                    danhdau = 1;
                   //Đánh dấu = 1 để hạn chế những lần load trang Dashboard sau nó tự động tăng số sự kiện 
                }
            }
            if (soSuKien == 0)
                txtblLoiNhac.Text = "".PadRight(8) + "Tuần Này\n Không Có Sự Kiện\n Nào Đáng Chú Ý";
            else
                txtblLoiNhac.Text = "".PadRight(5) +"Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            /*Hàm này để tính tuần của ngày,
              mục đích là so sánh ngày hiện tại
              và ngày của LenLich trên database
              có cùng tuần hay không để hiển thị
              thông tin hợp lý*/
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblGioHeThong.Content = "Bây giờ là: " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnDgXuat_Click(object sender, RoutedEventArgs e)
        {
            stkNoiDung.Children.Clear();
            stkLich.Children.Clear();
            stkbtnLenLich.Children.Clear();
            stkbtnXoaLich.Children.Clear();
            /*3 dòng trên để:
            Xoá ô Nội Dung
            Xoá ô Lên Lịch
            Xoá button Lên Lịch(Dành cho Quản Lý)*/

            App.Current.MainWindow.Visibility = Visibility.Collapsed;  //Ẩn Màn hình chính đi
            Windows.LoginView loginView = new Windows.LoginView();
            loginView.Show();           //Hiện màn hình đăng nhập
        }

        private void Lich_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            stkNoiDung.Children.Clear();
            RichTextBox rtbNoiDung = new RichTextBox();
            rtbNoiDung.Height = 150;
            rtbNoiDung.Width = 250;
            rtbNoiDung.Foreground = Brushes.Red;
            rtbNoiDung.FontSize = 13;
            if (PageChinh.getLoaiNV != 1)
                rtbNoiDung.IsReadOnly = true;
            stkNoiDung.Children.Add(rtbNoiDung);
            MainDatabase mainDatabase = new MainDatabase();

            int co = 0;
            foreach(var ngay in mainDatabase.LenLichs.ToList())  
            {
                if (Lich.SelectedDate.Value.ToString("dd/MM/yyyy") == ngay.NgLenLichBd.Value.ToString("dd/MM/yyyy"))
                {
                    rtbNoiDung.AppendText("Bắt Đầu: "+ngay.NgLenLichBd.Value.ToString("HH:mm")+ " - Kết Thúc: "+ngay.NgLenLichKt.Value.ToString("HH:mm")+ "\nNội Dung: " + ngay.NoiDungLenLich+"\n");
                    co = 1;
                }
            }
            if (co == 0)
                rtbNoiDung.AppendText("Không có sự kiện nổi bật");

            if (PageChinh.getLoaiNV==1) //Tất nhiên ng Quản Lý mới có quyền lên lịch
            {
                stkLich.Children.Clear();
                rtb = new RichTextBox();
                rtb.Height = 100;
                rtb.Width = 240;
                rtb.Foreground = Brushes.Black;
                rtb.FontSize = 14;
                stkLich.Children.Add(rtb);
                //Các dòng trên là tạo một RichTextBox chứa Nội Dung để Lên Lịch

                cbGioBD = new ComboBox();
                cbGioBD.Height = 40;
                cbGioBD.Width = 70;
                cbGioBD.Margin = new Thickness(15, 0, 0, 0);
                for (int i = 0; i <= 23; i++)
                    cbGioBD.Items.Add(i);
                cbPhutBD = new ComboBox();
                cbPhutBD.Height = 40;
                cbPhutBD.Width = 70;
                cbPhutBD.Margin = new Thickness(170, -40, 0, 0);
                for (int i = 0; i <= 59; i++)
                    cbPhutBD.Items.Add(i);
                stkLich.Children.Add(cbGioBD);
                stkLich.Children.Add(cbPhutBD);
                Label lblBatDau = new Label();
                lblBatDau.Content = "Giờ Bắt Đầu: ";
                lblBatDau.Height = 30;
                lblBatDau.Width = 85;
                lblBatDau.Margin = new Thickness(-160, -20, 0, 0);
                lblBatDau.FontSize = 14;
                lblBatDau.FontWeight = FontWeights.Medium;
                lblBatDau.Foreground = Brushes.Black;
                stkLich.Children.Add(lblBatDau);
                Label lblKetThuc = new Label();
                lblKetThuc.Content = "Giờ Kết Thúc: ";
                lblKetThuc.Height = 30;
                lblKetThuc.Width = 85;
                lblKetThuc.Margin = new Thickness(-160, 35, 0, 0);
                lblKetThuc.FontSize = 14;
                lblKetThuc.FontWeight = FontWeights.Medium;
                lblKetThuc.Foreground = Brushes.Black;
                stkLich.Children.Add(lblKetThuc);
                cbGioKT = new ComboBox();
                cbGioKT.Height = 40;
                cbGioKT.Width = 70;
                cbGioKT.Margin = new Thickness(15, -55, 0, 0);
                for (int i = 0; i <= 23; i++)
                    cbGioKT.Items.Add(i);
                cbPhutKT = new ComboBox();
                cbPhutKT.Height = 40;
                cbPhutKT.Width = 70;
                cbPhutKT.Margin = new Thickness(170, -55, 0, 0);
                for (int i = 0; i <= 59; i++)
                    cbPhutKT.Items.Add(i);
                stkLich.Children.Add(cbGioKT);
                stkLich.Children.Add(cbPhutKT);
            }

            /*Hàm này hơi dài, nhưng tóm gọn lại là
              có chức năng Lên Lịch và Xoá Lịch dành 
              cho người Quản Lý mỗi khi click chuột
              vào ngày bất kì trên tờ Lịch
            */
        }

        // Event Handler cho VisibleChanged
        private void DashboardPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)  //(bool)e.NewValue == true thì có nghĩa là Ứng dụng đc hiện lên
            {
                DashboardPage_Initialize();
                if(PageChinh.getLoaiNV==1)
                {
                    Button btnLenLich = new Button();
                    btnLenLich.Height = 40;
                    btnLenLich.Width = 110;
                    btnLenLich.Content = "Lên Lịch";
                    btnLenLich.Foreground = Brushes.Black;
                    btnLenLich.Background = Brushes.Aquamarine;
                    btnLenLich.FontSize = 19;
                    btnLenLich.FontWeight = FontWeights.Medium;
                    btnLenLich.Click += btnLenLich_Click;
                    stkbtnLenLich.Children.Add(btnLenLich);
                    Button btnXoaLich = new Button();
                    btnXoaLich.Height = 40;
                    btnXoaLich.Width = 110;
                    btnXoaLich.Content = "Xoá Lịch";
                    btnXoaLich.Foreground = Brushes.Black;
                    btnXoaLich.Background = Brushes.DeepSkyBlue;
                    btnXoaLich.FontSize = 19;
                    btnXoaLich.FontWeight = FontWeights.Medium;
                    btnXoaLich.Click += btnXoaLich_Click;
                    stkbtnXoaLich.Children.Add(btnXoaLich);
                }
            }
            else
            {
                // Collapse code here, Ứng dụng bị ẩn                
            }
            /*Hàm này nghĩa là mỗi lần Đăng Xuất sẽ ẩn Ứng dụng đi,
              chỉ hiện Form Đăng Nhập, cứ mỗi lần như vậy thì hàm này
              nó sẽ check nhân viên loại nào đăng nhập để hiển thị các
              button Lên, Xoá Lịch
             */
        }

        private void btnLenLich_Click(object sender, RoutedEventArgs e)
        {
            //if có ngày được select
            if (Lich.SelectedDate.HasValue)
            {
                string richText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
                if (string.IsNullOrEmpty(cbGioBD.Text) || string.IsNullOrEmpty(cbPhutBD.Text) || string.IsNullOrEmpty(cbGioKT.Text) || string.IsNullOrEmpty(cbPhutKT.Text))
                    MessageBox.Show("Vui lòng chọn giờ cụ thể");
                else if (string.IsNullOrWhiteSpace(richText))
                {
                    MessageBox.Show("Vui lòng ghi nội dung");
                }
                else
                {
                    if (int.Parse(cbGioBD.Text) > int.Parse(cbGioKT.Text) || int.Parse(cbGioBD.Text) == int.Parse(cbGioKT.Text) && int.Parse(cbPhutBD.Text) >= int.Parse(cbPhutKT.Text))
                        MessageBox.Show("Giờ kết thúc không thể nhỏ hơn hoặc bằng giờ bắt đầu");
                    else
                    {
                        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-5TG85UFG\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                        /*Dòng trên ae cop cái connection string của máy mình thay cho cái dòng trên
                        Nhớ chạy lại dòng lệnh databasePMQLCHBXM.sql trên máy mình*/

                        SqlCommand cmd;
                        con.Open();
                        string lich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy");

                        //Giải thích dòng trên:
                        //Vì SelectedDate.Value sẽ cho ra ngày/tháng/năm + giờ/phút/giây nên ta lược bớt phần sau (chỉ giữ lại ngày tháng năm)

                        cmd = new SqlCommand("set dateformat dmy\nInsert into LenLich values('" + PageChinh.getMa.ToString() + "', '" + lich + " " + cbGioBD.Text + ":" + cbPhutBD.Text + ":00" + "', '" + lich + " " + cbGioKT.Text + ":" + cbPhutKT.Text + ":00" + "', N'" + richText + "')", con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Lên lịch thành công!");
                        if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                        {
                            soSuKien++;
                            txtblLoiNhac.Text = "".PadRight(5) + "Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                        }
                        stkLich.Children.Clear();
                        stkNoiDung.Children.Clear();
                    }
                }
            }
            else
                MessageBox.Show("Vui lòng chọn ngày lên lịch");
        }

        private void btnXoaLich_Click(object sender, RoutedEventArgs e)
        {
            if (Lich.SelectedDate.HasValue)
            {
                if(string.IsNullOrEmpty(cbGioBD.Text)||string.IsNullOrEmpty(cbPhutBD.Text))
                {
                    MessageBox.Show("Vui Lòng chọn giờ sự kiện muốn xoá");
                }
                else
                {
                    string strGiomuonXoa = Lich.SelectedDate.Value.ToString("dd-MM-yyyy") +" "+ cbGioBD.Text + ":" + cbPhutBD.Text + ":00";
                    bool Deleted = false;
                    foreach(var gio in mdb.LenLichs.ToList())
                    {
                        if(gio.NgLenLichBd.Value.ToString("dd-MM-yyyy HH:mm:ss") == strGiomuonXoa)
                        {
                            SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-5TG85UFG\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                            SqlCommand cmd;
                            con.Open();
                            string lich = Lich.SelectedDate.Value.ToString("dd-MM-yyyy");
                            cmd = new SqlCommand("set dateformat dmy\ndelete from LenLich where NgLenLichBD='" + strGiomuonXoa + "'", con);
                            cmd.ExecuteNonQuery();
                            con.Close();
                            MessageBox.Show("Xoá Sự Kiện thành công");
                            if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                            {
                                soSuKien--;
                                txtblLoiNhac.Text = "".PadRight(5) + "Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                            }
                            stkLich.Children.Clear();
                            stkNoiDung.Children.Clear();
                            Deleted = true;
                            break;
                        }
                    }
                    if (!Deleted)
                        MessageBox.Show("Không tìm thấy Sự Kiện cần xoá");
                }
            }
            else
                MessageBox.Show("Vui lòng chọn ngày xoá lịch");
        }

        // Hàm khởi tạo DashboardPage, nên đặt tên khác cho dễ hiểu hơn
        private void DashboardPage_Initialize()
        {
            DateTime dt = DateTime.Now;

            var hoTenNV = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.HoTenNv).FirstOrDefault().ToString();
            var seperatedHoTenNV = hoTenNV.Split(' ');
            var tenNV = seperatedHoTenNV[seperatedHoTenNV.Length - 1];

            lblXinChao.Content = "Xin Chào, " + tenNV;

            switch (PageChinh.getLoaiNV)
            {
                case 1:  //Nhân viên loại 1
                    txtblSoNV.Text = "   Số Nhân Viên\n   Bạn Quản Lý:\n" + "".PadRight(12) + (mdb.NhanViens.Select(d => d.MaNv).Count() - 1).ToString();
                    txtblSoXe.Text = "".PadRight(9) + "Số Xe\n" + "".PadRight(5) + "Trong Kho:\n" + "".PadRight(11) + mdb.MatHangs.Sum(d => d.SoLuongTonKho).ToString();
                    anhNhanVien.Source = new BitmapImage(new Uri("F:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\anh3.png"));
                    txtblSoNV.FontSize = 20;
                    txtblSoXe.FontSize = 20.5;
                    break;
                case 2:  //Nhân viên loại 2
                    //3 dòng dưới để lấy ngày vào làm của nhân viên, tính số ngày từ đó đến nay và hiển thị nó
                    var dx = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                    int d3 = (int)(dt - dx).Value.TotalDays;
                    txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d3.ToString() + " Ngày";

                    var slg = mdb.HoaDons.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.SoLuong).Sum();
                    txtblSoXe.Text = "".PadRight(15) + slg.ToString() + "\n".PadRight(10) +"Là Số Xe\n"+"".PadRight(1)+"Bạn Bán Được";

                    if (PageChinh.getSex == "Nữ")
                        anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNu.png"));
                    else
                        anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNam.png"));
                    txtblSoNV.FontSize = 19;
                    txtblSoXe.FontSize = 18.9;
                    break;
            }
        }

    }
}
