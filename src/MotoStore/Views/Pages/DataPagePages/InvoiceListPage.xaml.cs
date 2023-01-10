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
    public partial class InvoiceListPage : INavigableView<ViewModels.InvoiceListViewModel>
    {
        public ViewModels.InvoiceListViewModel ViewModel
        {
            get;
        }

        internal ObservableCollection<HoaDon> TableData = new();

        bool isvalidtbxQntFrom = false;
        bool isvalidtbxQntTo = false;
        bool isvalidtbxTotFrom = false;
        bool isvalidtbxTotTo = false;

        public InvoiceListPage(ViewModels.InvoiceListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            DateTime? cdfrom = null;
            DateTime? cdto = null;
            MainDatabase con = new();
            TableData = new(con.HoaDons);
            foreach (var hoaDon in TableData.ToList())
            {
                if (cdfrom is null || cdfrom > hoaDon.NgayLapHd)
                    cdfrom = hoaDon.NgayLapHd;
                if (cdto is null || cdto < hoaDon.NgayLapHd)
                    cdto = hoaDon.NgayLapHd;
            }
            dpCDFrom.SelectedDate ??= cdfrom;
            dpCDTo.SelectedDate ??= cdto;
            // Filter
            foreach (var nhanVien in TableData.ToList())
            {
                if (nhanVien.NgayLapHd < dpCDFrom.SelectedDate || nhanVien.NgayLapHd > dpCDTo.SelectedDate)
                    TableData.Remove(nhanVien);
                else if ((isvalidtbxQntFrom && nhanVien.SoLuong < decimal.Parse(tbxQntFrom.Text)) || (isvalidtbxQntTo && nhanVien.SoLuong > decimal.Parse(tbxQntTo.Text)))
                    TableData.Remove(nhanVien);
                else if ((isvalidtbxTotFrom && nhanVien.ThanhTien < decimal.Parse(tbxTotFrom.Text)) || (isvalidtbxTotTo && nhanVien.ThanhTien > decimal.Parse(tbxTotTo.Text)))
                    TableData.Remove(nhanVien);
            }
            grdInvoice.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            try
            {
                MainDatabase mainDatabase = new();
                SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                SqlCommand cmd;
                con.Open();
                cmd = new("set dateformat dmy", con);
                cmd.ExecuteNonQuery();
                string ngayLapHD;

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdInvoice.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    if (obj is not HoaDon hd)
                        continue;

                    // Lấy chuỗi ngày lập hd theo format dd-MM-yyyy
                    if (hd.NgayLapHd.HasValue)
                        ngayLapHD = hd.NgayLapHd.Value.ToString("dd-MM-yyyy");
                    else
                        ngayLapHD = string.Empty;
                    // Kiểm tra dữ liệu đã đúng theo định nghĩa chưa
                    hd.SoLuong ??= 0;
                    hd.ThanhTien ??= 0;

                    // Thêm mới
                    if (string.IsNullOrEmpty(hd.MaHd))
                    {
                        cmd = new("Insert into HoaDon values('" + hd.MaMh + "', '" + hd.MaKh + "', '" + hd.MaNv + "', '" + ngayLapHD + "', " + hd.SoLuong + ", " + hd.ThanhTien + ")", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new("Update HoaDon Set MaMh = '" + hd.MaMh + "', MaKh = '" + hd.MaKh + "', MaNv = '" + hd.MaNv + "', NgayLapHd = '" + ngayLapHD + "', SoLuong = " + hd.SoLuong + ", ThanhTien = " + hd.ThanhTien + " Where MaHd = '" + hd.MaHd + "';", con);
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

        // Định nghĩa lại phím tắt Delete
        private new void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not DataGrid dg)
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
                MainDatabase mainDatabase = new();
                SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
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
                    if (string.IsNullOrEmpty(hd.MaHd))
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
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
                MessageBox.Show(ex.Message);
                // Báo đã thực hiện xong event để ngăn handler mặc định cho phím này hoạt động
                e.Handled = true;
            }
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (PageChinh.getChucVu.ToLower() == "quản lý")
                {
                    grdInvoice.IsReadOnly = false;
                }
                else
                {
                    grdInvoice.IsReadOnly = true;
                }
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

        private void ClearFilter(object sender, RoutedEventArgs e)
        {

        }
    }
}