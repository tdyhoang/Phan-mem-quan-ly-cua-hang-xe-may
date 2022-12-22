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
    public partial class InvoiceListPage : INavigableView<ViewModels.InvoiceListViewModel>
    {
        public ViewModels.InvoiceListViewModel ViewModel
        {
            get;
        }

        public InvoiceListPage(ViewModels.InvoiceListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new MainDatabase();
            grdInvoice.ItemsSource = con.HoaDons.ToList();
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
                HoaDon hd;
                string ngayLapHD;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdInvoice.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    // is not HoaDon chỉ để an toàn
                    if (obj is null || obj is not HoaDon)
                        continue;
                    hd = obj as HoaDon;
                    if (hd is null)
                        continue;

                    // Lấy chuỗi ngày lập hd theo format dd-MM-yyyy
                    if (hd.NgayLapHd.HasValue)
                        ngayLapHD = hd.NgayLapHd.Value.ToString("dd-MM-yyyy");
                    else
                        ngayLapHD = string.Empty;
                    // Kiểm tra dữ liệu đã đúng theo định nghĩa chưa
                    if (hd.SoLuong is null)
                        hd.SoLuong = 0;
                    if (hd.ThanhTien is null)
                        hd.ThanhTien = 0;

                    // Thêm mới
                    if (hd.MaHd.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        cmd = new SqlCommand("Insert into HoaDon values(newid(), '" + hd.MaMh + "', '" + hd.MaKh + "', '" + hd.MaNv + "', '" + ngayLapHD + "', " + hd.SoLuong + ", " + hd.ThanhTien + ")", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new SqlCommand("Update HoaDon Set MaMh = '" + hd.MaMh + "', MaKh = '" + hd.MaKh + "', MaNv = '" + hd.MaNv + "', NgayLapHd = '" + ngayLapHD + "', SoLuong = " + hd.SoLuong + ", ThanhTien = " + hd.ThanhTien + " Where MaHd = '" + hd.MaHd.ToString() + "';", con);
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

        private void CopyMaHD(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(((HoaDon)grdInvoice.SelectedItems[grdInvoice.SelectedItems.Count - 1]).MaHd.ToString());
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
                HoaDon hd;

                foreach (object obj in grdInvoice.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not HoaDon chỉ để an toàn
                    if (obj is null || obj is not HoaDon)
                        continue;
                    hd = obj as HoaDon;
                    if (hd is null)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã hd
                    if (hd.MaHd.ToString() == "00000000-0000-0000-0000-000000000000")
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new SqlCommand("Delete From HoaDon Where MaHd = '" + hd.MaHd.ToString() + "';", con);
                        cmd.ExecuteNonQuery();
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        grdInvoice.Items.Remove(obj);
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