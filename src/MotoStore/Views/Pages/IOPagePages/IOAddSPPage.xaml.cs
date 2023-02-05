using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using System.Collections.ObjectModel;
using System.Linq;
using MotoStore.Views.Pages.LoginPages;
using Microsoft.WindowsAPICodePack.Dialogs;
using MotoStore.Properties;
using System.IO;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOAddSPPage.xaml
    /// </summary>
    public partial class IOAddSPPage : Page
    {
        public IOAddSPPage()
        {
            InitializeComponent();
            RefreshMatHang();
            DataContext = this;
        }
        internal ObservableCollection<NhaCungCap> nhaCungCaps;
        private string? fileAnh = null;

        public void RefreshMatHang()
        {
            MainDatabase dtb = new();
            nhaCungCaps = new(dtb.NhaCungCaps);
            foreach (var mat in nhaCungCaps.ToList())
            {
                if (mat.DaXoa)
                    nhaCungCaps.Remove(mat);
            }
            cmbMaNCC.ItemsSource = nhaCungCaps;
            cmbMaNCC.Text = string.Empty;
        }

        private void btnLoadImageSP_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog OFD = new();
            OFD.Filters.Add(new("Image File", "jpg,jpeg,png"));
            if (OFD.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Uri fileUri = new(OFD.FileName);
                ImageSP.Source = new BitmapImage(fileUri);
                fileAnh = OFD.FileName;
            }
        }   

        private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text)|| string.IsNullOrEmpty(txtGiaNhapSP.Text)|| string.IsNullOrWhiteSpace(txtXuatXuSP.Text)|| string.IsNullOrEmpty(txtPhanKhoiSP.Text)|| string.IsNullOrWhiteSpace(cmbMaNCC.Text))
                MessageBox.Show("Các trường dữ liệu có dấu * không được để trống!");
            else
            {
                using SqlConnection con = new(Settings.Default.ConnectionString);
                con.Open();
                try 
                {
                    SqlCommand cmd = new("Set Dateformat dmy\nInsert into MatHang values(@TenSP,@PhanKhoi,null,@GiaNhap,null,0,@MaNCC,@HangSX,@XuatXu,@MoTa,0)", con);
                    cmd.Parameters.Add("@TenSP", System.Data.SqlDbType.NVarChar).Value = txtTenSP.Text;
                    cmd.Parameters.Add("@PhanKhoi", System.Data.SqlDbType.Int).Value = txtPhanKhoiSP.Text;
                    cmd.Parameters.Add("@GiaNhap", System.Data.SqlDbType.Money).Value = txtGiaNhapSP.Text;
                    cmd.Parameters.Add("@MaNCC", System.Data.SqlDbType.VarChar).Value = cmbMaNCC.Text;
                    cmd.Parameters.Add("@HangSX", System.Data.SqlDbType.NVarChar).Value = txtHangSXSP.Text;
                    cmd.Parameters.Add("@XuatXu", System.Data.SqlDbType.NVarChar).Value = txtXuatXuSP.Text;
                    cmd.Parameters.Add("@MoTa", System.Data.SqlDbType.NVarChar).Value = txtMoTaSP.Text;
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("Select top(1) MaMH from MatHang order by ID desc", con);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string MHMoi = "MH@";
                    if (sda.Read())
                        MHMoi = (string)sda[0];
                    DateTime dt = DateTime.Now;
                    cmd = new("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getNV.MaNv + "', '" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', N'thêm mới Mặt Hàng " + MHMoi + " ')", con);
                    cmd.ExecuteNonQuery();
                    if (fileAnh is not null)
                        File.Copy(fileAnh, Path.Combine(Settings.Default.ProductFilePath, MHMoi));
                    MessageBox.Show("Thêm mới dữ liệu thành công");
                    PageRefresh();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Thêm mới sản phẩm thất bại, Lỗi: " + ex.Message);
                }
                con.Close();
            }
        }

        private void PageRefresh()
        {
            txtTenSP.Clear();
            txtHangSXSP.Clear();
            txtGiaNhapSP.Clear();
            txtPhanKhoiSP.Clear();
            txtMoTaSP.Clear();
            txtXuatXuSP.Clear();
            ImageSP.Source = null;
            RefreshMatHang();
        }

        private void txtTenSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
                lblThongBao.Visibility = Visibility.Visible;
        }

        private void txtGiaNhapSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtGiaNhapSP.Text))
                lblThongBao.Visibility = Visibility.Visible;
        }

        private void txtXuatXuSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtXuatXuSP.Text))
                lblThongBao.Visibility = Visibility.Visible;
        }

        private void txtPhanKhoiSP_LostFocus(object sender, RoutedEventArgs e) //Check Phân Phối của Xe
        {
            if (string.IsNullOrEmpty(txtPhanKhoiSP.Text))
                lblThongBao.Visibility = Visibility.Visible;
        }

        private void cmbMaNCC_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbMaNCC.SelectedItem is NhaCungCap ncc)
                cmbMaNCC.Text = ncc.MaNcc;
        }

        private void cmbMaNCC_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbMaNCC.Text))
                lblThongBao.Visibility = Visibility.Visible;
        }
    }
}