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
using MotoStore.Database;
using Microsoft.Data.SqlClient;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IONhaSXPage.xaml
    /// </summary>
    public partial class IONhaSXPage : Page
    {
       
        public IONhaSXPage()
        {
            InitializeComponent();
        }
       
        private void btnAddNewNSX_Click(object sender, RoutedEventArgs e)
        {
            bool check=true;
            SqlConnection con = new SqlConnection(@"Data Source=.\SQLExpress;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
            SqlCommand cmd;
            if (string.IsNullOrWhiteSpace(txtTenNSX.Text) || (string.IsNullOrWhiteSpace(txtSDTNSX.Text)) || (string.IsNullOrWhiteSpace(txtEmailNSX.Text)) || (string.IsNullOrWhiteSpace(txtNuocSX.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
            }
            else
            {
                for (int i = 0; i < txtSDTNSX.Text.Length; i++)
                {
                    if (!(txtSDTNSX.Text[i] >= 48 && txtSDTNSX.Text[i] <= 57))
                    {
                        MessageBox.Show("SĐT không được chứa các ký tự ");
                        check = false;
                    }
                }
                if (!(txtEmailNSX.Text.Contains("@gmail.com")))
                {
                    MessageBox.Show("Đuôi email không hợp lệ !");
                    check = false;
                }
                for (int i = 0; i < txtNuocSX.Text.Length; i++)
                {
                    if ((txtNuocSX.Text[i] >= 48 && txtNuocSX.Text[i] <= 57))
                    {
                        MessageBox.Show("Tên Nước Sản Xuất không được chứa các ký tự số! ");
                        check = false;
                    }

                }
                if(check)
                {
                    con.Open();
                    cmd = new SqlCommand("Set Dateformat dmy\nInsert into NhaSanXuat values('" + txtTenNSX.Text + "','" + txtSDTNSX.Text + "','" + txtEmailNSX.Text + "',N'" + txtNuocSX.Text + " ')", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                }    
            }
           

        }
    }
}
