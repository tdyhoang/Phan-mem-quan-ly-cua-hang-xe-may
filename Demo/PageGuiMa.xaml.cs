using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using System.Threading;

namespace Demo
{
    /// <summary>
    /// Interaction logic for PageGuiMa.xaml
    /// </summary>
    public partial class PageGuiMa : Page
    {
        private PageChinh pgC;
        public PageGuiMa(PageChinh pageChinh)
        {
            InitializeComponent(); 
            pgC = pageChinh;
            /*Giống như PageQuenMatKhau, dùng hàm khởi tạo cõng theo PageChinh để có thể Back về PageChinh*/
        }

        private void buttonXacNhanGuiMa_Click(object sender, RoutedEventArgs e)
        { 
            if (string.IsNullOrEmpty(txtMa.Text))
            {
                switch (PageQuenMatKhau.ngonngu)
                {
                    case "Tiếng Việt":
                        lbl.Content = "Vui Lòng Nhập Mã!";
                        break;
                    case "English":
                        lbl.Content = "Please Fill The Code!";
                        break;
                }
            }
            else if (PageQuenMatKhau.ma==long.Parse(txtMa.Text))
            {
                this.NavigationService.Navigate(pgC);
                switch(PageQuenMatKhau.ngonngu)
                {
                    case "English":
                        pgC.lblThongBao.Content = "Change Password Successful";
                    break;
                    case "Tiếng Việt":
                        pgC.lblThongBao.Content = "Đổi Mật Khẩu Thành Công";
                        break;
                }

                /*Cập nhật mật khẩu mới lên DataBase
                ==>*/

        }
            else
            {
                switch(PageQuenMatKhau.ngonngu)
                {
                    case "Tiếng Việt":
                        lbl.Content = "Mật Mã Bạn Nhập Không Khớp, Hãy Kiểm Tra Lại";
                        break;
                    case "English":
                        lbl.Content = "Your Code You Fill Didn't Match, Check Again!";
                        break;
                }
                txtMa.Clear();
                txtMa.Focus();
            }
        }

    }
}
