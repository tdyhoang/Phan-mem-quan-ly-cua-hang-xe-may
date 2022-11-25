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
using System.Windows.Shapes;

namespace Demo.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
                    break;
                case "English":
                    buttonLanguage.Content = "Tiếng Việt";
                    txtTenTK.Text = "Tên Tài Khoản:";
                    txtMatKhau.Text = "Mật Khẩu:";
                    buttonDangNhap.Content = "Đăng Nhập";
                    buttonQuenMK.Content = "Quên Mật Khẩu ?";
                    txtQLYCHXM.Text = "QUẢN LÝ CỬA HÀNG XE MÁY";
                    lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    break;
            }

        }
        private void buttonDangNhap_Click(object sender, RoutedEventArgs e)
        {
            
            if(string.IsNullOrEmpty(txtUser.Text)==true||string.IsNullOrEmpty(txtPassword.Password))
            {     if (buttonLanguage.Content=="English")
                {
                    MessageBox.Show("Please fill in all fields completely and correctly!");
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                }
            }
            else
            {

            }
        }

        private void buttonQuenMK_Click(object sender, RoutedEventArgs e)
        {
            
        }

        
    }
}
