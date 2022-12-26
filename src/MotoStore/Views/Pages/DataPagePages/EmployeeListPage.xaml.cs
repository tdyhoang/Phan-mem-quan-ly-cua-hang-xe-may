using MotoStore.Database;
using MotoStore.Models;
using System.Collections.Generic;
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

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class EmployeeListPage : INavigableView<ViewModels.EmployeeListViewModel>
    {
        internal List<NhanVien> TableData = new();

        public ViewModels.EmployeeListViewModel ViewModel
        {
            get;
        }

        public EmployeeListPage(ViewModels.EmployeeListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            TableData.Clear();
            MainDatabase con = new();
            foreach (var nhanVien in con.NhanViens.ToList())
                if (!nhanVien.DaXoa)
                    TableData.Add(nhanVien);
            grdEmployee.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            try
            {
                MainDatabase mainDatabase = new MainDatabase();
                SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                SqlCommand cmd;
                con.Open();
                cmd = new SqlCommand("set dateformat dmy", con);
                cmd.ExecuteNonQuery();
                NhanVien nv;
                string ngaySinhNv, ngayVaoLam;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdEmployee.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    // is not NhanVien chỉ để an toàn
                    if (obj is null || obj is not NhanVien)
                        continue;
                    nv = obj as NhanVien;
                    if (nv is null)
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
                    if (nv.Luong is null)
                        nv.Luong = 0;

                    // Thêm mới
                    if (nv.MaNv.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        cmd = new SqlCommand("Insert into NhanVien values(newid(), N'" + nv.HoTenNv + "', '" + ngaySinhNv + "', N'" + nv.GioiTinh + "', N'" + nv.DiaChi + "', '" + nv.Sdt + "', '" + nv.Email + "', N'" + nv.ChucVu + "', '" + ngayVaoLam + "', " + nv.Luong + "0)", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new SqlCommand("Update NhanVien Set HoTenNv = N'" + nv.HoTenNv + "', NgSinh = '" + ngaySinhNv + "', GioiTinh = N'" + nv.GioiTinh + "', DiaChi = N'" + nv.DiaChi + "', Sdt = '" + nv.Sdt + "', Email = '" + nv.Email + "', ChucVu = N'" + nv.ChucVu + "', ngVL = '" + ngayVaoLam + "', Luong = " + nv.Luong + ", DaXoa = 0 Where Manv = '" + nv.MaNv.ToString() + "';", con);
                        cmd.ExecuteNonQuery();
                    }
                }

                con.Close();
                // Làm mới nội dung hiển thị cho khớp với database
                RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyMaNV(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(((NhanVien)grdEmployee.SelectedItems[grdEmployee.SelectedItems.Count - 1]).MaNv.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Định nghĩa lại phím tắt Delete
        private new void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg is null)
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
            try
            {
                MainDatabase mainDatabase = new MainDatabase();
                SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                SqlCommand cmd;
                con.Open();
                NhanVien nv;

                foreach (object obj in grdEmployee.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not NhanVien chỉ để an toàn
                    if (obj is null || obj is not NhanVien)
                        continue;
                    nv = obj as NhanVien;
                    if (nv is null)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã nv
                    if (nv.MaNv.ToString() == "00000000-0000-0000-0000-000000000000")
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new SqlCommand("Update NhanVien Set DaXoa = 1 Where Manv = '" + nv.MaNv.ToString() + "';", con);
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
    }
}