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
using System.Net.Mail;
using System.Net;
using System.Security.RightsManagement;

namespace Demo
{
    /// <summary>
    /// Interaction logic for PageQuenMatKhau.xaml
    /// </summary>
    public partial class PageQuenMatKhau : Page
    {
        static private PageChinh pgC; //Tạo biến kiểu PageChinh để có thể sử dụng trong class này
        public PageQuenMatKhau(PageChinh pageChinh)
        {
            InitializeComponent();
            pgC = pageChinh; //Gán biến pgC chính là tham số pageChinh được cõng theo
            //Hàm Khởi Tạo của trang QuenMatKhau có cõng theo tham số là pageChinh thuộc kiểu PageChinh
        }
        static public long ma;  //Đặt biến tĩnh để các PageGuiMa có thể truy cập*/
        public PageGuiMa pgGM = new PageGuiMa(pgC);
        static public string strEmail;
        static public string ngonngu = "Tiếng Việt";
        private int flag = 0;

        private void buttonLanguageQMK_Click(object sender, RoutedEventArgs e)
        {
            switch (buttonLanguage.Content)
            {
                case "Tiếng Việt":
                    lblNnguHienTai.Content = "Current Language:";
                    buttonLanguage.Content = "English";
                    lblEmail.Content = "Your Email:";
                    lbldoiPass.Content = "New Password:";
                    lblXacNhandoiPass.FontSize = 18;
                    lblXacNhandoiPass.Content = "Retype New Password:";
                    buttonXacNhan.Content = "Confirm";
                    buttonQuayLai.Content = "Back";
                    if (flag == 1)
                        lblThongBao.Content = "Please fill all of your fields fully!";
                    pgGM.lblThongBao.Content = "We had sent a code with 6 numbers to your Email, please fill it below:";
                    pgGM.buttonXacNhanGuiMa.Content = "Verify";
                    pgC.lblNnguHienTai.Content = "Current Language:";
                    pgC.buttonLanguage.Content = "English";
                    pgC.txtTenTK.Text = "Username:";
                    pgC.txtMatKhau.Text = "Password:";
                    pgC.buttonDangNhap.Content = "Login";
                    pgC.buttonQuenMK.FontSize = 14;
                    pgC.buttonQuenMK.Content = "Forgot Password ?";
                    pgC.txtQLYCHXM.FontSize = 22;
                    pgC.txtQLYCHXM.Text = "MOTORCYCLE SHOP MANAGER";
                    pgC.txtSlogan.Text = "We bring the best solution for manager";
                    break;
                case "English":
                    lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    buttonLanguage.Content = "Tiếng Việt";
                    lblEmail.Content = "Nhập Địa Chỉ Email:";
                    lbldoiPass.Content = "Nhập Mật Khẩu Mới:";
                    lblXacNhandoiPass.FontSize = 18;
                    lblXacNhandoiPass.Content = "Xác Nhận Mật Khẩu:";
                    buttonXacNhan.Content = "Xác Nhận";
                    buttonQuayLai.Content = "Quay Lại";
                    if (flag == 1)
                        lblThongBao.Content = "Vui lòng điền đầy đủ thông tin của bạn!";
                    pgGM.lblThongBao.Content = "Chúng tôi đã gửi mã 6 số về địa chỉ Email bạn cung cấp, xin hãy điền nó xuống dưới:";
                    pgGM.buttonXacNhanGuiMa.Content = "Xác Nhận";
                    pgC.buttonLanguage.Content = "Tiếng Việt";
                    pgC.txtTenTK.Text = "Tên Tài Khoản:";
                    pgC.txtMatKhau.Text = "Mật Khẩu:";
                    pgC.buttonDangNhap.Content = "Đăng Nhập";
                    pgC.buttonQuenMK.Content = "Quên Mật Khẩu ?";
                    pgC.txtQLYCHXM.Text = "QUẢN LÝ CỬA HÀNG XE MÁY";
                    pgC.txtSlogan.Text = " Chúng tôi mang đến giải pháp tốt nhất cho nhà quản lý";
                    pgC.lblNnguHienTai.Content = "Ngôn Ngữ Hiện Tại:";
                    break;
            }
            ngonngu = buttonLanguage.Content.ToString();
        } 

        private void buttonXacNhan_Click(object sender, RoutedEventArgs e)
        {
            flag = 1;  //Báo hiệu nút Xác Nhận đã được click
            if (string.IsNullOrEmpty(txtEmail.Text) == true || string.IsNullOrEmpty(txtDoiPass.Password)==true||string.IsNullOrEmpty(txtXacNhanDoiPass.Password)==true)
            {
                if (buttonLanguage.Content == "English")
                    lblThongBao.Content = "Please Fill All Fields Fully!";
                else
                    lblThongBao.Content = "Vui Lòng Điền Đầy Đủ Thông Tin!";
            }
            else if(txtDoiPass.Password!=txtXacNhanDoiPass.Password)
            {
                if (buttonLanguage.Content == "English")
                    lblThongBao.Content = "Password Retype Didn't Match New Password, Check Again!";
                else
                    lblThongBao.Content = "Mật Khẩu Xác Nhận Không Khớp Với Mật Khẩu Mới, Kiểm Tra Lại!";
                txtXacNhanDoiPass.Clear();
                txtXacNhanDoiPass.Focus();
            }
            else
            {
                GuiMail();
                var pageGuiMa = new PageGuiMa(pgC);
                switch(buttonLanguage.Content)
                {
                    case "English":
                        pageGuiMa.lblThongBao.Content = "We had sent a code with 6 numbers to your Email, fill it below:";
                        pageGuiMa.buttonXacNhanGuiMa.Content = "Verify";
                        break;
                    case "Tiếng Việt":
                        pageGuiMa.lblThongBao.Content = "Chúng tôi đã gửi mã 6 số về Email của bạn, điền nó xuống dưới:";
                        pageGuiMa.buttonXacNhanGuiMa.Content = "Xác Nhận";
                        break;
                }
                NavigationService.Navigate(pageGuiMa);
                //Nếu thoả mãn hết các điều kiện kiểm tra thì chuyển hướng tiếp sang Trang GuiMa
            }
        }

        private void GuiMail()
        {
            MailMessage mess = new MailMessage();
            mess.From = new MailAddress("datrua3152003@gmail.com"); //Đây là Email gửi từ ứng dụng
            Random rand = new Random();
            ma = rand.Next(100000, 999999);
            switch (buttonLanguage.Content)
            {
                case "Tiếng Việt":
                    mess.Subject = "Mã Xác Nhận Thay Đổi Mật Khẩu";
                    mess.Body = "Mã Xác Nhận Của Bạn Là: " + ma.ToString();
                    break;
                case "English":
                    mess.Subject = "Verify Change Password Code";
                    mess.Body = "Your Verify Code Is: " + ma.ToString();
                    break;
            }
            mess.To.Add(new MailAddress(txtEmail.Text));   //Email nhận là của người Nhân Viên Quản Lý
            SmtpClient smtpClient1 = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("datrua3152003@gmail.com", "ewxeolqofypyfzgi"),
                EnableSsl = true,
            };
            smtpClient1.Send(mess);

            /* Hàm này để ứng dụng gửi mã 6 số ngẫu nhiên
              về Email người Quản Lý để xác nhận thay đổi mật khẩu */
        }

        private void buttonQuayLai_Click(object sender, RoutedEventArgs e)
        { 
            this.NavigationService.Navigate(pgC);

            //Hàm này để Quay lại Trang Chính(pgC)
        }
    }
}
