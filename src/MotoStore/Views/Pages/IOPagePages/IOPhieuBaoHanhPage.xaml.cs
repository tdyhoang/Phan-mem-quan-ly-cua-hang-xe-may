using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MotoStore.Views.Pages.LoginPages;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOPhieuBaoHanhPage.xaml
    /// </summary>
    public partial class IOPhieuBaoHanhPage : Page
    {
        internal ObservableCollection<HoaDon> HoaDons;
        internal List<Control> InputFields;
        public IOPhieuBaoHanhPage()
        {
            InitializeComponent();
            PageInitialization();
        }
        private void PageInitialization()
        {
            InputFields = new()
            {
                cmbMaHD,
                txtThoiGian,
                txtGhiChu
            };
            PageRefresh();
        }
        private void PageRefresh()
        {
            RefreshHoaDon();
            
            foreach (var tbx in InputFields.Where(c => c is TextBox).Cast<TextBox>())
                tbx.Clear();
            txtThoiGian.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
        public void RefreshHoaDon()
        {
            MainDatabase dtb = new();
            HoaDons = new(dtb.HoaDons);
            cmbMaHD.ItemsSource = HoaDons;
            cmbMaHD.Text = string.Empty;
        }

        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (sender is ComboBox cmb)
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
        }
        private void cmbMaHD_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                if (cmb.SelectedItem is HoaDon hd)
                    cmb.Text = hd.MaHd;
                cmb.GetBindingExpression(ComboBox.TextProperty).UpdateSource();
            }
        }
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox cmb)
                return;

            CollectionView collection = (CollectionView)CollectionViewSource.GetDefaultView(cmb.ItemsSource);
            if (string.Equals(cmb.Name, "cmbMaHD"))
            {
                collection.Filter = (c) =>
                {
                    if (string.IsNullOrEmpty(cmb.Text))
                        return true;
                    if (((HoaDon)c).MaHd.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((HoaDon)c).MaKh.Contains(cmb.Text, StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (((HoaDon)c).NgayLapHd.HasValue)
                        return ((HoaDon)c).NgayLapHd.Value.ToString("dd/MM/yyyy").Contains(cmb.Text);
                    return false;
                    
                };
            }
        
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
        private void btnRefreshHD_Click(object sender, RoutedEventArgs e)
        {
            RefreshHoaDon();
        }


        private void btnAddPBH_Click(object sender, RoutedEventArgs e)
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
                    string thoigian = string.IsNullOrEmpty(txtThoiGian.Text) ? "null" : $"'{txtThoiGian.Text}'";
                    SqlCommand cmd = new("Set Dateformat dmy\n Insert into ThongTinBaoHanh values(@MaHD,@ThoiGian,@GhiChu)", con, trans);
                    cmd.Parameters.Add("@MaHD", System.Data.SqlDbType.VarChar).Value = cmbMaHD.Text;                 
                    cmd.Parameters.Add("@ThoiGian", System.Data.SqlDbType.SmallDateTime).Value = DateTime.TryParseExact(txtThoiGian.Text, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var Thoigian) ? Thoigian : DBNull.Value;
                    cmd.Parameters.Add("@GhiChu", System.Data.SqlDbType.NVarChar).Value = txtGhiChu.Text;
                   
                    cmd.ExecuteNonQuery();
                    cmd = new("Select top(1) MaBH from ThongTinBaoHanh order by ID desc", con, trans);
                    SqlDataReader sda = cmd.ExecuteReader();
                    string PhieuBHMoi = "PBH@";
                    if (sda.Read())
                    {
                        PhieuBHMoi = (string)sda[0];
                        sda.Close();
                    }
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Hoá Đơn " + PhieuBHMoi + "')", con, trans);
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

    }
}
