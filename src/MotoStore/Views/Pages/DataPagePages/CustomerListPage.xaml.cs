using MotoStore.Database;
using MotoStore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using System.Diagnostics;
using Wpf.Ui.Common.Interfaces;
using System.Windows.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MotoStore.Views.Pages.LoginPages;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Linq;

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
            if ((from c in (from object i in grdCustomer.ItemsSource select grdCustomer.ItemContainerGenerator.ContainerFromItem(i)) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                        string ngSinh = string.Empty;
                        if (kh.NgSinh.HasValue)
                            ngSinh = kh.NgSinh.Value.ToString("dd/MM/yyyy");
                        if (string.IsNullOrEmpty(kh.GioiTinh))
                            throw new("Giới tính không được để trống!");
                        if (string.IsNullOrEmpty(kh.LoaiKh))
                            throw new("Loại khách hàng không được để trống!");

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
            if (grdCustomer.IsReadOnly)
                return;
            // Kiểm tra xem key Delete có thực sự được bấm tại 1 hàng hoặc ô trong datagrid hay không
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            if (dep is not DataGridRow && dep is not DataGridCell)
                return;
            // Kiểm tra xem key Delete có được bấm trong khi đang chỉnh sửa ô hay không
            DataGridRow dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
            if (e.Key == System.Windows.Input.Key.Delete && !dgr.IsEditing)
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

                    foreach (var obj in grdCustomer.SelectedItems)
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

        private void RefreshView(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                bool isQuanLy = string.Equals(PageChinh.getChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);

                grdCustomer.IsReadOnly = !isQuanLy;

                if (sender is MenuItem item)
                    item.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());
    }
}