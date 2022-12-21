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
    public partial class CustomerListPage : INavigableView<ViewModels.CustomerListViewModel>
    {
        public ViewModels.CustomerListViewModel ViewModel
        {
            get;
        }

        public CustomerListPage(ViewModels.CustomerListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new MainDatabase();
            grdCustomer.ItemsSource = con.KhachHangs.ToList();
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
                KhachHang kh;
                string ngaySinhKh;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdCustomer.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    // is not KhachHang chỉ để an toàn
                    if (obj is null || obj is not KhachHang)
                        continue;
                    kh = obj as KhachHang;
                    if (kh is null)
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
                        break;
                    }
                    if (!string.IsNullOrEmpty(kh.Sdt) && (kh.Sdt.Contains('+') || kh.Sdt.Contains('-')) && int.TryParse(kh.Sdt, out _))
                    {
                        MessageBox.Show("Số điện thoại không được chứa ký tự không phải chữ số!");
                        break;
                    }
                    if (string.IsNullOrEmpty(kh.LoaiKh) || (kh.LoaiKh != "Vip" && kh.LoaiKh != "Thường" && kh.LoaiKh != "Thân quen"))
                    {
                        MessageBox.Show("Loại khách hàng phải là Vip, Thường hoặc Thân quen (có dấu)!");
                        break;
                    }

                    // Thêm mới
                    if (kh.MaKh.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        cmd = new SqlCommand("Insert into KhachHang values(newid(), N'" + kh.HoTenKh + "', '" + ngaySinhKh + "', N'" + kh.GioiTinh + "', N'" + kh.DiaChi + "', '" + kh.Sdt + "', '" + kh.Email + "', N'" + kh.LoaiKh + "')", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new SqlCommand("Update KhachHang Set HotenKh = N'" + kh.HoTenKh + "', NgSinh = '" + ngaySinhKh + "', GioiTinh = N'" + kh.GioiTinh + "', DiaChi = N'" + kh.DiaChi + "', Sdt = '" + kh.Sdt + "', Email = '" + kh.Email + "', LoaiKh = N'" + kh.LoaiKh + "' Where MaKh = '" + kh.MaKh.ToString() + "';", con);
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

        private void CopyMaKH(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(((KhachHang)grdCustomer.SelectedItems[grdCustomer.SelectedItems.Count - 1]).MaKh.ToString());
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
            if (dg == null)
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
                KhachHang kh;

                foreach (object obj in grdCustomer.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not KhachHang chỉ để an toàn
                    if (obj is null || obj is not KhachHang)
                        continue;
                    kh = obj as KhachHang;
                    if (kh is null)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã KH
                    if (kh.MaKh.ToString() == "00000000-0000-0000-0000-000000000000")
                        // Vẫn chạy handler mặc định để thay đổi hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new SqlCommand("Delete From KhachHang Where MaKh = '" + kh.MaKh.ToString() + "';", con);
                        cmd.ExecuteNonQuery();
                        // Vẫn chạy handler mặc định để thay đổi hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        grdCustomer.Items.Remove(obj);
                    }
                }
                con.Close();
                // Báo đã thực hiện xong event để ngăn handler mặc định cho phím này hoạt động
                e.Handled = true;
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