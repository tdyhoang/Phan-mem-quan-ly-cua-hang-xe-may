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
using System.Windows.Threading;
using System.Windows.Media.Animation;

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
            timer.Tick += Timer_Tick;
        }
        private int flag = 0;  //Đặt cờ để check xem nút Đăng Nhập có được Click vào hay chưa
        static public bool isValid = false;
        private readonly DispatcherTimer timer = new();
        private readonly DateTime dt = DateTime.Now;
        bool checkTenKH = false;
        bool checkNgaySinh= false;
        bool checkSDT = true;
        bool checkDiaChi= true;
        bool checkEmail = false;
       
      

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


        private void btnAddNewKhachHang_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if (!(checkTenKH && checkNgaySinh && checkEmail && checkSDT))
            {
                MessageBox.Show("Vui lòng nhập đúng thông tin! ");
            }     
            else
            {                      
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into KhachHang values(N'" + txtTenKH.Text + "','" + txtNgaySinhKH.Text + "',N'"  + cmbGioiTinhKH.Text + "', N'" + txtDiaChiKH.Text + "','" + txtSDTKH.Text + "','" + txtEmailKH.Text + "',N'" + cmbLoaiKH.Text + " ',0 )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                
            }
        }

        private void txtTenKH_LostFocus(object sender, RoutedEventArgs e) // Check Tên Khách Hang
        {
             checkTenKH = true;
            for (int i = 0; i < txtTenKH.Text.Length; i++)
            {
                if ((txtTenKH.Text[i] >= 48 && txtTenKH.Text[i] <= 57))
                {

                    lblThongBao.Content = "Tên Khách Hàng không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;
                    timer.Start();
                    checkTenKH = false;
                    break;
                }

            }
            if (checkTenKH)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }

        }

        private void txtNgaySinhKH_LostFocus(object sender, RoutedEventArgs e) //Check Ngày Sinh Khách Hàng
        {
             checkNgaySinh = true;
            DateTime date;
            if (!(DateTime.TryParseExact(txtNgaySinhKH.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)))
            {
                lblThongBaoNS.Content = "Ngày Sinh không hợp lệ!";
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBaoNS.Visibility = Visibility.Visible;
                timer.Start();
                checkNgaySinh = false;              
            }
            if (checkNgaySinh)
            {
                lblThongBaoNS.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSDTKH_LostFocus(object sender, RoutedEventArgs e) //Check SĐT khách hàng
        {
             checkSDT = true;
            for (int i = 0; i < txtSDTKH.Text.Length; i++)
            {
                if (!(txtSDTKH.Text[i] >= 48 && txtSDTKH.Text[i] <= 57))
                {

                    lblThongBaoSDT.Content = "SĐT Khách Hàng không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBaoSDT.Visibility = Visibility.Visible;
                    timer.Start();
                    checkSDT = false;
                    break;
                }

            }
            if (checkSDT)
            {
                lblThongBaoSDT.Visibility = Visibility.Collapsed;
            }
        }

        private void txtEmailKH_LostFocus(object sender, RoutedEventArgs e)
        {
             checkEmail = true;
            if (!(txtEmailKH.Text.Contains("@gmail.com"))) //Check Email Khách hàng
            {
                lblThongBaoEmail.Content = "Email Khách Hàng không hợp lệ!";
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBaoEmail.Visibility = Visibility.Visible;
                timer.Start();
                checkEmail = false;
                
            }
            if (checkEmail)
            {
                lblThongBaoEmail.Visibility = Visibility.Collapsed;
            }
        }

        private void txtDiaChiKH_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiaChiKH.Text))
            {
                checkDiaChi = true;
            }
        }

    }
}
