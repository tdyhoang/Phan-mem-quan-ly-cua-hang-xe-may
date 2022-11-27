using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
/*Trong Quá Trình Chạy Login Form, Nếu Gặp Lỗi 
 Unable to copy file "obj\Debug\Demo.exe" to "bin\Debug\Demo.exe".
 The process cannot access the file 'bin\Debug\Demo.exe' because it is being used by another process.Demo:
1.Tắt VSCode
2.Bật TaskManager
3.Tìm Demo.exe và EndTask nó
4.Bật VSCode và chạy lại
 */

namespace Demo.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        QuenMatKhau qmk = new QuenMatKhau();
        static public GuiMa gm = new GuiMa();
        static public string ngonngu; //Đặt biến tĩnh để các form sau có thể truy cập
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
                    qmk.lblEmail.Content = "Your Email:";
                    qmk.lbldoiPass.Content = "New Password:";
                    qmk.lblXacNhandoiPass.FontSize = 18;
                    qmk.lblXacNhandoiPass.Content = "Retype New Password:";
                    qmk.buttonXacNhan.Content = "Confirm";
                    qmk.Title = "Reset Password";
                    gm.Title = "Verify Code";
                    gm.lblGuiMa.Content = "We had sent a code with 6 numbers to your Email you provided, fill that code below:";
                    gm.buttonXacNhanMa.Content = "Verify";
                    ngonngu = buttonLanguage.Content.ToString(); //Cập nhật biến ngôn ngữ
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
                    qmk.Title = "Thiết Lập Lại Mật Khẩu";
                    gm.Title = "Nhập Mã";
                    gm.lblGuiMa.Content = "Chúng tôi đã gửi mã 6 số về Email mà bạn cung cấp, hãy điền mã đó xuống dưới:";
                    gm.buttonXacNhanMa.Content = "Xác Nhận";
                    ngonngu = buttonLanguage.Content.ToString(); //Cập nhật biến ngôn ngữ
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
            qmk.Show();  
        }
    }
}
