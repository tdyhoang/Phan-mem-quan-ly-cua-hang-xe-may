using Demo.View;
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

namespace Demo
{
    /// <summary>
    /// Interaction logic for PageChinh.xaml
    /// </summary>
    public partial class PageChinh : Page
    {

        public PageChinh()
        {
            InitializeComponent();
            txtUser.Focus();  //Khi khởi động màn hình trang Chính thì đặt con trỏ chuột vào ô tài khoản
        }
    
        //static public PageChinh pgC;
        private int flag = 0;  //Đặt cờ để check xem nút Đăng Nhập có được Click vào hay chưa

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
        
        private void buttonDangNhap_Click(object sender, RoutedEventArgs e)
        {
            
            flag = 1; //Báo hiệu nút Đăng Nhập đã được click

            if (string.IsNullOrEmpty(txtUser.Text) == true || string.IsNullOrEmpty(txtPassword.Password))
            {
                if (buttonLanguage.Content == "English")
                {                   
                    lblThongBao.Content = "Please fill in all fields fully!";               
                }
                else
                {
                    lblThongBao.Content = "Vui lòng điền đầy đủ thông tin!";
                }
            }
            else
            {

                //Kiểm tra tài khoản mật khẩu có khớp với trên DataBase không ?

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
