﻿using MotoStore.Database;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using MotoStore.Properties;
using MotoStore.Views.Pages.LoginPages;

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
            mainDataGrid.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in mainDataGrid.ItemsSource select mainDataGrid.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể lưu!");
                return;
            }
            MainDatabase mdb = new();
            SqlCommand cmd;
            using SqlConnection con = new(Settings.Default.ConnectionString);
            try
            {
                con.Open();
                using var trans = con.BeginTransaction();
                try
                {
                    int loopcount = 1;
                    cmd = new("set dateformat dmy", con, trans);

                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in mainDataGrid.Items)
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
                            cmd.CommandText += $"\nUpdate UserApp Set Username = @Username{loopcount}, Password = @Password{loopcount}, Email = @Email{loopcount} Where MaNv = @MaNV{loopcount};";

                        // Thêm mới
                        else
                            cmd.CommandText += $"\nInsert into UserApp values(@MaNV{loopcount}, @Username{loopcount}, @Password{loopcount}, @Email{loopcount})";

                        cmd.Parameters.Add($"@MaNV{loopcount}", SqlDbType.VarChar).Value = user.MaNv;
                        cmd.Parameters.Add($"@Username{loopcount}", SqlDbType.NVarChar).Value = user.UserName;
                        cmd.Parameters.Add($"@Password{loopcount}", SqlDbType.NVarChar).Value = user.Password;
                        cmd.Parameters.Add($"@Email{loopcount}", SqlDbType.NVarChar).Value = user.Email;
                        loopcount++;

                    }
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database tài khoản')", con);
                    cmd.ExecuteNonQuery();
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
                if (MessageBox.Show("Bạn có chắc muốn xóa? Hành động này không thể hoàn tác!", "Xóa tài khoản", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Nếu đáp ứng đủ điều kiện sẽ bắt đầu vòng lặp để xóa
                    MainDatabase mdb = new();
                    SqlCommand cmd;
                    using SqlConnection con = new(Settings.Default.ConnectionString);
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
                            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database tài khoản')", con);
                            cmd.ExecuteNonQuery();
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
                if (!mdb.NhanViens.Any(nv => !nv.DaXoa && string.Equals(nv.MaNv, tbxMaNv.Text)))
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
        private void mainDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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

        private void Export(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // tạo SaveFileDialog để lưu file excel
            CommonSaveFileDialog dialog = new();
            dialog.Filters.Add(new("Excel", "xlsx"));
            dialog.Filters.Add(new("Excel 2003", "*xls"));

            // Nếu mở file và chọn nơi lưu file không thành công sẽ kết thúc hàm
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            try
            {
                using (ExcelPackage p = new())
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "MotoStore App";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "Danh sách tài khoản";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("User");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["User"];

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Danh sách tài khoản từ MotoStore
                    ws.Cells[1, 1].Value = "Danh sách tài khoản từ MotoStore";
                    ws.Cells[1, 1, 1, mainDataGrid.Columns.Count].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, mainDataGrid.Columns.Count].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, mainDataGrid.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in mainDataGrid.Columns)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];

                        //set màu thành light blue
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        //gán giá trị
                        cell.Value = item.Header;

                        if (item.Header.ToString().Contains("Ngày", StringComparison.OrdinalIgnoreCase))
                            // Format cho ngày
                            ws.Column(colIndex).Style.Numberformat.Format = "dd/MM/yyyy";

                        colIndex++;
                    }

                    // lấy ra danh sách TK từ TableData

                    ObservableCollection<UserApp> UserList = new(TableData);

                    // với mỗi user trong danh sách sẽ ghi trên 1 dòng
                    foreach (var user in UserList.Where(user => mainDataGrid.Items.PassesFilter(user)))
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = user.MaNv;
                        ws.Cells[rowIndex, colIndex++].Value = user.UserName;
                        ws.Cells[rowIndex, colIndex++].Value = user.Password;
                        ws.Cells[rowIndex, colIndex++].Value = user.Email;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    //Lưu file lại
                    byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(dialog.FileName, bin);
                }
                using SqlConnection con = new(Settings.Default.ConnectionString);
                con.Open();
                SqlCommand cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'xuất excel tài khoản')", con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Xuất excel thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}