using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.Globalization;
using MotoStore.Views.Pages.LoginPages;
using System.Collections.Generic;
using System.Linq;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOKhachHangPage.xaml
    /// </summary>
    public partial class IOKhachHangPage : Page
    {
        internal List<Control> InputFields;
        public IOKhachHangPage()
        {
            InitializeComponent();
            PageInitialization();
        }
        private void PageInitialization()
        {
            InputFields = new()
            {
                txtTenKH,
                txtNgaySinhKH,
                txtDiaChiKH,
                txtSDTKH,
                txtEmailKH,        
                cmbLoaiKH,
                cmbGioiTinhKH
            };
        }
     

        private void btnAddNewKhachHang_Click(object sender, RoutedEventArgs e)
        {

            foreach (var c in InputFields.Where(c => Validation.GetHasError(c)))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể thêm!");
                return;
            }
            foreach (var tbx in InputFields.Where(c => c is TextBox).Cast<TextBox>().Where(tbx => !string.Equals(tbx.Name, "txtDiaChiKH") && !string.Equals(tbx.Name, "txtNgaySinhKH") && string.IsNullOrEmpty(tbx.Text)))
            {
                MessageBox.Show("Các ô được đánh dấu * không được để trống!");
                return;
            }
            foreach (var cmb in InputFields.Where(c => c is ComboBox).Cast<ComboBox>().Where(cmb => string.IsNullOrEmpty(cmb.Text)))
            {
                MessageBox.Show("Các ô được đánh dấu * không được để trống!");
                return;
            }

            using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            try
            {
                con.Open();
                using var trans = con.BeginTransaction();
                try
                {
                    SqlCommand cmd = new("Set Dateformat dmy\nInsert into KhachHang values(@TenKH,@NgaySinhKH,@GioiTinh,@DiaChi,@SDT,@Email,@LoaiKH,0)", con,trans);
                    //txtTenKH.Text + txtNgaySinhKH.Text cmbGioiTinhKH.Text ++txtDiaChiKH.Text + txtSDTKH.Tex+, + txtEmailKH.Text+ cmbLoaiKH.Text + 0
                    cmd.Parameters.Add("@TenKH", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@TenKh"].Value = txtTenKH.Text;
                    cmd.Parameters.Add("@NgaySinhKH", System.Data.SqlDbType.SmallDateTime).Value = DateTime.TryParseExact(txtNgaySinhKH.Text, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var NgaySinh) ? NgaySinh : DBNull.Value;
                    cmd.Parameters.Add("@GioiTinh", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@GioiTinh"].Value = cmbGioiTinhKH.Text;
                    cmd.Parameters.Add("@DiaChi", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@DiaChi"].Value = txtDiaChiKH.Text;
                    cmd.Parameters.Add("@SDT", System.Data.SqlDbType.VarChar);
                    cmd.Parameters["@SDT"].Value = txtSDTKH.Text;
                    cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@Email"].Value = txtEmailKH.Text;
                    cmd.Parameters.Add("@LoaiKH", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@LoaiKH"].Value = cmbLoaiKH.Text;
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("Select top(1) MaKH from KhachHang order by ID desc", con,trans);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string KHMoi = "KH@";
                    if (sda.Read())
                    {
                        KHMoi = (string)sda[0];
                        sda.Close();    
                    }                      
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Khách Hàng " + KHMoi + "')", con,trans);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                 
                    MessageBox.Show("Thêm Dữ Liệu Thành Công");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Thêm mới thất bại, Lỗi: " + ex.Message);
                    trans.Rollback();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (sender is ComboBox cmb)
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
        }






    }
}
