using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using MotoStore.Views.Pages.LoginPages;
using MotoStore.Database;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;

namespace MotoStore.Views.Pages.IOPagePages

{
    /// <summary>
    /// Interaction logic for IOHoaDonPage.xaml
    /// </summary>
    public partial class IOHoaDonPage : Page
    {
        internal ObservableCollection<MatHang> matHangs;
        internal ObservableCollection<KhachHang> KhachHangs;
        internal List<Control> InputFields;

        public IOHoaDonPage()
        {
            InitializeComponent();
            PageInitialization();
        }

        private void PageInitialization()
        {
            InputFields = new()
            {
                cmbMaSPHD,
                cmbMaKHHD,
                txtNgayXuatHD,
                txtGiamGiaHD,
                txtSoLuongHD,
                txtThanhTienHD
            };

            PageRefresh();
        }

        private void PageRefresh()
        {
            RefreshMatHang();
            RefreshKhachHang();
            foreach (var tbx in InputFields.Where(c => c is TextBox).Cast<TextBox>())
                tbx.Clear();
            txtNgayXuatHD.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
        private void btnAddNewHoaDon_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in InputFields.Where(c => Validation.GetHasError(c)))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể thêm!");
                return;
            }
            foreach (var tbx in InputFields.Where(c => c is TextBox).Cast<TextBox>().Where(tbx => !string.Equals(tbx.Name, "txtNgayXuatHD") && string.IsNullOrEmpty(tbx.Text)))
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
                    string ngayXuatHD = string.IsNullOrEmpty(txtNgayXuatHD.Text) ? "null" : $"'{txtNgayXuatHD.Text}'";
                    SqlCommand cmd = new ("Set Dateformat dmy\n Insert into HoaDon values(@MaSP,@MaKH,@MaNV,@NgayHD,@SoLuongHD,@ThanhTien)", con,trans );
                    cmd.Parameters.Add("@MaSP", System.Data.SqlDbType.VarChar).Value = cmbMaSPHD.Text;
                    cmd.Parameters.Add("@MaKH", System.Data.SqlDbType.VarChar).Value = cmbMaKHHD.Text;
                    cmd.Parameters.Add("@MaNV", System.Data.SqlDbType.VarChar).Value = PageChinh.getNV.MaNv;
                    cmd.Parameters.Add("@NgayHD", System.Data.SqlDbType.SmallDateTime).Value = DateTime.TryParseExact(txtNgayXuatHD.Text, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var ngayxuathd) ? ngayxuathd : DBNull.Value;
                    cmd.Parameters.Add("@SoLuongHD", System.Data.SqlDbType.Int).Value = int.Parse(txtSoLuongHD.Text);
                    cmd.Parameters.Add("@ThanhTien", System.Data.SqlDbType.Money).Value = decimal.Parse(txtThanhTienHD.Text);
                    cmd.ExecuteNonQuery();
                    cmd = new("Select top(1) MaHD from HoaDon order by ID desc", con, trans);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string HDMoi = "HD@";
                    if (sda.Read())
                    {
                        HDMoi = (string)sda[0];
                        sda.Close();
                    }
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Hoá Đơn " + HDMoi + "')", con, trans);
                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    PageRefresh();
                    MessageBox.Show("Thêm dữ liệu thành công");
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

        public void UpdateGiamGia()
        {
            if (string.IsNullOrEmpty(cmbMaKHHD.Text) || Validation.GetHasError(cmbMaKHHD))
                return;

            string LoaiKH = string.Empty;
            foreach (var cus in KhachHangs.Where(kh => string.Equals(cmbMaKHHD.Text, kh.MaKh)))
                LoaiKH = cus.LoaiKh;

            txtGiamGiaHD.Text = LoaiKH switch
            {
                "Vip" => "15%",
                "Thân quen" => "5%",
                "Thường" => "0%",
                _ => "0%",
            };
        }

        public void UpdateThanhTien()
        {
            if (Validation.GetHasError(cmbMaSPHD) || string.IsNullOrEmpty(cmbMaSPHD.Text) ||
                string.IsNullOrEmpty(txtGiamGiaHD.Text) || string.IsNullOrEmpty(txtSoLuongHD.Text))
                return;

            decimal? giaban = default;
            foreach (var item in matHangs.Where(mh => string.Equals(cmbMaSPHD.Text, mh.MaMh)))
                giaban = item.GiaBanMh;

            decimal? giamgia = txtGiamGiaHD.Text switch
            {
                "0%" => 0,
                "5%" => 0.05M,
                "15%" => 0.15M,
                _ => default,
            };
            decimal? thanhtien = giaban * int.Parse(txtSoLuongHD.Text) * (1 - giamgia);
            txtThanhTienHD.Text = thanhtien.ToString();
        }

        private void txtSoLuongHD_TextChanged(object sender, TextChangedEventArgs e)
            => UpdateThanhTien();

        private void txtGiamGiaHD_TextChanged(object sender, TextChangedEventArgs e)
            => UpdateThanhTien();

        public void RefreshMatHang()
        {
            MainDatabase dtb = new();
            matHangs = new(dtb.MatHangs);
            foreach (var mat in matHangs.Where(mh => mh.DaXoa).ToList())
                matHangs.Remove(mat);
            cmbMaSPHD.ItemsSource = matHangs;
            cmbMaSPHD.Text = string.Empty;
        }

        public void RefreshKhachHang()
        {
            MainDatabase dtb = new();
            KhachHangs = new(dtb.KhachHangs);
            foreach (var khach in KhachHangs.Where(kh => kh.DaXoa).ToList())
                KhachHangs.Remove(khach);
            cmbMaKHHD.ItemsSource = KhachHangs;
            cmbMaKHHD.Text = string.Empty;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshMatHang();

        }

        private void btnRefreshKH_Click(object sender, RoutedEventArgs e)
        {
            RefreshKhachHang();
        }

        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Return || e.Key == Key.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void cmbMaSPHD_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                if (cmb.SelectedItem is MatHang mh)
                    cmb.Text = mh.MaMh;
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }
            UpdateThanhTien();
        }

        private void cmbMaKHHD_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbMaKHHD.SelectedItem is KhachHang kh)
            {
                cmbMaKHHD.Text = kh.MaKh;
            }
            UpdateGiamGia();
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (sender is ComboBox cmb)
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
        }

        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox cmb)
                return;

            CollectionView collection = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            if (string.Equals(cmb.Name, "cmbMaSPHD"))
            {
                collection.Filter = (c) =>
                {
                    if (string.IsNullOrEmpty(cmb.Text))
                        return true;
                    if (((MatHang)c).MaMh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((MatHang)c).TenMh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((MatHang)c).SoPhanKhoi.ToString().Contains(cmb.Text))
                        return true;
                    return false;
                };
            }
            if (string.Equals(cmb.Name, "cmbMaKHHD"))
            {
                collection.Filter = (c) =>
                {
                    if (string.IsNullOrEmpty(cmb.Text))
                        return true;
                    if (((KhachHang)c).MaKh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((KhachHang)c).HoTenKh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((KhachHang)c).NgSinh.HasValue)
                        return ((KhachHang)c).NgSinh.Value.ToString("dd/MM/yyyy").Contains(cmb.Text);
                    return false;
                };
            }
        }
    }
}
