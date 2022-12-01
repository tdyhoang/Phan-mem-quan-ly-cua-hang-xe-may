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
        private PageChinh pgC; 
        public PageQuenMatKhau(PageChinh pageChinh)
        {
            InitializeComponent();
            pgC = pageChinh;
        }
        static public long ma;  //Đặt biến tĩnh để các Page sau có thể truy cập*/
        static public PageGuiMa pgGM = new PageGuiMa();
        static public string strEmail;
        static public string ngonngu = "Tiếng Việt";

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
                    pgGM.lblThongBao.Content = "We had sent a code with 6 numbers to your Email, please fill it below:";
                    pgGM.buttonXacNhanGuiMa.Content = "Verify";
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
                    pgGM.lblThongBao.Content = "Chúng tôi đã gửi mã 6 số về địa chỉ Email bạn cung cấp, xin hãy điền nó xuống dưới:";
                    pgGM.buttonXacNhanGuiMa.Content = "Xác Nhận";
                    break;
            }
            ngonngu = buttonLanguage.Content.ToString();
        } 

        private void buttonXacNhan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmail.Text) == true || string.IsNullOrEmpty(txtDoiPass.Password)==true||string.IsNullOrEmpty(txtXacNhanDoiPass.Password)==true)
            {
                if (buttonLanguage.Content == "English")
                {
                    lblThongBao.Content = "Please Fill All Fields Fully!";
                }
                else
                {
                    lblThongBao.Content = "Vui Lòng Điền Đầy Đủ Thông Tin!";
                }
            }
            else if(txtDoiPass.Password!=txtXacNhanDoiPass.Password)
            {
                if (buttonLanguage.Content == "English")
                {
                    lblThongBao.Content = "Password Retype Didn't Match New Password, Check Again!";
                }
                else
                {
                    lblThongBao.Content = "Mật Khẩu Xác Nhận Không Khớp Với Mật Khẩu Mới, Hãy Kiểm Tra Lại!";
                }
                txtXacNhanDoiPass.Clear();
                txtXacNhanDoiPass.Focus();
            }
            else
            {
                GuiMail();
                var Mainframe = Application.Current.MainWindow;
                Mainframe.Content = pgGM;
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
            /* var q = NavigationCommands.BrowseBack;
             NavigationService.Navigate(q); */
            /*if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("No entries in back navigation history.");
            }*/
            //var Mainframe = Application.Current.MainWindow;
            //this.NavigationService.Navigate(new Uri("PageChinh.xaml", UriKind.Relative));



            //LoginView.nav = this.NavigationService;

            //this.NavigationService.Navigate(pgC);
            NavigationService.Navigate(pgC);

           /* LoginView.nav = this.NavigationService;
            LoginView.nav.Navigate(new PageChinh()); */

            //NavigationService.Navigate(new PageChinh());

            /*NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("PageChinh.xaml", UriKind.RelativeOrAbsolute));*/



            //LoginView.Navigator.GoBack();
            //NavigationService.Navigate(Uri);

            /*var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ChangeView(new Page1());*/
            /*MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.Visibility = Visibility.Visible;
            Window win = (Window)this.Parent;
            win.Close();*/
        }
    }
}
