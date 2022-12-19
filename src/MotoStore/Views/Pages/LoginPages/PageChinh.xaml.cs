using MotoStore.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Data.SqlClient;
using MotoStore.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography;
using MotoStore.ViewModels;
using Wpf.Ui.Mvvm.Contracts;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Net;

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
    
        //static public PageChinh pgC;
        private int flag = 0;  //Đặt cờ để check xem nút Đăng Nhập có được Click vào hay chưa
        static public bool isValid = false;
        static public int getLoaiNV;   //Đặt biến tĩnh public để PageDashboard có thể truy cập để lấy các thông tin cần thiết
        static public string getMa;   //Tương tự như trên
        static public string getSex;  //Lấy giới tính
        private readonly DispatcherTimer timer = new DispatcherTimer();

        private void buttonLanguage_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonLanguage.Content)
            {
                case "Tiếng Việt":
                    buttonLanguage.Content = "English";
                    txtTenTK.Text = "Username:";
                    txtMatKhau.Text = "Password:";
                    buttonDangNhap.Content = "Login";
                    buttonQuenMK.FontSize = 14;
                    buttonQuenMK.Content = "Forgot Password ?";
                    txtQLYCHXM.FontSize = 22;
                    txtQLYCHXM.Text = "MOTORCYCLE SHOP MANAGER";
                    txtSlogan.Text = "We bring the best solution for manager";
                    lblNnguHienTai.Content = "Current Language:";
                    if(flag!=0)
                        lblThongBao.Content = "Please fill in all fields fully!";
                    break;
                case "English":
                    buttonLanguage.Content = "Tiếng Việt";
                    txtTenTK.Text = "Tên Tài Khoản:";
                    txtMatKhau.Text = "Mật Khẩu:";
                    buttonDangNhap.Content = "Đăng Nhập";
                    buttonQuenMK.Content = "Quên Mật Khẩu ?";
                    txtQLYCHXM.Text = "QUẢN LÝ CỬA HÀNG XE MÁY";
                    txtSlogan.Text = " Chúng tôi mang đến giải pháp tốt nhất cho nhà quản lý";
                    lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    if (flag != 0)
                        lblThongBao.Content = "Vui lòng điền đầy đủ thông tin!";
                    break;
            }
        }
        static private int dem = 0;
        private bool Nhay = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dem == 5)           //dem Vượt Quá 5 Thì Ngừng Nháy
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
            dem = 0;

            flag = 1; //Báo hiệu nút Đăng Nhập đã được click

            if (string.IsNullOrEmpty(txtUser.Text) == true || string.IsNullOrEmpty(txtPassword.Password))
            {
                if (buttonLanguage.Content == "English")
                {
                    lblThongBao.Content = "Please fill in all fields fully!";
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 200);      //Nháy Mỗi 200 milisecond
                }
                else
                {   
                    lblThongBao.Content = "Vui lòng điền đầy đủ thông tin!";
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                }
                timer.Start();         //Bắt Đầu Nháy       
            }
            else
            {
                //Kiểm tra tài khoản mật khẩu có khớp với trên DataBase không ?

                MainDatabase mDb = new MainDatabase();  
                foreach (var user in mDb.UserApps.ToList())
                    if (user.UserName == txtUser.Text && user.Password == txtPassword.Password)
                    {
                        isValid = true;               //Mở cổng đăng nhập
                        getLoaiNV = (int)mDb.NhanViens.Where(u => u.MaNv.ToString() == user.MaNv.ToString()).Select(u => u.LoaiNv).FirstOrDefault();
                        getSex=(string)mDb.NhanViens.Where(u => u.MaNv.ToString() == user.MaNv.ToString()).Select(u => u.GioiTinh).FirstOrDefault();
                        getMa = user.MaNv.ToString();   
                        getMa = getMa.ToUpper();  //Set lại giá trị Upper vì nếu để getMa không thôi thì nó sẽ không khớp với dữ liệu trên mDb
                    }

                if (isValid)  
                {
                    var getWd = Window.GetWindow(this);  //Lấy Window của cái Trang này
                    getWd.Close();                       //Đóng nó sau khi đăng nhập thành công
                    App.Current.MainWindow.Visibility = Visibility.Visible;
                    isValid = false;                    //Set lại giá trị để đóng cổng đăng nhập
                }
                else
                {
                    lblThongBao.Content = "Sai Tài Khoản Hoặc Mật Khẩu";
                    timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                    timer.Start();
                }
            }
        }

        private void buttonQuenMK_Click(object sender, RoutedEventArgs e)
        {
            var pageQuenMatKhau = new PageQuenMatKhau(this); //Khởi tạo biến pageQuenMatKhau là trang QuenMatKhau cõng theo this(trang Chính này)
            //Cõng theo this để có thể Back về this

            switch (buttonLanguage.Content)
            {
                case "English":
                    pageQuenMatKhau.lblEmail.Content = "Your Email:";
                    pageQuenMatKhau.lbldoiPass.Content = "New Password:";
                    pageQuenMatKhau.lblXacNhandoiPass.FontSize = 18;
                    pageQuenMatKhau.lblXacNhandoiPass.Content = "Retype New Password:";
                    pageQuenMatKhau.buttonXacNhan.Content = "Confirm";
                    pageQuenMatKhau.lblNnguHienTai.Content = "Current Language:";
                    pageQuenMatKhau.buttonLanguage.Content = "English";
                    pageQuenMatKhau.buttonXacNhan.Content = "Confirm";
                    pageQuenMatKhau.buttonQuayLai.Content = "Back";
                    break;
                case "Tiếng Việt":
                    pageQuenMatKhau.lblEmail.Content = "Nhập Địa Chỉ Email:";
                    pageQuenMatKhau.lbldoiPass.Content = "Nhập Mật Khẩu Mới:";
                    pageQuenMatKhau.lblXacNhandoiPass.FontSize = 18;
                    pageQuenMatKhau.lblXacNhandoiPass.Content = "Xác Nhận Mật Khẩu:";
                    pageQuenMatKhau.buttonXacNhan.Content = "Xác Nhận";
                    pageQuenMatKhau.lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    pageQuenMatKhau.buttonLanguage.Content = "Tiếng Việt";
                    pageQuenMatKhau.buttonXacNhan.Content = "Xác Nhận";
                    pageQuenMatKhau.buttonQuayLai.Content = "Quay Lại";
                    break;
            }
            //Lệnh switch trên để thay đổi tên các control của trang QuenMatKhau sao cho tương ứng với ngôn ngữ hiện tại

            NavigationService.Navigate(pageQuenMatKhau); //Chuyển hướng sang trang QuenMatKhau
            pageQuenMatKhau.txtEmail.Focus(); //Đặt con trỏ chuột vào ô Email

            //Hàm này dùng để chuyển Trang từ Trang Chính sang Trang Quên Mật Khẩu
        }

    }
}
