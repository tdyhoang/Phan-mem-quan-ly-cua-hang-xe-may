using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using MotoStore.Database;
using Microsoft.Data.SqlClient;

namespace MotoStore.Views.Pages.LoginPages
{
    /// <summary>
    /// Interaction logic for PageChinh.xaml
    /// </summary>
    public partial class PageChinh
    {
        
        public PageChinh()
        {
            InitializeComponent();
            txtUser.Focus();  //Khi khởi động màn hình trang Chính thì đặt con trỏ chuột vào ô tài khoản
            timer.Tick += Timer_Tick;
        }

        static public bool isValid = false;
        static public NhanVien? getNV; //Đặt biến tĩnh public để PageDashboard có thể truy cập để lấy các thông tin cần thiết
        private readonly DispatcherTimer timer = new();
        static private int dem = 0;   //Biến đếm số lần nháy
        private bool Nhay = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dem == 7)           //dem = 7 Thì Ngừng Nháy
                timer.Stop();
            if (Nhay)
            {
                lblThongBao.Foreground = Brushes.Red;
                dem++;
            }
            else
            {
                lblThongBao.Foreground = Brushes.Black;
                dem++;
            }
            Nhay = !Nhay;
            //Hàm Này Để Nháy Thông Báo 
        }

        public void buttonDangNhap_Click(object sender, RoutedEventArgs e)
        {
            dem = 0;  //Mỗi lần bấm nút Đăng Nhập thì dem được set lại = 0
            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                lblThongBao.Content = "Vui lòng điền đầy đủ thông tin!";
                timer.Interval = new(0, 0, 0, 0, 200);
                timer.Start();         //Bắt Đầu Nháy       
            }
            else
            {
                //Kiểm tra tài khoản mật khẩu có khớp với trên DataBase không ?

                MainDatabase mDb = new();  
                foreach (var user in mDb.UserApps.ToList())
                    if (string.Equals(user.UserName, txtUser.Text, StringComparison.OrdinalIgnoreCase) && string.Equals(user.Password, txtPassword.Password))
                    {
                        isValid = true;             //Mở cổng đăng nhập
                        foreach (var nv in mDb.NhanViens.Where(nv => !nv.DaXoa && nv.MaNv == user.MaNv))
                            getNV = nv;

                        // Trường hợp nhân viên đã bị xóa khỏi database
                        if (getNV is null)
                        {
                            isValid = false;
                            lblThongBao.Content = "Sai tài khoản hoặc mật khẩu!";
                            timer.Interval = new(0, 0, 0, 0, 200);
                            timer.Start();         //Bắt Đầu Nháy
                            return;
                        }

                        SqlCommand cmd;
                        using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
                        try
                        {
                            con.Open();
                            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'đăng nhập')", con);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                if (isValid)  
                {
                    var getWd = Window.GetWindow(this);  //Lấy Window của cái Trang này(PageChinh)
                    getWd.Close();                       //Đăng Nhập thành công => đóng Form Đăng Nhập
                    Application.Current.MainWindow.Visibility = Visibility.Visible;
                    isValid = false;                    //Set lại giá trị để đóng cổng đăng nhập
                }
                else
                {
                    lblThongBao.Content = "Sai Tài Khoản Hoặc Mật Khẩu";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    timer.Start();
                    //Nháy nếu sai TK hoặc MK
                }
            }
        }

        private void buttonQuenMK_Click(object sender, RoutedEventArgs e)
        {
            var pageQuenMatKhau = new PageQuenMatKhau(this);
            //Khởi tạo biến pageQuenMatKhau là trang QuenMatKhau cõng theo this(trang Chính này)
            //Cõng theo this để có thể Back về this
            NavigationService.Navigate(pageQuenMatKhau); //Chuyển hướng sang trang QuenMatKhau
            //Hàm này dùng để chuyển Trang từ Trang Chính sang Trang Quên Mật Khẩu
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                buttonDangNhap_Click(sender, e);
        }
    }
}
