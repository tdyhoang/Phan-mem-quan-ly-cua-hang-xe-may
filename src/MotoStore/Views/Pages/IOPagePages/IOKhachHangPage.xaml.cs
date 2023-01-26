using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Windows.Threading;
using MotoStore.Views.Pages.LoginPages;

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

        bool checkTenKH = false;
        bool checkNgaySinh= false;
        bool checkLoaiKH = true;
        bool checkGT = false;
        bool checkEmail = false;

        private void btnAddNewKhachHang_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            SqlCommand cmd;
            if (!(checkTenKH && checkNgaySinh && checkEmail && checkGT && checkLoaiKH))
                MessageBox.Show("Có Những Trường Dữ Liệu Bị Thiếu Hoặc Sai, Vui Lòng Kiểm Tra Lại!");
            else
            {
                try
                {
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into KhachHang values(N'" + txtTenKH.Text + "','" + txtNgaySinhKH.Text + "',N'" + cmbGioiTinhKH.Text + "', N'" + txtDiaChiKH.Text + "','" + txtSDTKH.Text + "','" + txtEmailKH.Text + "',N'" + cmbLoaiKH.Text + " ',0 )", con);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("Select top(1) MaKH from KhachHang order by ID desc", con);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string KHMoi = "KH@";
                    if (sda.Read())
                        KHMoi = (string)sda[0];
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Khách Hàng " + KHMoi + "')", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm Dữ Liệu Thành Công");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Thêm Mới Thất Bại! Lỗi: " + ex.Message);
                }
            }
        }

        private void txtTenKH_LostFocus(object sender, RoutedEventArgs e) // Check Tên Khách Hang
        {
            if (string.IsNullOrEmpty(txtTenKH.Text))
            {
                checkTenKH = false; 
                lblThongBao.Visibility = Visibility.Visible;
            }
            else
            {
                checkTenKH = true;
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }

        private void txtNgaySinhKH_LostFocus(object sender, RoutedEventArgs e) //Check Ngày Sinh Khách Hàng
        {
            DateTime date;
            if (!DateTime.TryParseExact(txtNgaySinhKH.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                lblThongBaoNS.Visibility = Visibility.Visible;
                checkNgaySinh = false;              
            }
            else
            {
                lblThongBaoNS.Visibility = Visibility.Collapsed;
                checkNgaySinh = true;
            }
        }

        private void txtEmailKH_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtEmailKH.Text.Contains("@gmail.com")) //Check Email Khách hàng
            {
                lblThongBaoEmail.Visibility = Visibility.Visible;
                checkEmail = false;
            }
            else
            {
                lblThongBaoEmail.Visibility = Visibility.Collapsed;
                checkEmail = true;
            }
        }

        private void cmbLoaiKH_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbLoaiKH.Text))
            {
                lblTBLoaiKH.Visibility = Visibility.Visible;
                checkLoaiKH = false;
            }
            else
            {
                lblTBLoaiKH.Visibility = Visibility.Collapsed;
                checkLoaiKH = true;
            }
        }

        private void cmbGioiTinhKH_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbGioiTinhKH.Text))
            {
                lblTBGT.Visibility = Visibility.Visible;
                checkGT = false;
            }
            else
            {
                lblTBGT.Visibility = Visibility.Collapsed;
                checkGT = true;
            }
        }
    }
}
