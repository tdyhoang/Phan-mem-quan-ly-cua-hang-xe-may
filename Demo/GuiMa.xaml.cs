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

namespace Demo
{
    /// <summary>
    /// Interaction logic for GuiMa.xaml
    /// </summary>
    public partial class GuiMa : Window
    {
        public GuiMa()
        {
            InitializeComponent();
        }

        private void buttonXacNhanMa_Click(object sender, RoutedEventArgs e)
        {
            if (txtMa.Text == "")
            {
                switch (LoginView.ngonngu)
                {
                    case "Tiếng Việt":
                        MessageBox.Show("Vui Lòng Nhập Mã!");
                        break;
                    case "English":
                        MessageBox.Show("Please Fill The Code");
                        break;
                }
                txtMa.Focus();
            }
            else if(long.Parse(txtMa.Text)==QuenMatKhau.ma)
            {
                /*Không như C++ phải thông qua đối tượng để lấy thuộc tính tĩnh,
                C# cho phép lấy biến tĩnh thông qua Class*/
                switch(LoginView.ngonngu)
                {
                    case "Tiếng Việt":
                        MessageBox.Show("Cập Nhật Mật Khẩu Thành Công, Vui Lòng Đăng Nhập Lại!");
                        break;
                    case "English":
                        MessageBox.Show("Update Password Succesful, Please Login Again!");
                        break;
                }
                //Cập nhật lại mật khẩu mới lên DataBase


                this.Close();
            }
            else
            {
                switch (LoginView.ngonngu)
                {
                    case "Tiếng Việt":
                        MessageBox.Show("Mật Mã Bạn Nhập Không Khớp, Hãy Kiểm Tra Lại!");
                        txtMa.Focus();
                        break;
                    case "English":
                        MessageBox.Show("Your Code You Filled Not Match, Check Again!");
                        txtMa.Focus();
                        break;
                }
            }
        }
        private void GuiMa_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            /*Hàm này để fix lỗi:
             Cannot set Visibility or call Show, ShowDialog,
             or WindowInteropHelper.EnsureHandle after a Window has closed.*/
        }
    }
}
