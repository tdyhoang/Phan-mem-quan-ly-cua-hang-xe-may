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
    /// Interaction logic for WarrantyListPage.xaml
    /// </summary>
    public partial class WarrantyListPage
    {
        internal ObservableCollection<ThongTinBaoHanh> TableData;
        bool isQuanLy = default;

        public WarrantyListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.ThongTinBaoHanhs);
            if (isQuanLy)
                grdWarranty.ItemsSource = TableData;
            else
            {
                foreach (var bh in TableData.ToList())
                    if (bh.MaNv != PageChinh.getMa)
                        TableData.Remove(bh);
                grdWarranty.ItemsSource = TableData;
            }
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdWarranty.ItemsSource select grdWarranty.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                    foreach (var obj in grdWarranty.Items)
                    {
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value) || string.Equals(value, "0") || string.Equals(value, PageChinh.getMa)))
                            continue;
                        if (obj is not ThongTinBaoHanh bh)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(bh.MaKh))
                            throw new("Mã khách hàng không được để trống!");
                        if (string.IsNullOrEmpty(bh.MaMh))
                            throw new("Mã mặt hàng không được để trống!");
                        string ngayHetHan = bh.ThoiGian.HasValue ? $"'{bh.ThoiGian.Value:dd/MM/yyyy}'" : "null";

                        // Thêm mới
                        if (string.IsNullOrEmpty(bh.MaBh))
                            cmd.CommandText += $"\nInsert into ThongTinBaoHanh values('{bh.MaMh}', '{bh.MaKh}', '{bh.MaNv}', {ngayHetHan}, N'{bh.GhiChu}')";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate ThongTinBaoHanh Set MaMh = '{bh.MaMh}', MaKh = '{bh.MaKh}', MaNv = '{bh.MaNv}', ThoiGian = {ngayHetHan}, GhiChu = N'{bh.GhiChu}' Where MaBh = '{bh.MaBh}';";
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
                            if (obj is not ThongTinBaoHanh bh)
                                continue;
                            // Trường hợp chưa thêm mới nên chưa có mã bh
                            if (string.IsNullOrEmpty(bh.MaBh))
                                continue;
                            // Xóa hàng
                            else
                                cmd.CommandText += $"Delete From ThongTinBaoHanh Where MaBh = '{bh.MaBh}';\n";
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
                isQuanLy = string.Equals(PageChinh.getChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);
                grdWarranty.IsReadOnly = !isQuanLy;
                grdWarranty.Columns[3].Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                if (sender is Button button)
                    button.Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                RefreshDataGrid();
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new() { MaNv = PageChinh.getMa });

        // Đẩy event mousewheel cho scrollviewer xử lý
        private void grdWarranty_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
