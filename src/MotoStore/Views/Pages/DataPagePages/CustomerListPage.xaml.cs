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
            DateTime? bdfrom = null;
            DateTime? bdto = null;
            MainDatabase con = new();
            TableData = new(con.KhachHangs);
            foreach (var khachHang in TableData.ToList())
            {
                if (khachHang.DaXoa)
                {
                    TableData.Remove(khachHang);
                    continue;
                }
                if (bdfrom is null || bdfrom > khachHang.NgSinh)
                    bdfrom = khachHang.NgSinh;
                if (bdto is null || bdto < khachHang.NgSinh)
                    bdto = khachHang.NgSinh;
            }
            dpBDFrom.SelectedDate ??= bdfrom;
            dpBDTo.SelectedDate ??= bdto;
            // Filter
            foreach (var khachHang in TableData.ToList())
                if (khachHang.NgSinh < dpBDFrom.SelectedDate || khachHang.NgSinh > dpBDTo.SelectedDate)
                    TableData.Remove(khachHang);
            grdCustomer.ItemsSource = TableData;
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
                string ngaySinhKh;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdCustomer.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    if (obj is not KhachHang kh)
                        continue;
                    // Lấy chuỗi ngày sinh theo format dd-MM-yyyy
                    if (kh.NgSinh.HasValue)
                        ngaySinhKh = kh.NgSinh.Value.ToString("dd-MM-yyyy");
                    else
                        ngaySinhKh = string.Empty;
                    // Kiểm tra dữ liệu đã đúng theo định nghĩa chưa
                    if (string.IsNullOrEmpty(kh.GioiTinh) || (kh.GioiTinh != "Nam" && kh.GioiTinh != "Nữ"))
                    {
                        MessageBox.Show("Giới tính chỉ có thể là Nam hoặc Nữ (có dấu)!");
                        return;
                    }
                    if (!string.IsNullOrEmpty(kh.Sdt) && (kh.Sdt.Contains('+') || kh.Sdt.Contains('-')) && int.TryParse(kh.Sdt, out _))
                    {
                        MessageBox.Show("Số điện thoại không được chứa ký tự không phải chữ số!");
                        return;
                    }
                    if (string.IsNullOrEmpty(kh.LoaiKh) || (kh.LoaiKh != "Vip" && kh.LoaiKh != "Thường" && kh.LoaiKh != "Thân quen"))
                    {
                        MessageBox.Show("Loại khách hàng phải là Vip, Thường hoặc Thân quen (có dấu)!");
                        return;
                    }

                    // Thêm mới
                    if (string.IsNullOrEmpty(kh.MaKh))
                    {
                        cmd = new("Insert into KhachHang values(N'" + kh.HoTenKh + "', '" + ngaySinhKh + "', N'" + kh.GioiTinh + "', N'" + kh.DiaChi + "', '" + kh.Sdt + "', '" + kh.Email + "', N'" + kh.LoaiKh + "', 0)", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new("Update KhachHang Set HotenKh = N'" + kh.HoTenKh + "', NgSinh = '" + ngaySinhKh + "', GioiTinh = N'" + kh.GioiTinh + "', DiaChi = N'" + kh.DiaChi + "', Sdt = '" + kh.Sdt + "', Email = '" + kh.Email + "', LoaiKh = N'" + kh.LoaiKh + "', DaXoa = 0 Where MaKh = '" + kh.MaKh + "';", con);
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
            if (grdCustomer.IsReadOnly)
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

                foreach (object obj in grdCustomer.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not KhachHang chỉ để an toàn
                    if (obj is not KhachHang kh)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã KH
                    if (string.IsNullOrEmpty(kh.MaKh))
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new("Update KhachHang Set DaXoa = 1 Where MaKh = '" + kh.MaKh + "'", con);
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

                grdCustomer.IsReadOnly = !isQuanLy;

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
            RefreshDataGrid();
        }
    }
}