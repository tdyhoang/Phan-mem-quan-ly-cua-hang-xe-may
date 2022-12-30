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
using Microsoft.Data.SqlClient;
using System.Globalization;
namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOKhachHangPage.xaml
    /// </summary>
    public partial class IOKhachHangPage : Page
    {
        public IOKhachHangPage()
        {
            InitializeComponent();
        }

        private void btnAddNewKhachHang_Click(object sender, RoutedEventArgs e)
        {
            bool check = true;
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if ((string.IsNullOrWhiteSpace(txtTenKH.Text)) || (string.IsNullOrWhiteSpace(txtEmailKH.Text)) || (string.IsNullOrWhiteSpace(txtNgaySinhKH.Text)) || (string.IsNullOrWhiteSpace(txtDiaChiKH.Text)) || (string.IsNullOrWhiteSpace(txtSDTKH.Text)) || (string.IsNullOrWhiteSpace(cmbGioiTinhKH.Text)) || (string.IsNullOrWhiteSpace(cmbLoaiKH.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
            }
            else
            {
                for (int i = 0; i < txtTenKH.Text.Length; i++)//Check Tên Khách Hàng
                {
                    if ((txtTenKH.Text[i] >= 48 && txtTenKH.Text[i] <= 57))
                    {
                        MessageBox.Show("Tên Khách Hàng không được chứa các ký tự số! ");
                        check = false;
                    }
                }
                DateTime date;
                if (!(DateTime.TryParseExact(txtNgaySinhKH.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)))
                {
                    MessageBox.Show("Ngày Sinh không hợp lệ !");
                    check=false;
                }
                
                for (int i = 0; i < txtSDTKH.Text.Length; i++) //Check SĐT KHách Hàng
                {
                    if (!(txtSDTKH.Text[i] >= 48 && txtSDTKH.Text[i] <= 57))
                    {
                        MessageBox.Show("SĐT không được chứa các ký tự ");
                        check = false;
                    }
                }
                if (!(txtEmailKH.Text.Contains("@gmail.com"))) //Check Email Khách hàng
                {
                    MessageBox.Show("Đuôi email không hợp lệ !");
                    check = false;
                }
                
                if (check)
                {
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into KhachHang values( NEWID(),  "+"  N'" + txtTenKH.Text + "','" + txtNgaySinhKH.Text + "','" + cmbGioiTinhKH.Text + "','" + txtDiaChiKH.Text + "','" + txtSDTKH.Text + "','" + txtEmailKH.Text + "','" + cmbLoaiKH.Text + " ' )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                }
            }
        }
    }
}
