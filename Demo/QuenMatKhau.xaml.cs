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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;
namespace Demo
{
    /// <summary>
    /// Interaction logic for QuenMatKhau.xaml
    /// </summary>
    public partial class QuenMatKhau : Window
    {
        static public long ma; //Đặt biến tĩnh để form GuiMa lấy mã
        public QuenMatKhau()
        {
            InitializeComponent();      
        }
        private void buttonXacNhan_Click(object sender, RoutedEventArgs e)
        { 
            if (string.IsNullOrEmpty(txtEmail.Text) == true||string.IsNullOrEmpty(txtDoiPass.ToString())==true||string.IsNullOrEmpty(txtXacNhanDoiPass.ToString())==true)
            {
                switch (LoginView.ngonngu)
                {
                    case "Tiếng Việt":
                        MessageBox.Show("Vui Lòng Điền Đầy Đủ Thông Tin!");
                        break;
                    case "English":
                        MessageBox.Show("Please Fill Your Information Fully!");
                        break;
                }
            }
            else if(txtDoiPass.Password!=txtXacNhanDoiPass.Password)
            {
                switch (LoginView.ngonngu)
                {
                    case "Tiếng Việt":
                        MessageBox.Show("Mật Khẩu Xác Nhận Không Khớp Với Mật Khẩu Thay Đổi, Vui Lòng Kiểm Tra Lại!");
                        break;
                    case "English":
                        MessageBox.Show("Your Verify Password Didn't Match New Password, Please Check Again!");
                        break;
                }
                txtXacNhanDoiPass.Clear();
                txtXacNhanDoiPass.Focus();
            }
            else
            {
                LoginView.gm.Show();
                GuiMail();
                this.Close();
            }
        }
        private void QuenMatKhau_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            /*Hàm này để fix lỗi:
             Cannot set Visibility or call Show, ShowDialog,
             or WindowInteropHelper.EnsureHandle after a Window has closed.
            Xảy ra khi người dùng Bấm ButtonQuenMatKhau nhiều lần*/
        }
        private void GuiMail()
        {
            MailMessage mess = new MailMessage();
            mess.From = new MailAddress("datrua3152003@gmail.com"); //Đây là Email gửi từ ứng dụng
            Random rand = new Random();
            ma = rand.Next(100000, 999999);
            switch (LoginView.ngonngu)
            {
                case "Tiếng Việt":
                    mess.Subject = "Mã Xác Nhận Thay Đổi Mật Khẩu";
                    mess.Body = "Mã xác nhận của bạn là: " + ma.ToString();
                    break;
                case "English":
                    mess.Subject = "Verify Change Password Code";
                    mess.Body = "Your Verify Code is: " + ma.ToString();
                    break;
            }
            mess.To.Add(new MailAddress(txtEmail.ToString()));   //Email nhận là của người Nhân Viên Quản Lý
            SmtpClient smtpClient1 = new SmtpClient("smtp.gmail.com")  
            {
                Port = 587,
                Credentials = new NetworkCredential("datrua3152003@gmail.com", "ewxeolqofypyfzgi"),
                EnableSsl = true,
            };
            smtpClient1.Send(mess);
            /*Hàm này để ứng dụng gửi mã 6 số ngẫu nhiên 
             về Email người Quản Lý để xác nhận thay đổi mật khẩu*/
        }
    }
}
