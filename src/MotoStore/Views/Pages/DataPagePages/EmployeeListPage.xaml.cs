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
            DateTime? bdfrom = null;
            DateTime? bdto = null;
            DateTime? jdfrom = null;
            DateTime? jdto = null;
            MainDatabase con = new();
            TableData = new(con.NhanViens);
            foreach (var nhanVien in TableData.ToList())
            {
                if (nhanVien.DaXoa)
                {
                    TableData.Remove(nhanVien);
                    continue;
                }
                if (bdfrom is null || bdfrom > nhanVien.NgSinh)
                    bdfrom = nhanVien.NgSinh;
                if (bdto is null || bdto < nhanVien.NgSinh)
                    bdto = nhanVien.NgSinh;
                if (jdfrom is null || jdfrom > nhanVien.NgVl)
                    jdfrom = nhanVien.NgVl;
                if (jdto is null || jdto < nhanVien.NgVl)
                    jdto = nhanVien.NgVl;
            }
            dpBDFrom.SelectedDate ??= bdfrom;
            dpBDTo.SelectedDate ??= bdto;
            dpJDFrom.SelectedDate ??= jdfrom;
            dpJDTo.SelectedDate ??= jdto;
            // Filter
            foreach (var nhanVien in TableData.ToList())
            {
                if (nhanVien.NgSinh < dpBDFrom.SelectedDate || nhanVien.NgSinh > dpBDTo.SelectedDate)
                    TableData.Remove(nhanVien);
                else if (nhanVien.NgVl < dpJDFrom.SelectedDate || nhanVien.NgVl > dpJDTo.SelectedDate)
                    TableData.Remove(nhanVien);
            }
            grdEmployee.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            try
            {
                con.Open();
                cmd = new("begin transaction\nset dateformat dmy", con);
                cmd.ExecuteNonQuery();
                string ngaySinhNv, ngayVaoLam;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdEmployee.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    if (obj is not NhanVien nv)
                        continue;
                    // Lấy chuỗi ngày sinh theo format dd-MM-yyyy
                    if (nv.NgSinh.HasValue)
                        ngaySinhNv = nv.NgSinh.Value.ToString("dd-MM-yyyy");
                    else
                        ngaySinhNv = string.Empty;
                    // Lấy chuỗi ngày vào làm theo format dd-MM-yyyy
                    if (nv.NgVl.HasValue)
                        ngayVaoLam = nv.NgVl.Value.ToString("dd-MM-yyyy");
                    else
                        ngayVaoLam = string.Empty;
                    // Kiểm tra dữ liệu đã đúng theo định nghĩa chưa
                    if (string.IsNullOrEmpty(nv.GioiTinh) || (nv.GioiTinh != "Nam" && nv.GioiTinh != "Nữ"))
                    {
                        MessageBox.Show("Giới tính chỉ có thể là Nam hoặc Nữ (có dấu)!");
                        return;
                    }
                    if (!string.IsNullOrEmpty(nv.Sdt) && (nv.Sdt.Contains('+') || nv.Sdt.Contains('-')) && int.TryParse(nv.Sdt, out _))
                    {
                        MessageBox.Show("Số điện thoại không được chứa ký tự không phải chữ số!");
                        return;
                    }
                    nv.Luong ??= 0;

                    // Thêm mới
                    if (string.IsNullOrEmpty(nv.MaNv))
                    {
                        cmd = new("Insert into NhanVien values(N'" + nv.HoTenNv + "', '" + ngaySinhNv + "', N'" + nv.GioiTinh + "', N'" + nv.DiaChi + "', '" + nv.Sdt + "', '" + nv.Email + "', N'" + nv.ChucVu + "', '" + ngayVaoLam + "', " + nv.Luong + "0)", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new("Update NhanVien Set HoTenNv = N'" + nv.HoTenNv + "', NgSinh = '" + ngaySinhNv + "', GioiTinh = N'" + nv.GioiTinh + "', DiaChi = N'" + nv.DiaChi + "', Sdt = '" + nv.Sdt + "', Email = '" + nv.Email + "', ChucVu = N'" + nv.ChucVu + "', ngVL = '" + ngayVaoLam + "', Luong = " + nv.Luong + ", DaXoa = 0 Where Manv = '" + nv.MaNv + "';", con);
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
            try
            {
                MainDatabase mainDatabase = new();
                SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                SqlCommand cmd;
                con.Open();

                foreach (object obj in grdEmployee.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not NhanVien chỉ để an toàn
                    if (obj is not NhanVien nv)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã nv
                    if (string.IsNullOrEmpty(nv.MaNv))
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new("Update NhanVien Set DaXoa = 1 Where Manv = '" + nv.MaNv + "'", con);
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
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
                bool isQuanLy = PageChinh.getChucVu.ToLower() == "quản lý";

                grdEmployee.IsReadOnly = !isQuanLy;

                if (sender is MenuItem item)
                    item.IsEnabled = isQuanLy;
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            dpBDFrom.SelectedDate = null;
            dpBDTo.SelectedDate = null;
            dpJDFrom.SelectedDate = null;
            dpJDTo.SelectedDate = null;
            RefreshDataGrid();
        }
    }
}