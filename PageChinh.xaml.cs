using Demo.View;
using System;
using System.Collections.Generic;
using System.Linq;
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

        }
        static public PageChinh pgC;
        static public PageQuenMatKhau qmk = new PageQuenMatKhau(pgC);

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void buttonLanguage_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonLanguage.Content)
            {
                case "Tiếng Việt":
                    buttonLanguage.Content = "English";
                    txtTenTK.Text = "Username:";
                    txtMatKhau.Text = "Password:";
                    buttonDangNhap.Content = "Login";
                    buttonQuenMK.Content = "Forgot Password ?";
                    txtQLYCHXM.FontSize = 23;
                    txtQLYCHXM.Text = "MOTORCYCLE SHOP MANAGER";
                    lblNnguHienTai.Content = "Current Language:";
                    qmk.lblEmail.Content = "Your Email:";
                    qmk.lbldoiPass.Content = "New Password:";
                    qmk.lblXacNhandoiPass.FontSize = 18;
                    qmk.lblXacNhandoiPass.Content = "Retype New Password:";
                    qmk.buttonXacNhan.Content = "Confirm";
                    qmk.lblNnguHienTai.Content = "Current Language:";
                    qmk.buttonLanguage.Content = "English";
                    qmk.buttonXacNhan.Content = "Confirm";
                    qmk.buttonQuayLai.Content = "Back";
                    break;
                case "English":
                    buttonLanguage.Content = "Tiếng Việt";
                    txtTenTK.Text = "Tên Tài Khoản:";
                    txtMatKhau.Text = "Mật Khẩu:";
                    buttonDangNhap.Content = "Đăng Nhập";
                    buttonQuenMK.Content = "Quên Mật Khẩu ?";
                    txtQLYCHXM.Text = "QUẢN LÝ CỬA HÀNG XE MÁY";
                    lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    qmk.lblEmail.Content = "Nhập Địa Chỉ Email:";
                    qmk.lbldoiPass.Content = "Nhập Mật Khẩu Mới:";
                    qmk.lblXacNhandoiPass.Content = "Xác Nhận Mật Khẩu:";
                    qmk.buttonXacNhan.Content = "Xác Nhận";
                    qmk.lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    qmk.buttonLanguage.Content = "Tiếng Việt";
                    qmk.buttonXacNhan.Content = "Xác Nhận";
                    qmk.buttonQuayLai.Content = "Quay Lại";
                    break;
            }
        }

        private void buttonDangNhap_Click(object sender, RoutedEventArgs e)
        {
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

            }
        }

        private void buttonQuenMK_Click(object sender, RoutedEventArgs e)
        {
            // var Mainframe = Application.Current.MainWindow;   
            // Mainframe.Content = qmk;
            var pageQuenMatKhau = new PageQuenMatKhau(this);
            NavigationService.Navigate(pageQuenMatKhau);
            //Hàm này dùng để chuyển Trang từ Trang Chính sang Trang Quên Mật Khẩu
        }

    }
}
