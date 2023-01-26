using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Net.Mail;
using System.Net;
using System.Windows.Threading;
using MotoStore.Database;
using System.Linq;

namespace MotoStore.Views.Pages.LoginPages
{
    /// <summary>
    /// Interaction logic for PageQuenMatKhau.xaml
    /// </summary>
    public partial class PageQuenMatKhau
    {
        static private PageChinh pgC; //Tạo biến kiểu PageChinh để có thể sử dụng trong class này
        static public UserApp? getUser;
        public PageQuenMatKhau(PageChinh pageChinh)
        {
            InitializeComponent();
            pgC = pageChinh; //Gán biến pgC chính là tham số pageChinh được cõng theo
            txtUsername.Focus();
            timer.Tick += Timer_Tick;
            //Hàm Khởi Tạo của trang QuenMatKhau có cõng theo tham số là pageChinh thuộc kiểu PageChinh
        }

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

        static public long ma;  //Đặt biến tĩnh để các PageGuiMa có thể truy cập*/
        public PageGuiMa pgGM = new(pgC);
        static public string? strEmail;
        private readonly DispatcherTimer timer = new();

        private void buttonXacNhan_Click(object sender, RoutedEventArgs e)
        {
            dem = 0;
            var pageGuiMa = new PageGuiMa(pgC);
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtDoiPass.Password) || string.IsNullOrEmpty(txtXacNhanDoiPass.Password))
            {
                lblThongBao.Content = "Vui Lòng Điền Đầy Đủ Thông Tin!";
                timer.Interval = new(0, 0, 0, 0, 200);
                timer.Start();
            }
            else if(txtDoiPass.Password != txtXacNhanDoiPass.Password)
            {
                lblThongBao.Content = "Mật Khẩu Xác Nhận Không Khớp Với Mật Khẩu Mới, Kiểm Tra Lại!";
                timer.Interval = new(0, 0, 0, 0, 200);
                timer.Start();
                txtXacNhanDoiPass.Clear();
                txtXacNhanDoiPass.Focus();
            }
            else
            {
                MainDatabase mdb = new();
                if (!mdb.UserApps.Any(user => string.Compare(txtUsername.Text, user.UserName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    lblThongBao.Content = "Tên tài khoản không tồn tại, hãy xem lại!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    timer.Start();
                }
                else
                {
                    bool chuyentrang = false;
                    foreach(var user in mdb.UserApps)
                    {
                        if (string.Equals(txtUsername.Text, user.UserName, StringComparison.OrdinalIgnoreCase))
                        {
                            getUser = user;
                            GuiMail();
                            chuyentrang = true;
                            pageGuiMa.lblThongBao.Content = "Chúng tôi đã gửi mã 6 số về Email của bạn, điền nó xuống dưới:";
                            pageGuiMa.buttonXacNhanGuiMa.Content = "Xác Nhận";
                            break;
                        }
                    }
                    if (chuyentrang)
                        NavigationService.Navigate(pageGuiMa);
                }
                //Nếu thoả mãn hết các điều kiện kiểm tra thì chuyển hướng tiếp sang Trang GuiMa
            }
        }

        private static void GuiMail()
        {
            try
            {
                Random rand = new();
                ma = rand.Next(1000000);
                MailMessage mess = new()
                {
                    From = new("datrua3152003@gmail.com"), //Đây là Email gửi từ ứng dụng
                    Subject = "Mã Xác Nhận Thay Đổi Mật Khẩu",
                    Body = $"Mã Xác Nhận Của Bạn Là: {ma:000000}"
                };
                mess.To.Add(new(getUser.Email));   //Email nhận là của người Nhân Viên Quản Lý
                SmtpClient smtpClient1 = new("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("datrua3152003@gmail.com", "ewxeolqofypyfzgi"),
                    EnableSsl = true,
                };
                smtpClient1.Send(mess);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /* Hàm này để ứng dụng gửi mã 6 số ngẫu nhiên
              về Email người Quản Lý để xác nhận thay đổi mật khẩu */
        }

        private void buttonQuayLai_Click(object sender, RoutedEventArgs e)
            => NavigationService.Navigate(pgC);
            //Hàm này để Quay lại Trang Chính(pgC)

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                buttonXacNhan_Click(sender, e);
        }
    }
}
