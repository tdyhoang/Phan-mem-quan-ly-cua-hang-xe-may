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
using System.Windows.Media.Animation;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.IO;
using Path = System.IO.Path;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private MainDatabase mdb = new MainDatabase();
        private PageChinh pgChinh;
        private DateTime dt = DateTime.Now;
        private int soSuKien = 0;  //Biến đếm số sự kiện trong tuần
        private int danhdau = 0;   //Biến đánh dấu lời nhắc số sự kiện
        private bool enableLenLich;
        /*Biến cho phép lên lịch, tránh trường hợp ng dùng nhấn lại nút lên lịch
        sau khi đã lên lịch thành công mà ứng dụng vẫn lên lịch*/
        private bool enableXoaLich;
        //Biến cho phép xoá lịch, công dụng tương tự như trên
        private bool isFirstClicked = true;
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);

        List<string> listMaNV { get; set; } = new List<string>();
        List<string> listTenNV { get; set; } = new List<string>();
        List<string> listThoiGian { get; set; } = new List<string>();
        List<string> listHoatDong { get; set; } = new List<string>();

        public DashboardPage(PageChinh pgC)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            pgChinh = pgC;
            //this.DataContext = this;
        }

        public class LichSuHoatDong
        {
            public string MaNV { get; set; }
            public string HoTenNV { get; set; }
            public string ThoiGian { get; set; }
            public string HoatDong { get; set; }
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (danhdau == 0)  //danhdau=0 có nghĩa là DashboardPage lần đầu được khởi tạo
            {
                for (int i = 0; i <= 23; i++)
                    cbGioBD.Items.Add(i);
                for (int i = 0; i <= 23; i++)
                    cbGioKT.Items.Add(i);
                for (int i = 0; i <= 59; i++)
                    cbPhutBD.Items.Add(i);
                for (int i = 0; i <= 59; i++)
                    cbPhutKT.Items.Add(i);
                foreach (var demngay in mdb.LenLichs.ToList())
                {
                    //Một tuần tới không có sự kiện gì đáng chú ý
                    //Nếu có nhiều hơn 1 sự kiện thì Tuần này có > 1 sự kiện đáng chú ý, xem chi tiết ở lịch
                    //Nếu chỉ có một: Nhắc Bạn: Còn n ngày, Hôm nay 
                    //Tuần này có 3 sự kiện đáng chú ý, xem chi tiết ở Lịch
                    if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(demngay.NgLenLichBd.Value))
                    {
                        soSuKien++;   //Vì nó là biến toàn cục nên cứ thế mà tăng 
                        //Đánh dấu = 1 để hạn chế những lần load trang Dashboard sau nó tự động tăng số sự kiện 
                    }
                }
                danhdau = 1;
            }
            if (soSuKien == 0)
                txtblLoiNhac.Text = "".PadRight(8) + "Tuần Này\n Không Có Sự Kiện\n Nào Đáng Chú Ý";
            else
                txtblLoiNhac.Text = "".PadRight(5) + "Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
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
            dataGridLSHD.Items.Clear(); //Clear dữ liệu Lịch Sử, khi nào 1st Click thì đổ dữ liệu lại sau
            dataGridLSHD.Visibility = Visibility.Collapsed;
            btnXoaLichSu.Visibility = Visibility.Collapsed;
            rtbNoiDung.Visibility = Visibility.Collapsed;
            isFirstClicked = true;

            SqlCommand cmd;
            con.Open();
            DateTime DT = DateTime.Now;
            cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '" + PageChinh.getMa + "', '" + DT.ToString("dd-MM-yyyy HH:mm:ss") + "', N'" + "đăng xuất')", con);
            cmd.ExecuteNonQuery();
            con.Close();

            App.Current.MainWindow.Visibility = Visibility.Collapsed;  //Ẩn Màn hình chính đi
            Windows.LoginView loginView = new Windows.LoginView();
            loginView.Show();           //Hiện màn hình đăng nhập
        }

        private void Lich_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            rtbNoiDung.Visibility = Visibility.Visible;
            if (PageChinh.getChucVu.ToLower() == "quản lý")
            {
                enableLenLich = true;    //Cho phép lên lịch mỗi lần click chuột vào ngày bất kì trên Lịch
                enableXoaLich = true;
                borderLichvaButton.Visibility = Visibility.Visible;
                dataGridLSHD.Visibility = Visibility.Collapsed;
            }
            MainDatabase mainDatabase = new MainDatabase();
            bool co = false;   //biến này để check xem có sự kiện nổi bật hay không
            rtbNoiDung.Document.Blocks.Clear(); //Clear hết nội dung mỗi lần bấm, sau đó đổ lại nội dung vào
            //Sắp xếp sự kiện
            List<string> listNgBD = new();
            List<string> listNgKT = new();
            List<string> listNoiDung = new();
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT CONVERT(VARCHAR(8),NgLenLichBD,108) from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda = cmd.ExecuteReader();
            while (sda.Read())
                listNgBD.Add((string)sda[0]);

            SqlCommand cmd2 = new SqlCommand("SELECT CONVERT(VARCHAR(8),NgLenLichKT,108) from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda2 = cmd2.ExecuteReader();
            while (sda2.Read())
                listNgKT.Add((string)sda2[0]);

            SqlCommand cmd3 = new SqlCommand("SELECT NoiDungLenLich from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda3 = cmd3.ExecuteReader();
            while (sda3.Read())
                listNoiDung.Add((string)sda3[0]);

            int TabLength = 0;
            SqlCommand cmd4 = new SqlCommand("SELECT Count(*) from LenLich", con);
            SqlDataReader sda4 = cmd4.ExecuteReader();
            while (sda4.Read())
                TabLength = (int)sda4[0];

            for (int i = 0; i < TabLength; i++) 
            {
                if (Lich.SelectedDate.Value.ToString("d/M/yyyy") == DateTime.Parse(listNgBD[i]).ToString("d/M/yyyy"))
                {
                    rtbNoiDung.AppendText("Bắt Đầu: " + listNgBD[i] + " - Kết Thúc: " + listNgKT[i] + "\n" + "Nội Dung: " + listNoiDung[i] + "\n");
                    co = true;
                }
            }
            con.Close();
            if (co == false)
                rtbNoiDung.AppendText("Không có sự kiện nổi bật");
            /*Hàm này để khi ng quản lý bấm vào ngày
            bất kì trên lịch sẽ hiện nội dung(Nếu có)*/
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
            borderLichvaButton.Visibility = Visibility.Collapsed;
            //Button Hủy tương tác với Lịch
        }

        // Event Handler cho VisibleChanged
        private void DashboardPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)  //(bool)e.NewValue == true thì có nghĩa là Ứng dụng đc hiện lên
            {
                DashboardPage_Initialize();
            }
            else
            {
                // Collapse code here, Ứng dụng bị ẩn                
            }

            /*Hàm này nghĩa là mỗi lần Đăng Xuất sẽ ẩn Ứng dụng đi,
              chỉ hiện Form Đăng Nhập, cứ mỗi lần đăng nhập đúng TK, MK
              thì sẽ gọi hàm khởi tạo trang Dashboard_Initialize()
             */
        }

        private void btnLenLich_Click(object sender, RoutedEventArgs e)
        {
            isFirstClicked = true;
            bool valid = true; //Biến này để check xem khoảng thời gian lên lịch có hợp lệ hay không
            /*Một khoảng thời gian lên lịch đc xem là hợp lệ 
              NẾU nó không chen vào khoảng thgian sự kiện khác
              HOẶC nó không chứa khoảng thời gian sự kiện khác*/

            if (enableLenLich)
            {
                string ngaylenlich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy") + " " + cbGioBD.Text + ":" + cbPhutBD.Text + ":00";
                //if có ngày được select
                if (Lich.SelectedDate.HasValue)
                {
                    string richText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
                    if (string.IsNullOrEmpty(cbGioBD.Text) || string.IsNullOrEmpty(cbPhutBD.Text) || string.IsNullOrEmpty(cbGioKT.Text) || string.IsNullOrEmpty(cbPhutKT.Text))
                        MessageBox.Show("Vui lòng chọn giờ cụ thể");
                    else if (DateTime.Parse(ngaylenlich) < DateTime.Now)
                    {
                        MessageBox.Show("Ngày hoặc giờ bắt đầu lên lịch không hợp lệ");
                    }
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
                            //Không cho phép một sự kiện mới đc chen vào giữa giờ 1 sự kiện khác
                            //Hoặc khoảng thời gian sự kiện mới lại chứa các sự kiện khác
                            foreach (var item in mdb.LenLichs.ToList())
                            {
                                if (item.NgLenLichBd.Value.ToString("dd-MM-yyyy") == Lich.SelectedDate.Value.ToString("dd-MM-yyyy"))
                                {
                                    string gioBD = int.Parse(cbGioBD.Text) + ":" + int.Parse(cbPhutBD.Text) + ":00";
                                    string gioKT = int.Parse(cbGioKT.Text) + ":" + int.Parse(cbPhutKT.Text) + ":00";
                                    if (DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss")) <= DateTime.Parse(gioBD) && DateTime.Parse(gioBD) <= DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss")) || DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss")) <= DateTime.Parse(gioKT) && DateTime.Parse(gioKT) <= DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss")) || DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss")) >= DateTime.Parse(gioBD) && DateTime.Parse(gioKT) >= DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss")))
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                            }
                            if (valid)
                            {
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("d/M/yyyy");
                                //Giải thích dòng trên:
                                //Vì Lich.SelectedDate.Value sẽ cho ra ngày/tháng/năm + giờ/phút/giây nên ta lược bớt phần sau (chỉ giữ lại ngày tháng năm)

                                SqlCommand cmd = new SqlCommand("set dateformat dmy\nInsert into LenLich values(NewID(),'" + PageChinh.getMa.ToString() + "', '" + lich + " " + cbGioBD.Text + ":" + cbPhutBD.Text + ":00" + "', '" + lich + " " + cbGioKT.Text + ":" + cbPhutKT.Text + ":00" + "', N'" + richText + "')", con);
                                cmd.ExecuteNonQuery();
                                DateTime DT = DateTime.Now;
                                cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(NewID(),'" + PageChinh.getMa + "', '" + DT.ToString("dd-MM-yyyy HH:mm:ss") + "', N'" + "lên lịch cho ngày " + lich + "')", con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                                MessageBox.Show("Lên lịch thành công!");
                                if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                                {
                                    soSuKien++;
                                    txtblLoiNhac.Text = "".PadRight(5) + "Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                                }
                                cbGioBD.Text = String.Empty;
                                cbPhutBD.Text = String.Empty;
                                cbGioKT.Text = String.Empty;
                                cbPhutKT.Text = String.Empty;
                                rtb.Document.Blocks.Clear();
                                enableLenLich = false;
                                enableXoaLich = false;
                                /*Lên Lịch xong thì set lại 2 biến trên
                                  tránh việc ứng dụng tự thêm, xoá sự kiện 
                                  khi ng dùng nhấn tiếp 1 trong 2 nút*/
                            }
                            else
                                MessageBox.Show("Giờ lên lịch không hợp lệ vì khoảng thời gian đã nằm trong hoặc bao gồm một hoặc nhiều sự kiện khác!");
                        }
                    }
                }
                else
                    MessageBox.Show("Vui lòng chọn ngày lên lịch");
            }
            else
                MessageBox.Show("Vui lòng chọn lại ngày lên lịch");
            //Hàm này để Lên Lịch
        }

        private void btnXoaLich_Click(object sender, RoutedEventArgs e)
        {
            isFirstClicked = true;
            if (enableXoaLich)
            {
                if (Lich.SelectedDate.HasValue)
                {
                    if (string.IsNullOrEmpty(cbGioBD.Text) || string.IsNullOrEmpty(cbPhutBD.Text))
                    {
                        MessageBox.Show("Vui lòng chọn giờ sự kiện muốn xoá");
                    }
                    else
                    {
                        string strGiomuonXoa = Lich.SelectedDate.Value.ToString("dd-MM-yyyy") + " " + cbGioBD.Text + ":" + cbPhutBD.Text + ":00";
                        bool Deleted = false;
                        foreach (var gio in mdb.LenLichs.ToList())
                        {
                            if (gio.NgLenLichBd.Value.ToString("dd-MM-yyyy HH:mm:ss") == strGiomuonXoa)
                            {
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("d/M/yyyy");
                                SqlCommand cmd = new SqlCommand("set dateformat dmy\ndelete from LenLich where NgLenLichBD='" + strGiomuonXoa + "'", con);
                                cmd.ExecuteNonQuery();
                                DateTime DT = DateTime.Now;
                                cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(),'" + PageChinh.getMa + "', '" + DT.ToString("dd-MM-yyyy HH:mm:ss") + "', N'" + "xoá lịch cho ngày " + lich + "')", con);
                                cmd.ExecuteNonQuery();
                                con.Close();
                                MessageBox.Show("Xoá Sự Kiện thành công");
                                if (GetIso8601WeekOfYear(dt) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                                {
                                    soSuKien--;
                                    if (soSuKien != 0)
                                        txtblLoiNhac.Text = "".PadRight(5) + "Tuần Này Có:\n " + soSuKien.ToString() + " Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                                    else
                                        txtblLoiNhac.Text = "".PadRight(8) + "Tuần Này\n Không Có Sự Kiện\n Nào Đáng Chú Ý";
                                }
                                Deleted = true;
                                enableXoaLich = false;
                                enableLenLich = false;
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
            else
                MessageBox.Show("Vui lòng chọn lại ngày xoá lịch");
        }

        private void btnLichSu_Click(object sender, RoutedEventArgs e)
        {
            borderLichvaButton.Visibility = Visibility.Collapsed;
            dataGridLSHD.Visibility = Visibility.Visible;
            if (isFirstClicked)
            {
                listMaNV.Clear();
                listTenNV.Clear();
                listThoiGian.Clear();
                listHoatDong.Clear();
                dataGridLSHD.Items.Clear();
                //Mỗi lần First Click phải Clear hết 4 trường dữ liệu trên
                //Nếu kh thì hành động mới sẽ bị đẩy vào cuối Danh Sách
                con.Open();
                SqlCommand cmd = new SqlCommand("SET Dateformat dmy\nSelect MaNV From LichSuHoatDong Order by ThoiGian DESC", con);
                SqlDataReader sda = cmd.ExecuteReader();

                while (sda.Read())
                    listMaNV.Add((string)sda[0]);

                SqlCommand cmd1 = new SqlCommand("SET Dateformat dmy\nSelect Nhanvien.HoTenNV\r\nfrom NhanVien join LichSuHoatDong\r\non NhanVien.MaNV=LichSuHoatDong.MaNV\r\norder by ThoiGian DESC", con);
                SqlDataReader sda1 = cmd1.ExecuteReader();

                while (sda1.Read())
                    listTenNV.Add((string)sda1[0]);

                SqlCommand cmd2 = new SqlCommand("SELECT FORMAT(ThoiGian,'dd/MM/yyyy hh:mm:ss tt') FROM LichSuHoatDong Order by ThoiGian DESC", con);
                SqlDataReader sda2 = cmd2.ExecuteReader();

                while (sda2.Read())
                    listThoiGian.Add((string)sda2[0]);

                SqlCommand cmd3 = new SqlCommand("SET Dateformat dmy\nSelect HoatDong from LichSuHoatDong order by ThoiGian DESC", con);
                SqlDataReader sda3 = cmd3.ExecuteReader();
                while (sda3.Read())
                    listHoatDong.Add((string)sda3[0]);
                int TableLength = 0;
                SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM LichSuHoatDong", con);
                SqlDataReader sda4 = cmd4.ExecuteReader();
                if (sda4.Read())
                    TableLength = (int)sda4[0];
                for (int i = 0; i < TableLength; i++)
                {
                    dataGridLSHD.Items.Add(new LichSuHoatDong()
                    {
                        MaNV = listMaNV[i],
                        HoTenNV = listTenNV[i],
                        ThoiGian = listThoiGian[i],
                        HoatDong = listHoatDong[i],
                    });
                }
                isFirstClicked = false;
                con.Close();
            }
        }

        private void btnXoaLichSu_Click(object sender, RoutedEventArgs e)
        {
            var Result = MessageBox.Show("Bạn Có Chắc Muốn Xoá Lịch Sử Hoạt Động?", " ", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Delete from LichSuHoatDong", con);
                cmd.ExecuteNonQuery();
                DateTime now = DateTime.Now;
                cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getMa + "', '" + now.ToString("dd-MM-yyyy HH:mm:ss") + "', N'xoá lịch sử')", con);
                cmd.ExecuteNonQuery();
                dataGridLSHD.Items.Clear();
                MessageBox.Show("Xoá Lịch Sử Hoạt Động Thành Công!");
                con.Close();
            }
        }

        // Hàm khởi tạo DashboardPage, nên đặt tên khác cho dễ hiểu hơn
        private void DashboardPage_Initialize()
        {
            if (File.Exists("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa);
                image.EndInit();

                anhNhanVien.ImageSource = image;
            }
            else if (PageChinh.getSex == "Nữ")
                anhNhanVien.ImageSource = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNu.png"));
            else
                anhNhanVien.ImageSource = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNam.png"));

            if (PageChinh.getChucVu.ToLower() == "quản lý")
            {
                lblXinChao.Content = "  Xin Chào, " + PageChinh.getTen;
                lblChucVu.Content = "Nhân Viên Quản Lý";
                txtblSoNV.Text = "   Số Nhân Viên\n   Bạn Quản Lý:\n" + "".PadRight(12) + (mdb.NhanViens.Select(d => d.MaNv).Count() - 1).ToString();
                int? solgxe = mdb.MatHangs.Sum(d => d.SoLuongTonKho) - mdb.HoaDons.Sum(d => d.SoLuong);
                //Số xe hiện tại = Số Xe nhập về - Số xe bán đc
                txtblSoXe.Text = "".PadRight(9) + "Số Xe\n" + "".PadRight(5) + "Trong Kho:\n" + "".PadRight(11) + solgxe.ToString();
                txtblSoNV.FontSize = 20;
                txtblSoXe.FontSize = 20.5;
                btnLichSu.Visibility = Visibility.Visible;
                btnXoaLichSu.Visibility=Visibility.Visible;
                lblChucVu.Foreground = Brushes.Red;
            }
            else  //Nhân viên loại 2
            {
                lblXinChao.Content = "  Xin Chào, " + PageChinh.getTen;
                lblChucVu.Content = "Nhân Viên Văn Phòng";

                //3 dòng dưới để lấy ngày vào làm của nhân viên, tính số ngày từ đó đến nay và hiển thị nó
                var dx = mdb.NhanViens.Where(u => u.MaNv == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                int d3 = (int)(dt - dx).Value.TotalDays;
                txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d3.ToString() + " Ngày";

                var slg = mdb.HoaDons.Where(u => u.MaNv == PageChinh.getMa).Select(u => u.SoLuong).Sum();
                txtblSoXe.Text = "".PadRight(15) + slg.ToString() + "\n".PadRight(10) + "Là Số Xe\n" + "".PadRight(1) + "Bạn Bán Được";
                txtblSoNV.FontSize = 19;
                txtblSoXe.FontSize = 18.9;

                borderLichvaButton.Visibility = Visibility.Collapsed;
                btnLichSu.Visibility = Visibility.Collapsed;
                btnXoaLichSu.Visibility = Visibility.Collapsed;
                lblChucVu.Foreground = Brushes.White;
                //Với nhân viên văn phòng thì giấu quyền Lên, Xoá Lịch và xem Lịch Sử đi
            }
        }

        private async void btnCapNhatAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            //Cho IF vào TRY, Catch đc lỗi thì check xem có file Backup hay kh,
            //Nếu có xoá file ảnh hiện tại và đổi tên file Backup thành file ảnh hiệện tại
            //Nếu 0 có BaKUP thì quay lại hình Default
            //sau If sẽ hiện lỗi messagebox
            string destFile = "C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa + ".BackUp";
            string newPathToFile = "C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa;
            //destFile: file dự phòng
            //newPathToFile: file ảnh mới
            try
            {
                if (OFD.ShowDialog() == true)
                {
                    if (File.Exists(newPathToFile)) //Nếu có 1 file ảnh khác tồn tại thì xoá nó đi và cập nhật file ảnh mới
                    {
                        File.Move(newPathToFile, destFile); //Đổi tên File
                    }
                    File.Copy(OFD.FileName, newPathToFile); //Chỉnh tên File ảnh đc chọn

                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri(newPathToFile);
                    image.EndInit();
                    anhNhanVien.ImageSource = image;

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(destFile); //Xoá file tạm đi
                    MessageBox.Show("Cập nhật ảnh thành công!");
                    isFirstClicked = true;
                    con.Open();
                    DateTime now = DateTime.Now;
                    SqlCommand cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getMa + "', '" + now.ToString("dd-MM-yyyy HH:mm:ss") + "', N'cập nhật ảnh')", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch(Exception ex)
            {
                if(File.Exists(destFile))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(newPathToFile);

                    File.Move(destFile, newPathToFile);
                }
                else
                {
                    if (PageChinh.getSex == "Nữ")
                        anhNhanVien.ImageSource = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNu.png"));
                    else
                        anhNhanVien.ImageSource = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNam.png"));
                }
                MessageBox.Show(ex.Message);
            }
        }

        private void brdSoNV_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(15, 15, 0, 0);
        }

        private void brdSoNV_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(15, 30, 0, 0);
        }

        private void brdSoXe_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(15, 0, 0, 45);
        }

        private void brdSoXe_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(15, 0, 0, 30);
        }

        private void brdLoiNhac_MouseMove(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(0, 0, 20, 15);
        }

        private void brdLoiNhac_MouseLeave(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(0, 0, 20, 0);
        }

        private void border3thgtin_MouseMove(object sender, MouseEventArgs e)
        {
            border3thgtin.Opacity = 1;
        }

        private void border3thgtin_MouseLeave(object sender, MouseEventArgs e)
        {
            border3thgtin.Opacity = 0.8;
        }

        private void borderThgTinUser_MouseMove(object sender, MouseEventArgs e)
        {
            borderThgTinUser.Opacity = 1;
        }

        private void borderThgTinUser_MouseLeave(object sender, MouseEventArgs e)
        {
            borderThgTinUser.Opacity = 0.8;
        }

        private void borderLichvaButton_MouseMove(object sender, MouseEventArgs e)
        {
            borderLichvaButton.Opacity = 1;
        }

        private void borderLichvaButton_MouseLeave(object sender, MouseEventArgs e)
        {
            borderLichvaButton.Opacity = 0.8;
        }

        private void btnChamHoi_Click(object sender, RoutedEventArgs e)
        {
            borderHuongDan.Visibility = Visibility.Visible;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Views/Pages/Images/huongdanLich.png", UriKind.Relative);
            bi3.EndInit();
            anhHuongDan.Stretch = Stretch.Fill;
            anhHuongDan.Source = bi3;
        }

        private void btnDaHieu_Click(object sender, RoutedEventArgs e)
        {
            borderHuongDan.Visibility = Visibility.Collapsed;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Views/Pages/Images/huongdanLich2.png", UriKind.Relative);
            bi3.EndInit();
            anhHuongDan.Stretch = Stretch.Fill;
            anhHuongDan.Source = bi3;
        }

        private void brdSoNV_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(15, 15, 0, 0);
        }

        private void brdSoNV_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(15, 30, 0, 0);
        }

        private void brdSoXe_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(15, 0, 0, 45);
        }

        private void brdSoXe_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(15, 0, 0, 30);
        }

        private void brdLoiNhac_MouseMove(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(0, 0, 20, 15);
        }

        private void brdLoiNhac_MouseLeave(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(0, 0, 20, 0);
        }

        private void border3thgtin_MouseMove(object sender, MouseEventArgs e)
        {
            border3thgtin.Opacity = 1;
        }

        private void border3thgtin_MouseLeave(object sender, MouseEventArgs e)
        {
            border3thgtin.Opacity = 0.8;
        }

        private void borderThgTinUser_MouseMove(object sender, MouseEventArgs e)
        {
            borderThgTinUser.Opacity = 1;
        }

        private void borderThgTinUser_MouseLeave(object sender, MouseEventArgs e)
        {
            borderThgTinUser.Opacity = 0.8;
        }

        private void borderLichvaButton_MouseMove(object sender, MouseEventArgs e)
        {
            borderLichvaButton.Opacity = 1;
        }

        private void borderLichvaButton_MouseLeave(object sender, MouseEventArgs e)
        {
            borderLichvaButton.Opacity = 0.8;
        }
    }
}