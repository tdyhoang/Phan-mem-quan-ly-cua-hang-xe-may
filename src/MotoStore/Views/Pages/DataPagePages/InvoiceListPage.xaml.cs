using MotoStore.Database;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using MotoStore.Views.Pages.LoginPages;
using MotoStore.Helpers;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class InvoiceListPage
    {
        internal ObservableCollection<HoaDon> TableData;

        public InvoiceListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.HoaDons);
            grdInvoice.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdInvoice.ItemsSource select grdInvoice.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể lưu!");
                return;
            }
            SqlCommand cmd;
            using SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            try
            {
                con.Open();
                using var trans = con.BeginTransaction();
                try
                {
                    cmd = new("set dateformat dmy", con, trans);
                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in grdInvoice.Items)
                    {
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value) || string.Equals(value, "0") || string.Equals(value, PageChinh.getMa) || string.Equals(value, DateTime.Today.ToString("dd/MM/yyyy"))))
                            continue;
                        if (obj is not HoaDon hd)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(hd.MaKh))
                            throw new("Mã khách hàng không được để trống!");
                        if (string.IsNullOrEmpty(hd.MaMh))
                            throw new("Mã mặt hàng không được để trống!");
                        string ngayLapHd = hd.NgayLapHd.HasValue ? $"'{hd.NgayLapHd.Value:dd/MM/yyyy}'" : "null";

                        // Thêm mới
                        if (string.IsNullOrEmpty(hd.MaHd))
                            cmd.CommandText += $"\nInsert into HoaDon values('{hd.MaMh}', '{hd.MaKh}', '{hd.MaNv}', {ngayLapHd}, {hd.SoLuong}, {hd.ThanhTien})";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate HoaDon Set MaMh = '{hd.MaMh}', MaKh = '{hd.MaKh}', MaNv = '{hd.MaNv}', NgayLapHd = {ngayLapHd}, SoLuong = {hd.SoLuong}, ThanhTien = {hd.ThanhTien} Where MaHd = '{hd.MaHd}';";
                    }
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    // Làm mới nội dung hiển thị cho khớp với database
                    RefreshDataGrid();
                    MessageBox.Show("Lưu chỉnh sửa thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.Rollback();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Định nghĩa lại phím tắt Delete
        private new void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not DataGrid dg)
                return;
            // Kiểm tra nếu không được phép chỉnh sửa thì không được xoá
            if (dg.IsReadOnly)
                return;
            // Kiểm tra xem key Delete có thực sự được bấm tại 1 hàng hoặc ô trong datagrid hay không
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            if (dep is not DataGridRow && dep is not DataGridCell)
                return;
            // Kiểm tra xem key Delete có được bấm trong khi đang chỉnh sửa ô hay không
            DataGridRow dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
            if (e.Key == Key.Delete && !dgr.IsEditing)
            {
                // Nếu đáp ứng đủ điều kiện sẽ bắt đầu vòng lặp để xóa
                SqlCommand cmd;
                using SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                try
                {
                    con.Open();
                    using var trans = con.BeginTransaction();
                    try
                    {
                        cmd = new(" ", con, trans);

                        foreach (var obj in dg.SelectedItems)
                        {
                            if (obj is not HoaDon hd)
                                continue;
                            // Trường hợp chưa thêm mới nên chưa có mã hd
                            if (string.IsNullOrEmpty(hd.MaHd))
                                continue;
                            // Xóa hàng
                            else
                                cmd.CommandText += $"Delete From HoaDon Where MaHd = '{hd.MaHd}';\n";
                        }
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                        // Báo đã thực hiện xong event để ngăn handler mặc định cho phím này hoạt động
                        e.Handled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    // Báo đã thực hiện xong event để ngăn handler mặc định cho phím này hoạt động
                    e.Handled = true;
                }
            }
        }

        private void RefreshView(object sender, RoutedEventArgs e)
            => RefreshDataGrid();

        // Tắt các hoạt động chỉnh sửa data nếu không phải quản lý
        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                bool isQuanLy = string.Equals(PageChinh.getChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);
                grdInvoice.IsReadOnly = !isQuanLy;

                if (sender is Button button)
                    button.Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                RefreshDataGrid();
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new() { MaNv = PageChinh.getMa, NgayLapHd = DateTime.Today });

        // Đẩy event mousewheel cho scrollviewer xử lý
        private void grdInvoice_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            var parent = ((Control)sender).Parent as UIElement;
            parent?.RaiseEvent(eventArg);
        }

        private void grdInvoice_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                DataGridColumn col = e.Column;
                DataGridRow row = e.Row;
                var editingElement = e.EditingElement as TextBox;
                var newValue = editingElement.Text;

                if (row.Item is not HoaDon hd || string.IsNullOrEmpty(newValue))
                    return;
                switch (col.Header)
                {
                    case "Mã MH": hd.MaMh = newValue; break;
                    case "Mã KH": hd.MaKh = newValue; break;
                    case "Số lượng": hd.SoLuong = int.Parse(newValue); break;
                    default: return;
                }
                if (string.IsNullOrEmpty(hd.MaMh) || string.IsNullOrEmpty(hd.MaKh))
                    return;

                MainDatabase mdb = new();
                string loaiKh = default;
                decimal giamGia = default;
                decimal? giaBan = default;

                foreach (var kh in mdb.KhachHangs)
                    if (!kh.DaXoa && kh.MaKh == hd.MaKh)
                        loaiKh = kh.LoaiKh;
                foreach (var mh in mdb.MatHangs)
                    if (!mh.DaXoa && mh.MaMh == hd.MaMh)
                        giaBan = mh.GiaBanMh;

                if (giaBan is null)
                    return;
                switch (loaiKh)
                {
                    case "Vip": giamGia = 0.15M; break;
                    case "Thân quen": giamGia = 0.05M; break;
                    case "Thường": giamGia = 0; break;
                    default: return;
                }

                ((HoaDon)e.Row.Item).ThanhTien = giaBan.Value * hd.SoLuong * (1 - giamGia);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}