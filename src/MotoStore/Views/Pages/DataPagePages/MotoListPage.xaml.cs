using MotoStore.Database;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Linq;
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
    public partial class MotoListPage : INavigableView<ViewModels.MotoListViewModel>
    {
        internal List<MatHang> TableData = new();

        public ViewModels.MotoListViewModel ViewModel
        {
            get;
        }

        public MotoListPage(ViewModels.MotoListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = con.MatHangs.ToList();
            foreach (var MatHang in TableData.ToList())
                if (MatHang.DaXoa)
                    TableData.Remove(MatHang);
            grdMoto.ItemsSource = TableData;
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

                // Lý do cứ mỗi lần có cell sai là break:
                // - Tránh trường hợp hiện MessageBox liên tục
                // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                foreach (object obj in grdMoto.Items)
                {
                    // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                    // is not MatHang chỉ để an toàn
                    if (obj is null || obj is not MatHang mh)
                        continue;

                    // Thêm mới
                    if (string.IsNullOrEmpty(mh.MaMh))
                    {
                        cmd = new("Insert into MatHang values('" + mh.TenMh + "', '" + mh.SoPhanKhoi + "', '" + mh.GiaNhapMh + "', '" + mh.GiaBanMh + "', '" + mh.SoLuongTonKho + "', '" + mh.MaNcc + "', N'" + mh.HangSx + "', N'" + mh.XuatXu + "', N'" + mh.MoTa + "0)", con);
                        cmd.ExecuteNonQuery();
                    }

                    // Cập nhật
                    else
                    {
                        cmd = new("Update MatHang Set TenMh = '" + mh.TenMh + "', SoPhanKhoi = '" + mh.SoPhanKhoi + "', GiaNhapMh = '" + mh.GiaNhapMh + "', GiaBanMh = '" + mh.GiaBanMh + "', SoLuongTonKho = '" + mh.SoLuongTonKho + "', MaNCC = '" + mh.MaNcc + "', HangSx = N'" + mh.HangSx + "', XuatXu = N'" + mh.XuatXu + "', MoTa = N'" + mh.MoTa + "', DaXoa = 0 Where MaMh = '" + mh.MaMh + "';", con);
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
            DataGridRow dgr = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex);
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
                MatHang mh;

                foreach (object obj in grdMoto.SelectedItems)
                {
                    // Bỏ qua ô trắng mà vẫn được Select
                    // is not MatHang chỉ để an toàn
                    if (obj is null || obj is not MatHang)
                        continue;
                    mh = obj as MatHang;
                    if (mh is null)
                        continue;
                    // Trường hợp chưa thêm mới nên chưa có mã mh
                    if (string.IsNullOrEmpty(mh.MaMh))
                        // Vẫn chạy hàm xóa trên phần hiển thị thay vì refresh
                        // Lý do: nếu refresh hiển thị cho khớp với database thì sẽ mất những chỉnh sửa
                        // của người dùng trên datagrid trước khi nhấn phím delete do chưa được lưu.
                        // !! Chưa tìm ra hướng xử lý
                        continue;
                    // Xóa hàng
                    else
                    {
                        cmd = new("Update MatHang Set DaXoa = 1 Where MaMh = '" + mh.MaMh + "';", con);
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
                    btnSave.Visibility = Visibility.Visible;
                    grdMoto.IsReadOnly = false;
                }
                else
                {
                    btnSave.Visibility = Visibility.Collapsed;
                    grdMoto.IsReadOnly = true;
                }
            }
        }
    }
}