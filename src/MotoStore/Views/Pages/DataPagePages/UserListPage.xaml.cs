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
using System.Security.Cryptography;
using Microsoft.Xaml.Behaviors.Layout;
using System.Text.RegularExpressions;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class UserListPage
    {
        internal ObservableCollection<UserApp> TableData;

        public UserListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.UserApps);
            grdUser.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdUser.ItemsSource select grdUser.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể lưu!");
                return;
            }
            MainDatabase mdb = new();
            SqlCommand cmd;
            using SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            try
            {
                con.Open();
                using var trans = con.BeginTransaction();
                try
                {
                    cmd = new("set dateformat dmy", con, trans);

                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in grdUser.Items)
                    {
                        if (obj is not UserApp user)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrWhiteSpace(user.UserName))
                            throw new("Username không được để trống!");
                        if (string.IsNullOrEmpty(user.Password))
                            throw new("Password không được để trống!");
                        if (string.IsNullOrEmpty(user.Email))
                            throw new("Email không được để trống!");
                        // Kiểm tra username có bị trùng không
                        foreach (var existedUser in mdb.UserApps.ToList())
                            if (string.Equals(existedUser.UserName, user.UserName, StringComparison.OrdinalIgnoreCase) && !string.Equals(existedUser.MaNv, user.MaNv))
                                throw new("Username đã tồn tại!");

                        // Cập nhật
                        if (mdb.UserApps.Any(nv => nv.MaNv == user.MaNv))
                            cmd.CommandText += $"\nUpdate UserApp Set Username = N'{user.UserName}', Password = N'{user.Password}', Email = N'{user.Email}' Where MaNv = '{user.MaNv}';";

                        // Thêm mới
                        else
                            cmd.CommandText += $"\nInsert into UserApp values(N'{user.MaNv}', N'{user.UserName}', N'{user.Password}', N'{user.Email}')";
                        
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
                MainDatabase mdb = new();
                SqlCommand cmd;
                using SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
                try
                {
                    con.Open();
                    using var trans = con.BeginTransaction();
                    try
                    {
                        cmd = new(" ", con, trans);

                        foreach (var obj in dg.SelectedItems)
                        {
                            if (obj is not UserApp user)
                                continue;
                            // Trường hợp chưa thêm mới nên chưa có mã NV
                            if (!mdb.UserApps.Any(nv => nv.MaNv == user.MaNv))
                                continue;
                            // Xóa hàng
                            else
                                cmd.CommandText += $"Delete From UserApp Where MaNv = '{user.MaNv}';\n";
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
                RefreshDataGrid();
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbxMaNv.Text))
                    throw new("Mã nhân viên không được để trống!");
                if (!Regex.IsMatch(tbxMaNv.Text, @"^NV\d{3}$"))
                    throw new("Mã nhân viên phải theo cú pháp NV***, trong đó * là các chữ số");
                MainDatabase mdb = new();
                if (!mdb.NhanViens.Any(nv => string.Equals(nv.MaNv, tbxMaNv.Text)))
                    throw new("Mã nhân viên không tồn tại hoặc đã xóa");
                if (TableData.Any(ua => string.Equals(ua.MaNv, tbxMaNv.Text)))
                    throw new("Nhân viên đã hoặc đang được tạo tài khoản!");
                TableData.Add(new() { MaNv = tbxMaNv.Text });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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