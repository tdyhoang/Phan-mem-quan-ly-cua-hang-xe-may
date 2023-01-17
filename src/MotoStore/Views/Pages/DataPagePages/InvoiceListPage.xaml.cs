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

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class InvoiceListPage : INavigableView<ViewModels.InvoiceListViewModel>
    {
        public ViewModels.InvoiceListViewModel ViewModel
        {
            get;
        }

        internal ObservableCollection<HoaDon> TableData;

        public InvoiceListPage(ViewModels.InvoiceListViewModel viewModel)
        {
            ViewModel = viewModel;
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
            MainDatabase mainDatabase = new();
            SqlCommand cmd;
            using SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            try
            {
                con.Open();
                using var trans = con.BeginTransaction();
                try
                {
                    cmd = new("set dateformat dmy", con);
                    cmd.Transaction = trans;
                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in grdInvoice.Items)
                    {
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not HoaDon hd)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(hd.MaKh))
                            throw new("Mã khách hàng không được để trống!");
                        if (string.IsNullOrEmpty(hd.MaMh))
                            throw new("Mã mặt hàng không được để trống!");
                        if (string.IsNullOrEmpty(hd.MaNv))
                            throw new("Mã nhân viên không được để trống!");
                        string ngayLapHd = string.Empty;
                        if (hd.NgayLapHd.HasValue)
                            ngayLapHd = hd.NgayLapHd.Value.ToString("dd/MM/yyyy");
                        hd.SoLuong ??= 0;
                        hd.ThanhTien ??= 0;

                        // Thêm mới
                        if (string.IsNullOrEmpty(hd.MaHd))
                            cmd.CommandText += $"\nInsert into HoaDon values('{hd.MaMh}', '{hd.MaKh}', '{hd.MaNv}', '{ngayLapHd}', {hd.SoLuong}, {hd.ThanhTien})";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate HoaDon Set MaMh = '{hd.MaMh}', MaKh = '{hd.MaKh}', MaNv = '{hd.MaNv}', NgayLapHd = '{ngayLapHd}', SoLuong = {hd.SoLuong}, ThanhTien = {hd.ThanhTien} Where MaHd = '{hd.MaHd}';";
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
                        cmd = new(" ", con);
                        cmd.Transaction = trans;

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
                    button.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

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
    }
}