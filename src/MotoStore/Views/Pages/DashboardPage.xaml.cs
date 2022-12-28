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
        private RichTextBox rtb;
        private ComboBox cbGioBD;
        private ComboBox cbPhutBD;
        private ComboBox cbGioKT;
        private ComboBox cbPhutKT;
        private DateTime dt = DateTime.Now;
        private int soSuKien = 0;  //Biến đếm số sự kiện trong tuần
        private int danhdau = 0;   //Biến đánh dấu lời nhắc số sự kiện
        private bool enableLenLich;
        /*Biến cho phép lên lịch, tránh trường hợp ng dùng nhấn lại nút lên lịch
        sau khi đã lên lịch thành công mà ứng dụng vẫn lên lịch*/
        private bool enableXoaLich;
        //Biến cho phép xoá lịch, công dụng tương tự như trên
        
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
            if (danhdau == 0)  //danhdau=0 có nghĩa là DashboardPage lần đầu được khởi tạo
            {
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
            stkbtnLichSu.Children.Clear();
            /*5 dòng trên để:
            Xoá ô Nội Dung
            Xoá ô Lên Lịch
            Xoá button Lên Lịch, Xoá Lịch(Dành cho Quản Lý)
            Xoá button Lịch Sử mỗi lần đăng xuất*/

            SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
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
            enableLenLich = true;    //Cho phép lên lịch mỗi lần click chuột vào ngày bất kì trên Lịch
            enableXoaLich = true;
            stkNoiDung.Children.Clear();  
            RichTextBox rtbNoiDung = new RichTextBox();
            rtbNoiDung.Height = 150;
            rtbNoiDung.Width = 250;
            rtbNoiDung.Foreground = Brushes.Red;
            rtbNoiDung.FontSize = 11;
            rtbNoiDung.IsReadOnly = true;
            stkNoiDung.Children.Add(rtbNoiDung);
            MainDatabase mainDatabase = new MainDatabase();
            bool co = false;   //biến này để check xem có sự kiện nổi bật hay không
            foreach (var ngay in mainDatabase.LenLichs.ToList())  
            {
                if (Lich.SelectedDate.Value.ToString("dd/MM/yyyy") == ngay.NgLenLichBd.Value.ToString("dd/MM/yyyy"))
                {
                    rtbNoiDung.AppendText("Bắt Đầu: " + ngay.NgLenLichBd.Value.ToString("HH:mm") + " - Kết Thúc: " + ngay.NgLenLichKt.Value.ToString("HH:mm") + "\n" + "Nội Dung: " + ngay.NoiDungLenLich + "\n");
                    co = true;
                }
            }
            if (co==false)
                rtbNoiDung.AppendText("Không có sự kiện nổi bật");

            if (PageChinh.getChucVu.ToLower()== "quản lý")   //Tất nhiên ng Quản Lý mới có quyền lên lịch
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
                    cbGioBD.Items.Add(i.ToString("D2"));
                cbPhutBD = new ComboBox();
                cbPhutBD.Height = 40;
                cbPhutBD.Width = 70;
                cbPhutBD.Margin = new Thickness(170, -40, 0, 0);
                for (int i = 0; i <= 59; i++)
                    cbPhutBD.Items.Add(i.ToString("D2"));
                stkLich.Children.Add(cbGioBD);
                stkLich.Children.Add(cbPhutBD);
                Label lblBatDau = new Label();
                lblBatDau.Content = "Giờ Bắt Đầu: ";
                lblBatDau.Height = 30;
                lblBatDau.Width = 85;
                lblBatDau.Margin = new Thickness(-155, -20, 0, 0);
                lblBatDau.FontSize = 14;
                lblBatDau.FontWeight = FontWeights.Medium;
                lblBatDau.Foreground = Brushes.Black;
                stkLich.Children.Add(lblBatDau);
                Label lblKetThuc = new Label();
                lblKetThuc.Content = "Giờ Kết Thúc: ";
                lblKetThuc.Height = 30;
                lblKetThuc.Width = 85;
                lblKetThuc.Margin = new Thickness(-155, 35, 0, 0);
                lblKetThuc.FontSize = 14;
                lblKetThuc.FontWeight = FontWeights.Medium;
                lblKetThuc.Foreground = Brushes.Black;
                stkLich.Children.Add(lblKetThuc);
                cbGioKT = new ComboBox();
                cbGioKT.Height = 40;
                cbGioKT.Width = 70;
                cbGioKT.Margin = new Thickness(15, -55, 0, 0);
                for (int i = 0; i <= 23; i++)
                    cbGioKT.Items.Add(i.ToString("D2"));
                cbPhutKT = new ComboBox();
                cbPhutKT.Height = 40;
                cbPhutKT.Width = 70;
                cbPhutKT.Margin = new Thickness(170, -55, 0, 0);
                for (int i = 0; i <= 59; i++)
                    cbPhutKT.Items.Add(i.ToString("D2"));
                //Các dòng tiếp theo tạo các khung giờ và các nhãn

                Button btnLenLich = new Button();
                btnLenLich.Height = 40;
                btnLenLich.Width = 110;
                btnLenLich.Content = "Lên Lịch";
                btnLenLich.Foreground = Brushes.Black;
                btnLenLich.Background = Brushes.Aquamarine;
                btnLenLich.FontSize = 19;
                btnLenLich.FontWeight = FontWeights.Medium;
                btnLenLich.Click += btnLenLich_Click;

                Button btnXoaLich = new Button();
                btnXoaLich.Height = 40;
                btnXoaLich.Width = 110;
                btnXoaLich.Content = "Xoá Lịch";
                btnXoaLich.Foreground = Brushes.Black;
                btnXoaLich.Background = Brushes.DeepSkyBlue;
                btnXoaLich.FontSize = 19;
                btnXoaLich.FontWeight = FontWeights.Medium;
                btnXoaLich.Click += btnXoaLich_Click;

                Button btnHuy = new Button();
                btnHuy.Height = 30;
                btnHuy.Width = 100;
                btnHuy.Content = "Hủy";
                btnHuy.FontSize = 11;
                btnHuy.FontWeight = FontWeights.Medium;
                btnHuy.Margin = new Thickness(115, -5, 0, 15);
                btnHuy.Background = Brushes.LightSkyBlue;
                btnHuy.Click += BtnHuy_Click;

                stkLich.Children.Add(cbGioKT);
                stkLich.Children.Add(cbPhutKT);
                stkbtnLenLich.Children.Add(btnLenLich);
                stkbtnXoaLich.Children.Add(btnXoaLich);
                stkLich.Children.Add(btnHuy);
                //Tạo 3 button Thêm, Xoá, Huỷ và thêm nó vào StackPanel
            }

            /*Hàm này hơi dài, nhưng tóm gọn lại là
              có chức năng Lên Lịch và Xoá Lịch dành 
              cho người Quản Lý mỗi khi click chuột
              vào ngày bất kì trên tờ Lịch
            */
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            stkLich.Children.Clear();
            stkbtnLenLich.Children.Clear();
            stkbtnXoaLich.Children.Clear();
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
            bool valid=true; //Biến này để check xem khoảng thời gian lên lịch có hợp lệ hay không

            /*Một khoảng thời gian lên lịch đc xem là hợp lệ 
              NẾU nó không chen vào khoảng thgian sự kiện khác
              HOẶC nó không chứa khoảng thời gian sự kiện khác*/

            if (enableLenLich)
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
                            //Không cho phép một sự kiện mới đc chen vào giữa giờ 1 sự kiện khác
                            //Hoặc khoảng thời gian sự kiện mới lại chứa các sự kiện khác
                            foreach (var item in mdb.LenLichs.ToList())
                            {
                                if(item.NgLenLichBd.Value.ToString("dd-MM-yyyy")==Lich.SelectedDate.Value.ToString("dd-MM-yyyy"))
                                {
                                    string gioBD = int.Parse(cbGioBD.Text) + ":" + int.Parse(cbPhutBD.Text) + ":00";
                                    string gioKT = int.Parse(cbGioKT.Text) + ":" + int.Parse(cbPhutKT.Text) + ":00";
                                    if (DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss"))<=DateTime.Parse(gioBD)&&DateTime.Parse(gioBD)<=DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss"))|| DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss"))<= DateTime.Parse(gioKT)&& DateTime.Parse(gioKT)<= DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss"))|| DateTime.Parse(item.NgLenLichBd.Value.ToString("HH:mm:ss")) >= DateTime.Parse(gioBD) && DateTime.Parse(gioKT) >= DateTime.Parse(item.NgLenLichKt.Value.ToString("HH:mm:ss")))
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                            }
                            if (valid)
                            {
                                SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                                /*Dòng trên ae cop cái connection string của máy mình thay cho cái dòng trên
                                Nhớ chạy lại dòng lệnh databasePMQLCHBXM.sql trên máy mình*/

                                SqlCommand cmd;
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy");
     

                                //Giải thích dòng trên:
                                //Vì SelectedDate.Value sẽ cho ra ngày/tháng/năm + giờ/phút/giây nên ta lược bớt phần sau (chỉ giữ lại ngày tháng năm)

                                cmd = new SqlCommand("set dateformat dmy\nInsert into LenLich values(NewID(),'" + PageChinh.getMa.ToString() + "', '" + lich + " " + cbGioBD.Text + ":" + cbPhutBD.Text + ":00" + "', '" + lich + " " + cbGioKT.Text + ":" + cbPhutKT.Text + ":00" + "', N'" + richText + "')", con);
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
                                stkLich.Children.Clear();
                                stkNoiDung.Children.Clear();

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
            if(enableXoaLich)
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
                                SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                                SqlCommand cmd;
                                con.Open();
                                string lich = Lich.SelectedDate.Value.ToString("dd-MM-yyyy");
                                cmd = new SqlCommand("set dateformat dmy\ndelete from LenLich where NgLenLichBD='" + strGiomuonXoa + "'", con);
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
                                stkLich.Children.Clear();
                                stkNoiDung.Children.Clear();
                                Deleted = true;
                                enableXoaLich = false; 
                                enableLenLich=false;
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
            stkbtnLenLich.Children.Clear();
            stkbtnXoaLich.Children.Clear();
            stkLich.Children.Clear();

            RichTextBox rtbLichSu = new RichTextBox();
            rtbLichSu.IsReadOnly = true;
            rtbLichSu.Height = 240;
            rtbLichSu.Width = 240;
            rtbLichSu.FontSize = 15;
            rtbLichSu.FontWeight = FontWeights.Medium;
            stkLich.Children.Add(rtbLichSu);
            rtbLichSu.AppendText("------Lịch Sử Hoạt Động------\n");
            foreach(var lshd in mdb.LichSuHoatDongs.ToList())
            {
                string time = lshd.ThoiGian.Value.ToString("dd-MM-yyyy HH:mm:ss");
                rtbLichSu.AppendText(time+"\n");

                var hoTenNV = mdb.NhanViens.Where(u => u.MaNv.ToString() == lshd.MaNv.ToString()).Select(u => u.HoTenNv).FirstOrDefault().ToString();
                var seperatedHoTenNV = hoTenNV.Split(' ');
                var tenNV = seperatedHoTenNV[seperatedHoTenNV.Length - 1];
                rtbLichSu.AppendText("Nhân viên " + tenNV +" "+ lshd.HoatDong+"\n");
            }
        }

        // Hàm khởi tạo DashboardPage, nên đặt tên khác cho dễ hiểu hơn
        private void DashboardPage_Initialize()
        {
            //if (File.Exists("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa))
              //  anhNhanVien.Source = new BitmapImage(new Uri("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\" + PageChinh.getMa + ".png"));
           // else
            //{
                if (PageChinh.getSex == "Nữ")
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNu.png"));
                else
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\Images\\userNam.png"));
            //}

            if (PageChinh.getChucVu.ToLower() == "quản lý")
            {
                lblXinChao.Content = "Xin Chào, " + PageChinh.getTen;
                lblChucVu.Content = "Nhân Viên Quản Lý";
                txtblSoNV.Text = "   Số Nhân Viên\n   Bạn Quản Lý:\n" + "".PadRight(12) + (mdb.NhanViens.Select(d => d.MaNv).Count() - 1).ToString();
                int? solgxe = mdb.MatHangs.Sum(d => d.SoLuongTonKho) - mdb.HoaDons.Sum(d => d.SoLuong);
                //Số xe hiện tại = Số Xe nhập về - Số xe bán đc
                txtblSoXe.Text = "".PadRight(9) + "Số Xe\n" + "".PadRight(5) + "Trong Kho:\n" + "".PadRight(11) + solgxe.ToString();
                txtblSoNV.FontSize = 20;
                txtblSoXe.FontSize = 20.5;
                Button btnLichSu = new Button();
                btnLichSu.Content = "Lịch Sử";
                btnLichSu.FontSize = 18;
                btnLichSu.FontWeight = FontWeights.Medium;
                btnLichSu.Height = 40;
                btnLichSu.Width = 110;
                btnLichSu.Foreground = Brushes.Black;
                btnLichSu.Background = Brushes.LightSkyBlue;
                btnLichSu.Click += btnLichSu_Click;
                stkbtnLichSu.Children.Add(btnLichSu);
            }
            else  //Nhân viên loại 2
            {
                lblXinChao.Content = "Xin Chào, " + PageChinh.getTen;
                lblChucVu.Content = "Nhân Viên Văn Phòng";

                //3 dòng dưới để lấy ngày vào làm của nhân viên, tính số ngày từ đó đến nay và hiển thị nó
                var dx = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                int d3 = (int)(dt - dx).Value.TotalDays;
                txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d3.ToString() + " Ngày";

                var slg = mdb.HoaDons.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.SoLuong).Sum();
                txtblSoXe.Text = "".PadRight(15) + slg.ToString() + "\n".PadRight(10) + "Là Số Xe\n" + "".PadRight(1) + "Bạn Bán Được";
                txtblSoNV.FontSize = 19;
                txtblSoXe.FontSize = 18.9;
            }
        }

        private async void btnCapNhatAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            if (OFD.ShowDialog()==true)
            {
                string newPathToFile = @"C:\Users\ADMIN\Documents\GitHub\Phan-mem-quan-ly-cua-hang-xe-may\src\MotoStore\Views\Pages\Images\" + PageChinh.getMa;
                if (File.Exists(newPathToFile)) //Nếu có 1 file ảnh khác tồn tại thì xoá nó đi và cập nhật file ảnh mới
                {

                    anhNhanVien.Source = null;

                    /* anhNhanVien.Source = null;
                     System.GC.Collect();
                     System.GC.WaitForPendingFinalizers();
                     File.Delete(newPathToFile);*/
                    //anhNhanVien.Source = null;

                    //Nó đang đc hiển thị

                }
                File.Copy(OFD.FileName, newPathToFile);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(newPathToFile);
                image.EndInit();
                anhNhanVien.Source = image;
                MessageBox.Show("Cập nhật ảnh thành công!");

                SqlConnection con = new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                SqlCommand cmd;
                con.Open();
                DateTime now = DateTime.Now;
                cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values('" + PageChinh.getMa + "', '" + now.ToString("dd-MM-yyyy HH:mm:ss") + "', N'cập nhật ảnh')", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void DisposeImage()
        {
            anhNhanVien.Source = null;
            GC.Collect();
        }

        private void brdSoNV_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(0, 198, 220, 222);
        }

        private void brdSoNV_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoNV.Margin = new Thickness(0, 248, 220, 222);
        }

        private void brdSoXe_MouseMove(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(0, 355, 220, 65);
        }

        private void brdSoXe_MouseLeave(object sender, MouseEventArgs e)
        {
            brdSoXe.Margin = new Thickness(0, 405, 220, 65);
        }

        private void brdLoiNhac_MouseMove(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(200, 272, 20, 138);
        }

        private void brdLoiNhac_MouseLeave(object sender, MouseEventArgs e)
        {
            brdLoiNhac.Margin = new Thickness(200, 322, 20, 138);
        }
    }
}