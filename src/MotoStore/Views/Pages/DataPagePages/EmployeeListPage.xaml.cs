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
    public partial class EmployeeListPage : INavigableView<ViewModels.EmployeeListViewModel>
    {
        public ViewModels.EmployeeListViewModel ViewModel
        {
            get;
        }

        internal ObservableCollection<NhanVien> TableData;

        public EmployeeListPage(ViewModels.EmployeeListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.NhanViens);
            foreach (var nhanVien in TableData.ToList())
                if (nhanVien.DaXoa)
                    TableData.Remove(nhanVien);
            grdEmployee.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in (from object i in grdEmployee.ItemsSource select grdEmployee.ItemContainerGenerator.ContainerFromItem(i)) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                    foreach (object obj in grdEmployee.Items)
                    {
                        // Trường hợp gặp dòng trắng được người dùng thêm mà chưa chỉnh sửa
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not NhanVien nv)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        string ngaySinhNv = string.Empty;
                        string ngayVaoLam = string.Empty;
                        if (nv.NgSinh.HasValue)
                            ngaySinhNv = nv.NgSinh.Value.ToString("dd-MM-yyyy");
                        if (nv.NgVl.HasValue)
                            ngayVaoLam = nv.NgVl.Value.ToString("dd-MM-yyyy");
                        if (string.IsNullOrEmpty(nv.GioiTinh))
                            throw new("Giới tính không được để trống!");
                        nv.Luong ??= 0;

                        // Thêm mới
                        if (string.IsNullOrEmpty(nv.MaNv))
                            cmd.CommandText += $"\nInsert into NhanVien values(N'{nv.HoTenNv}', '{ngaySinhNv}', N'{nv.GioiTinh}', N'{nv.DiaChi}', '{nv.Sdt}', '{nv.Email}', N'{nv.ChucVu}', '{ngayVaoLam}', {nv.Luong}, 0)";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate NhanVien Set HoTenNv = N'{nv.HoTenNv}', NgSinh = '{ngaySinhNv}', GioiTinh = N'{nv.GioiTinh}', DiaChi = N'{nv.DiaChi}', Sdt = '{nv.Sdt}', Email = '{nv.Email}', ChucVu = N'{nv.ChucVu}', ngVL = '{ngayVaoLam}', Luong = {nv.Luong}, DaXoa = 0 Where Manv = '{nv.MaNv}';";
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
            if (grdEmployee.IsReadOnly)
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
                DeleteRow(sender, e);
            }
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
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

                    foreach (object obj in grdEmployee.SelectedItems)
                    {
                        if (obj is not NhanVien nv)
                            continue;
                        // Trường hợp chưa thêm mới nên chưa có mã nv
                        if (string.IsNullOrEmpty(nv.MaNv))
                            continue;
                        // Xóa hàng
                        else
                            cmd.CommandText += $"Update NhanVien Set DaXoa = 1 Where Manv = '{nv.MaNv}'\n";
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

        private void RefreshView(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                bool isQuanLy = string.Equals(PageChinh.getChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);

                grdEmployee.IsReadOnly = !isQuanLy;

                if (sender is MenuItem item)
                    item.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());
    }
}