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
    public partial class CustomerListPage : INavigableView<ViewModels.CustomerListViewModel>
    {
        public ViewModels.CustomerListViewModel ViewModel
        {
            get;
        }

        internal ObservableCollection<KhachHang> TableData;

        public CustomerListPage(ViewModels.CustomerListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.KhachHangs);
            foreach (var khachHang in TableData.ToList())
                if (khachHang.DaXoa)
                    TableData.Remove(khachHang);
            grdCustomer.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdCustomer.ItemsSource select grdCustomer.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                    cmd = new("set dateformat dmy", con);
                    cmd.Transaction = trans;

                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in grdCustomer.Items)
                    {
                        // Trường hợp gặp dòng trắng được người dùng thêm mà chưa chỉnh sửa
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not KhachHang kh)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(kh.GioiTinh))
                            throw new("Giới tính không được để trống!");
                        if (string.IsNullOrEmpty(kh.LoaiKh))
                            throw new("Loại khách hàng không được để trống!");
                        string ngSinh = string.Empty;
                        if (kh.NgSinh.HasValue)
                            ngSinh = kh.NgSinh.Value.ToString("dd/MM/yyyy");

                        // Thêm mới
                        if (string.IsNullOrEmpty(kh.MaKh))
                            cmd.CommandText += $"\nInsert into KhachHang values(N'{kh.HoTenKh}', '{ngSinh}', N'{kh.GioiTinh}', N'{kh.DiaChi}', '{kh.Sdt}', '{kh.Email}', N'{kh.LoaiKh}', 0)";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate KhachHang Set HotenKh = N'{kh.HoTenKh}', NgSinh = '{ngSinh}', GioiTinh = N'{kh.GioiTinh}', DiaChi = N'{kh.DiaChi}', Sdt = '{kh.Sdt}', Email = '{kh.Email}', LoaiKh = N'{kh.LoaiKh}', DaXoa = 0 Where MaKh = '{kh.MaKh}';";
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
                            if (obj is not KhachHang kh)
                                continue;
                            // Trường hợp chưa thêm mới nên chưa có mã KH
                            if (string.IsNullOrEmpty(kh.MaKh))
                                continue;
                            // Xóa hàng
                            else
                                cmd.CommandText += $"Update KhachHang Set DaXoa = 1 Where MaKh = '{kh.MaKh}';\n";
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

                grdCustomer.IsReadOnly = !isQuanLy;

                if (sender is Button button)
                    button.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

        // Đẩy event mousewheel cho scrollviewer xử lý
        private void grdCustomer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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