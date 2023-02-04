using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using MotoStore.Views.Pages.LoginPages;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Security.Cryptography;
using MotoStore.Helpers;

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
                ImageSP.Source = BitmapConverter.FilePathToBitmapImage(OFD.FileName);
        }
       

        private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd;
            using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            try
            {
                if (string.IsNullOrWhiteSpace(txtTenSP.Text) || string.IsNullOrEmpty(txtGiaNhapSP.Text) || string.IsNullOrWhiteSpace(txtXuatXuSP.Text) || string.IsNullOrEmpty(txtPhanKhoiSP.Text) || string.IsNullOrWhiteSpace(cmbMaNCC.Text))
                    MessageBox.Show("Các trường dữ liệu có dấu * không được để trống!");
                else
                {
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into MatHang values('" + txtTenSP.Text + "', " + txtPhanKhoiSP.Text + ", null, '" + txtGiaNhapSP.Text + "', null, 0, '" + cmbMaNCC.Text + "', '" + txtHangSXSP.Text + "', N'" + txtXuatXuSP.Text + "', N'" + txtMoTaSP.Text + "', 0)", con);
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand("Select top(1) MaMH from MatHang order by ID desc", con);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string MHMoi = "MH@";
                    if (sda.Read())
                        MHMoi = (string)sda[0];
                    DateTime dt = DateTime.Now;
                    cmd = new("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getNV.MaNv + "', '" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', N'thêm mới Mặt Hàng " + MHMoi + " ')", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm mới dữ liệu thành công");
                }
            }
            catch (Exception ex)
            {

            }
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