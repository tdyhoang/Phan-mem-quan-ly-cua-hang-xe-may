using MotoStore.Database;
using System.Collections.ObjectModel;
using System;
using System.Windows;
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

        internal ObservableCollection<HoaDon> TableData = new();

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
            MainDatabase mainDatabase = new();
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            try
            {
                con.Open();
                cmd = new("begin transaction\nset dateformat dmy", con);
                cmd.ExecuteNonQuery();

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdInvoice.Items)
                {
                    bool khExist = false;
                    bool mhExist = false;
                    bool nvExist = false;
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    if (obj is not HoaDon hd)
                        continue;
                    // Kiểm tra dữ liệu có tồn tại không
                    foreach (var kh in mainDatabase.KhachHangs)
                        if (!kh.DaXoa && hd.MaKh == kh.MaKh)
                            khExist = true;
                    if (!khExist)
                        throw new("Mã khách hàng không tồn tại hoặc đã xóa!");
                    foreach (var mh in mainDatabase.MatHangs)
                        if (!mh.DaXoa && hd.MaMh == mh.MaMh)
                            mhExist = true;
                    if (!mhExist)
                        throw new("Mã mặt hàng không tồn tại hoặc đã xóa!");
                    foreach (var nv in mainDatabase.NhanViens)
                        if (!nv.DaXoa && hd.MaNv == nv.MaNv)
                            nvExist = true;
                    if (!nvExist)
                        throw new("Mã nhân viên không tồn tại hoặc đã xóa!");
                    // Giá trị mặc định
                    hd.SoLuong ??= 0;
                    hd.ThanhTien ??= 0;

                    // Thêm mới
                    if (string.IsNullOrEmpty(hd.MaHd))
                    {
                        cmd = new($"Insert into HoaDon values('{hd.MaMh}', '{hd.MaKh}', '{hd.MaNv}', '{hd.NgayLapHd}', {hd.SoLuong}, {hd.ThanhTien})", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new($"Update HoaDon Set MaMh = '{hd.MaMh}', MaKh = '{hd.MaKh}', MaNv = '{hd.MaNv}', NgayLapHd = '{hd.NgayLapHd}', SoLuong = {hd.SoLuong}, ThanhTien = {hd.ThanhTien} Where MaHd = '{hd.MaHd}';", con);
                        cmd.ExecuteNonQuery();
                    }
                }

                con.Close();
                // Làm mới nội dung hiển thị cho khớp với database
                RefreshDataGrid();
                MessageBox.Show("Lưu chỉnh sửa thành công!");
            }
            catch (Exception ex)
            {
                cmd = new("rollback transaction", con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show(ex.Message);
            }
        }

        // Định nghĩa lại phím tắt Delete
        private new void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not DataGrid dg)
                return;
            // Kiểm tra nếu không được phép chỉnh sửa thì không được xoá
            if (grdInvoice.IsReadOnly)
                return;
            // Kiểm tra xem key Delete có thực sự được bấm tại 1 hàng hoặc ô trong datagrid hay không
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            if (dep is not DataGridRow && dep is not DataGridCell)
                return;
            // Kiểm tra xem key Delete có được bấm trong khi đang chỉnh sửa ô hay không
            DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
            if (e.Key == Key.Delete && !dgr.IsEditing)
            {
                // Nếu đáp ứng đủ điều kiện sẽ bắt đầu vòng lặp để xóa
                DeleteRow(sender, e);
            }
        }

        private void RefreshView(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            try
            {
                con.Open();
                cmd = new("begin transaction", con);
                cmd.ExecuteNonQuery();

                foreach (object obj in grdInvoice.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not HoaDon chỉ để an toàn
                    if (obj is not HoaDon hd)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã hd
                    if (string.IsNullOrEmpty(hd.MaHd))
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new("Delete From HoaDon Where MaHd = '" + hd.MaHd + "';", con);
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                cmd = new("rollback transaction", con);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show(ex.Message);
                // Báo đã thực hiện xong event để ngăn handler mặc định cho phím này hoạt động
                e.Handled = true;
            }
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                bool isQuanLy = PageChinh.getChucVu.ToLower() == "quản lý";

                grdInvoice.IsReadOnly = !isQuanLy;

                if (sender is MenuItem item)
                    item.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());
    }
}