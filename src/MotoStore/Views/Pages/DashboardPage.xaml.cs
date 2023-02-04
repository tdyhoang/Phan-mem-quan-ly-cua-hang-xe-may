using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MotoStore.Views.Pages.LoginPages;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using MotoStore.Properties;
using MotoStore.Helpers;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage
    {
        // Nên đặt tên biến thể hiện rõ chức năng của nó. Tên biến hiện tại quá chung chung
        private int soSuKien = 0;  //Biến đếm số sự kiện trong tuần
        private int danhdau = 0;   //Biến đánh dấu lời nhắc số sự kiện
        private bool enableLenLich;
        /*Biến cho phép lên lịch, tránh trường hợp ng dùng nhấn lại nút lên lịch
        sau khi đã lên lịch thành công mà ứng dụng vẫn lên lịch*/
        private bool enableXoaLich;
        //Biến cho phép xoá lịch, công dụng tương tự như trên
        private bool isFirstClicked = true;
        static int solanbam = 0;

        List<string> listMaNV { get; set; } = new();
        List<string> listTenNV { get; set; } = new();
        List<string> listThoiGian { get; set; } = new();
        List<string> listHoatDong { get; set; } = new();

        public DashboardPage(PageChinh pgC)
        {
            InitializeComponent();
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public class LichSuHoatDong
        {
            public string? MaNV { get; set; }
            public string? HoTenNV { get; set; }
            public string? ThoiGian { get; set; }
            public string? HoatDong { get; set; }
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainDatabase mdb = new();
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
                    if (GetIso8601WeekOfYear(DateTime.Now) == GetIso8601WeekOfYear(demngay.NgLenLichBd))
                    {
                        soSuKien++;   //Vì nó là biến toàn cục nên cứ thế mà tăng 
                        //Đánh dấu = 1 để hạn chế những lần load trang Dashboard sau nó tự động tăng số sự kiện 
                    }
                }
                danhdau = 1;
            }
            if (soSuKien == 0)
                txtblLoiNhac.Text = $"{"",-8}Tuần Này\n Không Có Sự Kiện\n Nào Đáng Chú Ý";
            else
                txtblLoiNhac.Text = $"{"",-5}Tuần Này Có:\n {soSuKien} Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
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
            lblGioHeThong.Content = $"Bây giờ là: {DateTime.Now:HH:mm:ss}";
        }

        private void btnDgXuat_Click(object sender, RoutedEventArgs e)
        {
            dataGridLSHD.Items.Clear(); //Clear dữ liệu Lịch Sử, khi nào 1st Click thì đổ dữ liệu lại sau
            dataGridLSHD.Visibility = Visibility.Collapsed;
            btnXoaLichSu.Visibility = Visibility.Collapsed;
            rtbNoiDung.Visibility = Visibility.Collapsed;
            isFirstClicked = true;

            SqlCommand cmd;
            using SqlConnection con = new(Settings.Default.ConnectionString);
            con.Open();
            DateTime DT = DateTime.Now;
            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DT:dd-MM-yyyy HH:mm:ss}', N'đăng xuất')", con);
            cmd.ExecuteNonQuery();

            Application.Current.MainWindow.Visibility = Visibility.Collapsed;  //Ẩn Màn hình chính đi
            Windows.LoginView loginView = new();
            loginView.Show();           //Hiện màn hình đăng nhập
        }

        private void Lich_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            rtbNoiDung.Visibility = Visibility.Visible;
            if (string.Equals(PageChinh.getNV.ChucVu, "Quản lý", StringComparison.OrdinalIgnoreCase))
            {
                enableLenLich = true;    //Cho phép lên lịch mỗi lần click chuột vào ngày bất kì trên Lịch
                enableXoaLich = true;
                borderLichvaButton.Visibility = Visibility.Visible;
                dataGridLSHD.Visibility = Visibility.Collapsed;
            }
            bool co = false;   //biến này để check xem có sự kiện nổi bật hay không
            rtbNoiDung.Document.Blocks.Clear(); //Clear hết nội dung mỗi lần bấm, sau đó đổ lại nội dung vào
            //Sắp xếp sự kiện
            List<string> listNgBD = new();
            List<string> listNgKT = new();
            List<string> listNoiDung = new();
            using SqlConnection con = new(Settings.Default.ConnectionString);
            con.Open();
            SqlCommand cmd = new("SELECT CONVERT(VARCHAR(8),NgLenLichBD,108) from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda = cmd.ExecuteReader();
            while (sda.Read())
                listNgBD.Add((string)sda[0]);

            SqlCommand cmd2 = new("SELECT CONVERT(VARCHAR(8),NgLenLichKT,108) from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda2 = cmd2.ExecuteReader();
            while (sda2.Read())
                listNgKT.Add((string)sda2[0]);

            SqlCommand cmd3 = new("SELECT NoiDungLenLich from LenLich order by NgLenLichBD ASC", con);
            SqlDataReader sda3 = cmd3.ExecuteReader();
            while (sda3.Read())
                listNoiDung.Add((string)sda3[0]);

            int TabLength = 0;
            SqlCommand cmd4 = new("SELECT Count(*) from LenLich", con);
            SqlDataReader sda4 = cmd4.ExecuteReader();
            while (sda4.Read())
                TabLength = (int)sda4[0];

            for (int i = 0; i < TabLength; i++) 
            {
                if (Lich.SelectedDate == DateTime.ParseExact(listNgBD[i], "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    rtbNoiDung.AppendText($"Bắt Đầu: {listNgBD[i]} - Kết Thúc: {listNgKT[i]}\nNội Dung: {listNoiDung[i]}\n");
                    co = true;
                }
            }
            if (co == false)
                rtbNoiDung.AppendText("Không có sự kiện nổi bật");
            /*Hàm này để khi ng quản lý bấm vào ngày
            bất kì trên lịch sẽ hiện nội dung(Nếu có)*/
        }

        private void btnHuy_Click(object sender, RoutedEventArgs e)
            => borderLichvaButton.Visibility = Visibility.Collapsed;
            //Button Hủy tương tác với Lịch

        // Event Handler cho VisibleChanged
        private void DashboardPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)  //(bool)e.NewValue == true thì có nghĩa là Ứng dụng đc hiện lên
            {
                DashboardPage_Initialize();
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
                DateTime ngaylenlich = DateTime.ParseExact($"{Lich.SelectedDate:dd/MM/yyyy} {cbGioBD.Text}:{cbPhutBD.Text}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                //if có ngày được select
                if (Lich.SelectedDate.HasValue)
                {
                    string richText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
                    if (string.IsNullOrEmpty(cbGioBD.Text) || string.IsNullOrEmpty(cbPhutBD.Text) || string.IsNullOrEmpty(cbGioKT.Text) || string.IsNullOrEmpty(cbPhutKT.Text))
                        MessageBox.Show("Vui lòng chọn giờ cụ thể");
                    else if (ngaylenlich < DateTime.Now)
                    {
                        MessageBox.Show("Ngày hoặc giờ bắt đầu lên lịch không hợp lệ");
                    }
                    else if (string.IsNullOrWhiteSpace(richText))
                    {
                        MessageBox.Show("Vui lòng ghi nội dung");
                    }
                    else
                    {
                        if (int.Parse(cbGioBD.Text) >= int.Parse(cbGioKT.Text) && int.Parse(cbPhutBD.Text) >= int.Parse(cbPhutKT.Text))
                            MessageBox.Show("Giờ kết thúc không thể nhỏ hơn hoặc bằng giờ bắt đầu");
                        else
                        {
                            MainDatabase mdb = new();
                            //Không cho phép một sự kiện mới đc chen vào giữa giờ 1 sự kiện khác
                            //Hoặc khoảng thời gian sự kiện mới lại chứa các sự kiện khác
                            foreach (var item in mdb.LenLichs.ToList())
                            {
                                if (item.NgLenLichBd == Lich.SelectedDate.Value)
                                {
                                    DateTime gioBD = DateTime.ParseExact($"{cbGioBD.Text}:{cbPhutBD.Text}:00", "HH:mm:ss", CultureInfo.InvariantCulture);
                                    DateTime gioKT = DateTime.ParseExact($"{cbGioKT.Text}:{cbPhutKT.Text}:00", "HH:mm:ss", CultureInfo.InvariantCulture);
                                    if (item.NgLenLichBd <= gioBD && gioBD <= item.NgLenLichKt || item.NgLenLichBd <= gioKT && gioKT <= item.NgLenLichKt || item.NgLenLichBd >= gioBD && gioKT >= item.NgLenLichKt)
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                            }
                            if (valid)
                            {
                                using SqlConnection con = new(Settings.Default.ConnectionString);
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy");
                                //Giải thích dòng trên:
                                //Vì Lich.SelectedDate.Value sẽ cho ra ngày/tháng/năm + giờ/phút/giây nên ta lược bớt phần sau (chỉ giữ lại ngày tháng năm)

                                SqlCommand cmd = new($"set dateformat dmy\nInsert into LenLich values(NewID(),'{PageChinh.getNV.MaNv}', '{lich} {cbGioBD.Text}:{cbPhutBD.Text}:00', '{lich} {cbGioKT.Text}:{cbPhutKT.Text}:00', N'{richText}')", con);
                                cmd.ExecuteNonQuery();
                                DateTime DT = DateTime.Now;
                                cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NewID(),'{PageChinh.getNV.MaNv}', '{DT:dd-MM-yyyy HH:mm:ss}', N'lên lịch cho ngày {lich}')", con);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Lên lịch thành công!");
                                if (GetIso8601WeekOfYear(DateTime.Now) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                                {
                                    soSuKien++;
                                    txtblLoiNhac.Text = $"{"",-5}Tuần Này Có:\n {soSuKien} Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                                }
                                cbGioBD.Text = string.Empty;
                                cbPhutBD.Text = string.Empty;
                                cbGioKT.Text = string.Empty;
                                cbPhutKT.Text = string.Empty;
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
            MainDatabase mdb = new();
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
                        DateTime giomuonxoa = DateTime.ParseExact($"{Lich.SelectedDate:dd/MM/yyyy} {cbGioBD.Text}:{cbPhutBD.Text}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        bool Deleted = false;
                        foreach (var gio in mdb.LenLichs.ToList())
                        {
                            if (gio.NgLenLichBd == giomuonxoa)
                            {
                                using SqlConnection con = new(Settings.Default.ConnectionString);
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy");
                                SqlCommand cmd = new($"set dateformat dmy\ndelete from LenLich where NgLenLichBD=@NgayLenLichBD", con);
                                cmd.Parameters.Add("@NgayLenLichBD", System.Data.SqlDbType.SmallDateTime).Value = giomuonxoa;
                                cmd.ExecuteNonQuery();
                                DateTime DT = DateTime.Now;
                                cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(),'{PageChinh.getNV.MaNv}', '{DT:dd-MM-yyyy HH:mm:ss}', N'xoá lịch cho ngày {lich}')", con);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Xoá Sự Kiện thành công");
                                if (GetIso8601WeekOfYear(DateTime.Now) == GetIso8601WeekOfYear(Lich.SelectedDate.Value))
                                {
                                    soSuKien--;
                                    if (soSuKien != 0)
                                        txtblLoiNhac.Text = $"{"",-5}Tuần Này Có:\n {soSuKien} Sự Kiện Đáng\n Chú Ý, Xem Chi\n Tiết Ở Lịch!";
                                    else
                                        txtblLoiNhac.Text = $"{"",-8}Tuần Này\n Không Có Sự Kiện\n Nào Đáng Chú Ý";
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
            if(solanbam == 0)
            {
                borderLichvaButton.Visibility = Visibility.Collapsed;
                dataGridLSHD.Visibility = Visibility.Visible;
                solanbam = 1;
                if (isFirstClicked)
                {
                    listMaNV.Clear();
                    listTenNV.Clear();
                    listThoiGian.Clear();
                    listHoatDong.Clear();
                    dataGridLSHD.Items.Clear();
                    //Mỗi lần First Click phải Clear hết 4 trường dữ liệu trên
                    //Nếu kh thì hành động mới sẽ bị đẩy vào cuối Danh Sách
                    using SqlConnection con = new(Settings.Default.ConnectionString);
                    con.Open();
                    SqlCommand cmd = new("SET Dateformat dmy\nSelect MaNV From LichSuHoatDong Order by ThoiGian DESC", con);
                    SqlDataReader sda = cmd.ExecuteReader();

                    while (sda.Read())
                        listMaNV.Add((string)sda[0]);

                    SqlCommand cmd1 = new("SET Dateformat dmy\nSelect Nhanvien.HoTenNV\r\nfrom NhanVien join LichSuHoatDong\r\non NhanVien.MaNV=LichSuHoatDong.MaNV\r\norder by ThoiGian DESC", con);
                    SqlDataReader sda1 = cmd1.ExecuteReader();

                    while (sda1.Read())
                        listTenNV.Add((string)sda1[0]);

                    SqlCommand cmd2 = new("SELECT FORMAT(ThoiGian,'dd/MM/yyyy hh:mm:ss tt') FROM LichSuHoatDong Order by ThoiGian DESC", con);
                    SqlDataReader sda2 = cmd2.ExecuteReader();

                    while (sda2.Read())
                        listThoiGian.Add((string)sda2[0]);

                    SqlCommand cmd3 = new("SET Dateformat dmy\nSelect HoatDong from LichSuHoatDong order by ThoiGian DESC", con);
                    SqlDataReader sda3 = cmd3.ExecuteReader();
                    while (sda3.Read())
                        listHoatDong.Add((string)sda3[0]);

                    int TableLength = 0;
                    SqlCommand cmd4 = new("SELECT COUNT(*) FROM LichSuHoatDong", con);
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
                }
            }
            else
            {
                dataGridLSHD.Visibility = Visibility.Collapsed;
                borderLichvaButton.Visibility = Visibility.Visible;
                solanbam = 0;
                isFirstClicked = true;
            }
        }

        private void btnXoaLichSu_Click(object sender, RoutedEventArgs e)
        {
            var Result = MessageBox.Show("Bạn Có Chắc Muốn Xoá Lịch Sử Hoạt Động?", " ", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (Result == MessageBoxResult.Yes)
            {
                using SqlConnection con = new(Settings.Default.ConnectionString);
                con.Open();
                SqlCommand cmd = new("Delete from LichSuHoatDong", con);
                cmd.ExecuteNonQuery();
                DateTime now = DateTime.Now;
                cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{now:dd-MM-yyyy HH:mm:ss}', N'xoá lịch sử')", con);
                cmd.ExecuteNonQuery();
                dataGridLSHD.Items.Clear();
                MessageBox.Show("Xoá Lịch Sử Hoạt Động Thành Công!");
            }
        }

        // Hàm khởi tạo DashboardPage, nên đặt tên khác cho dễ hiểu hơn
        private void DashboardPage_Initialize()
        {
            MainDatabase mdb = new();
            var seperatedHoTenNV = PageChinh.getNV.HoTenNv.Split(' ');
            var tenNV = seperatedHoTenNV[^1];
            //2 dòng trên lấy tên nhân viên và gán nó cho biến getTen (VD: Phan Tấn Trung => getTen = Trung)
            var filename = Path.Combine(Settings.Default.AvatarFilePath, PageChinh.getNV.MaNv);
            List<string> filepaths = new() { $"{filename}.png", $"{filename}.jpg", $"{filename}.jpeg" };

            if (PageChinh.getNV.GioiTinh == "Nữ")
                anhNhanVien.ImageSource = new BitmapImage(new("pack://application:,,,/Avatars/userNu.png"));
            else
                anhNhanVien.ImageSource = new BitmapImage(new("pack://application:,,,/Avatars/userNam.png"));
            foreach (var path in filepaths)
                if (File.Exists(path))
                {
                    anhNhanVien.ImageSource = BitmapConverter.FilePathToBitmapImage(path);
                    break;
                }

            if (string.Equals(PageChinh.getNV.ChucVu, "quản lý", StringComparison.OrdinalIgnoreCase))
            {
                lblXinChao.Content = $"  Xin Chào, {tenNV}";
                lblChucVu.Content = "Nhân Viên Quản Lý";
                txtblSoNV.Text = $"   Số Nhân Viên\n   Bạn Quản Lý:\n{"",-12}{mdb.NhanViens.Select(d => d.MaNv).Count() - 1}";
                int? solgxe = mdb.MatHangs.Sum(d => d.SoLuongTonKho) - mdb.HoaDons.Sum(d => d.SoLuong);
                //Số xe hiện tại = Số Xe nhập về - Số xe bán đc
                txtblSoXe.Text = $"{"",-9}Số Xe\n{"",-5}Trong Kho:\n{"",-11}{solgxe}";
                txtblSoNV.FontSize = 20;
                txtblSoXe.FontSize = 20.5;
                btnLichSu.Visibility = Visibility.Visible;
                btnXoaLichSu.Visibility=Visibility.Visible;
                lblChucVu.Foreground = Brushes.Red;
            }
            else  //Nhân viên thường
            {
                lblXinChao.Content = $"  Xin Chào, {tenNV}";
                lblChucVu.Content = "Nhân Viên Văn Phòng";

                //3 dòng dưới để lấy ngày vào làm của nhân viên, tính số ngày từ đó đến nay và hiển thị nó
                DateTime dx = mdb.NhanViens.Where(u => u.MaNv == PageChinh.getNV.MaNv).Select(u => u.NgVl).FirstOrDefault() ?? DateTime.Today;
                var d3 = (int)(DateTime.Today - dx).TotalDays;
                txtblSoNV.Text = $" Bạn Đã Gắn Bó\n Với Chúng Tôi:\n{"",-6}{d3} Ngày";

                var slg = mdb.HoaDons.Where(u => u.MaNv == PageChinh.getNV.MaNv).Select(u => u.SoLuong).Sum();
                txtblSoXe.Text = $"{"",-15}{slg}{"\n",-10}Là Số Xe\n{"",-1}Bạn Bán Được";
                txtblSoNV.FontSize = 19;
                txtblSoXe.FontSize = 18.9;

                borderLichvaButton.Visibility = Visibility.Collapsed;
                btnLichSu.Visibility = Visibility.Collapsed;
                btnXoaLichSu.Visibility = Visibility.Collapsed;
                lblChucVu.Foreground = Brushes.White;
                //Với nhân viên văn phòng thì giấu quyền Lên, Xoá Lịch và xem Lịch Sử đi
            }
        }

        private void btnCapNhatAvatar_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog OFD = new();
            OFD.Filters.Add(new("Image File", "jpg,jpeg,png"));
            //Cho IF vào TRY, Catch đc lỗi thì check xem có file Backup hay kh,
            //Nếu có xoá file ảnh hiện tại và đổi tên file Backup thành file ảnh hiệện tại
            //Nếu 0 có BaKUP thì quay lại hình Default
            //sau If sẽ hiện lỗi messagebox
            //string backupFile = @$"/Avatars/{PageChinh.getNV.MaNv}.BackUp";
            //string genericFilePath = @$"/Avatars/{PageChinh.getNV.MaNv}";
            string genericFilePath = Path.Combine(Settings.Default.AvatarFilePath, PageChinh.getNV.MaNv);
            // Tên của ảnh cũ có thể thuộc 1 trong 3 đuôi file này
            List<string> possiblePathsToOldFile = new() { $"{genericFilePath}.png", $"{genericFilePath}.jpg", $"{genericFilePath}.jpeg" };
            // Lấy tên của ảnh cũ (nhằm backup lại khi copy file thất bại)
            string pathToOldFile = string.Empty;
            //backupFile: file dự phòng
            //genericFilePath: file ảnh mới
            try
            {
                if (OFD.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach(var path in possiblePathsToOldFile)
                        if (File.Exists(path)) //Nếu có 1 file ảnh khác tồn tại thì xoá nó đi và cập nhật file ảnh mới
                        {
                            pathToOldFile = path;
                            File.Delete(path + ".bak"); // Xóa trước phòng trường hợp có sẵn file trùng tên
                            File.Move(path, path + ".bak"); // Backup file
                            break;
                        }
                    // File đích là tên file + đuôi file của ảnh được chọn
                    File.Copy(OFD.FileName, genericFilePath + Path.GetExtension(OFD.FileName));

                    anhNhanVien.ImageSource = BitmapConverter.FilePathToBitmapImage(genericFilePath + Path.GetExtension(OFD.FileName));

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(pathToOldFile + ".bak"); //Xoá file tạm đi
                    MessageBox.Show("Cập nhật ảnh thành công!");
                    isFirstClicked = true;
                    using SqlConnection con = new(Settings.Default.ConnectionString);
                    con.Open();
                    SqlCommand cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'cập nhật ảnh')", con);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Trường hợp gặp lỗi thì sẽ phục hồi file backup (nếu có)
                if (File.Exists(pathToOldFile + ".bak"))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    File.Delete(pathToOldFile);
                    File.Move(pathToOldFile + ".bak", pathToOldFile);
                }
                else
                {
                    if (PageChinh.getNV.GioiTinh == "Nữ")
                        anhNhanVien.ImageSource = new BitmapImage(new("pack://application:,,,/Avatars/userNu.png"));
                    else
                        anhNhanVien.ImageSource = new BitmapImage(new("pack://application:,,,/Avatars/userNam.png"));
                }
                MessageBox.Show(ex.Message);
            }
        }

        private void brdSoNV_MouseMove(object sender, MouseEventArgs e)
            => brdSoNV.Margin = new(15, 15, 0, 0);

        private void brdSoNV_MouseLeave(object sender, MouseEventArgs e)
            => brdSoNV.Margin = new(15, 30, 0, 0);

        private void brdSoXe_MouseMove(object sender, MouseEventArgs e)
            => brdSoXe.Margin = new(15, 0, 0, 45);

        private void brdSoXe_MouseLeave(object sender, MouseEventArgs e)
            => brdSoXe.Margin = new(15, 0, 0, 30);

        private void brdLoiNhac_MouseMove(object sender, MouseEventArgs e)
            => brdLoiNhac.Margin = new(0, 0, 20, 15);

        private void brdLoiNhac_MouseLeave(object sender, MouseEventArgs e)
            => brdLoiNhac.Margin = new(0, 0, 20, 0);

        private void border3thgtin_MouseMove(object sender, MouseEventArgs e)
            => border3thgtin.Opacity = 1;

        private void border3thgtin_MouseLeave(object sender, MouseEventArgs e)
            => border3thgtin.Opacity = 0.8;

        private void borderThgTinUser_MouseMove(object sender, MouseEventArgs e)
            => borderThgTinUser.Opacity = 1;

        private void borderThgTinUser_MouseLeave(object sender, MouseEventArgs e)
            => borderThgTinUser.Opacity = 0.8;

        private void borderLichvaButton_MouseMove(object sender, MouseEventArgs e)
            => borderLichvaButton.Opacity = 1;

        private void borderLichvaButton_MouseLeave(object sender, MouseEventArgs e)
            => borderLichvaButton.Opacity = 0.8;

        private void btnChamHoi_Click(object sender, RoutedEventArgs e)
        {
            borderHuongDan.Visibility = Visibility.Visible;
            BitmapImage bi3 = new();
            bi3.BeginInit();
            bi3.UriSource = new("pack://application:,,,/Views/Pages/Images/huongdanLich.png");
            bi3.EndInit();
            anhHuongDan.Stretch = Stretch.Fill;
            anhHuongDan.Source = bi3;
        }

        private void btnDaHieu_Click(object sender, RoutedEventArgs e)
            => borderHuongDan.Visibility = Visibility.Collapsed;

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new();
            bi3.BeginInit();
            bi3.UriSource = new("pack://application:,,,/Views/Pages/Images/huongdanLich2.png");
            bi3.EndInit();
            anhHuongDan.Stretch = Stretch.Fill;
            anhHuongDan.Source = bi3;
        }
    }
}