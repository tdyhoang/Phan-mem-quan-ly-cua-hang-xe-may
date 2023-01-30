using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Data.SqlClient;
using MotoStore.Views.Pages.LoginPages;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IONhaSXPage.xaml
    /// </summary>
    public partial class IONhaSXPage : Page
    {
        bool checkTenNCC = false;
        public IONhaSXPage()
        {
            InitializeComponent();
        }

        private void btnAddNewNSX_Click(object sender, RoutedEventArgs e)
        {           
            SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            SqlCommand cmd;
            if (!checkTenNCC)
                MessageBox.Show("Các Trường Dữ Liệu Quan Trọng(Có Dấu (*) Đang Bị Thiếu), Vui Lòng Kiểm Tra Lại");
            else
            {
                con.Open();
                cmd = new("Set Dateformat dmy\nInsert into NhaCungCap values('" + txtTenNCC.Text + "','" + txtSDTNSX.Text + "','" + txtEmailNSX.Text + "',N'" + txtDiaChi.Text + " ' ,0)", con);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("Select top(1) MaNCC from NhaCungCap order by ID desc", con);
                SqlDataReader sda = cmd.ExecuteReader();
                string NCCMoi = "NCC@";
                if (sda.Read())
                    NCCMoi = (string)sda[0];
                DateTime now = DateTime.Now;
                cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Nhà Cung Cấp " + NCCMoi + "')", con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Thêm dữ liệu thành công");
            }
        }

        //check Email
        private void txtEmailNSX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtEmailNSX.Text.Contains("@gmail.com"))
                lblThongBaoEmail.Visibility = Visibility.Visible;
            else
                lblThongBaoEmail.Visibility = Visibility.Collapsed;
        }

        private void txtTenNCC_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNCC.Text))
                lblThongBao.Visibility = Visibility.Visible;
            else
            {
                checkTenNCC = true;
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }
    }
}

