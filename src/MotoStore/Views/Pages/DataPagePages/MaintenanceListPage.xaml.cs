﻿using MotoStore.Database;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using MotoStore.Views.Pages.LoginPages;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for MaintenanceListPage.xaml
    /// </summary>
    public partial class MaintenanceListPage
    {
        internal ObservableCollection<ThongTinBaoHanh> TableData;

        public MaintenanceListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.ThongTinBaoHanhs);
            mainDataGrid.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in mainDataGrid.ItemsSource select mainDataGrid.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể lưu!");
                return;
            }
            SqlCommand cmd;
            using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
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
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => pi.GetValue(obj) as string).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not ThongTinBaoHanh bh)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(bh.MaHd))
                            throw new("Mã hóa đơn không được để trống!");

                        // Thêm mới
                        if (string.IsNullOrEmpty(bh.MaBh))
                            cmd.CommandText += $"\nInsert into ThongTinBaoHanh values(@MaHD{loopcount}, @ThoiGian{loopcount}, @GhiChu{loopcount})";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate ThongTinBaoHanh Set MaHd = @MaHD{loopcount}, ThoiGian = @ThoiGian{loopcount}, GhiChu = @GhiChu{loopcount} Where MaBh = '{bh.MaBh}';";

                        cmd.Parameters.Add($"@MaHD{loopcount}", SqlDbType.VarChar).Value = bh.MaHd;
                        cmd.Parameters.Add($"@ThoiGian{loopcount}", SqlDbType.SmallDateTime).Value = bh.ThoiGian.HasValue ? bh.ThoiGian.Value : DBNull.Value;
                        cmd.Parameters.Add($"@GhiChu{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(bh.GhiChu) ? DBNull.Value : bh.GhiChu;
                        loopcount++;
                    }
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database bảo hành')", con);
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
                if (MessageBox.Show("Bạn có chắc muốn xóa? Hành động này không thể hoàn tác!", "Xóa phiếu bảo hành", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Nếu đáp ứng đủ điều kiện sẽ bắt đầu vòng lặp để xóa
                    SqlCommand cmd;
                    using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
                    try
                    {
                        con.Open();
                        using var trans = con.BeginTransaction();
                        try
                        {
                            cmd = new(" ", con, trans);

                            foreach (var obj in dg.SelectedItems)
                            {
                                if (obj is not ThongTinBaoHanh bh)
                                    continue;
                                // Trường hợp chưa thêm mới nên chưa có mã bh
                                if (string.IsNullOrEmpty(bh.MaBh))
                                    continue;
                                // Xóa hàng
                                else
                                    cmd.CommandText += $"Delete From ThongTinBaoHanh Where MaBh = '{bh.MaBh}';\n";
                            }
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database bảo hành')", con);
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
            {
                bool isQuanLy = string.Equals(PageChinh.getNV.ChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);
                mainDataGrid.IsReadOnly = !isQuanLy;

                if (sender is Button button)
                    button.Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                RefreshDataGrid();
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

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
            dialog.Filters.Add(new("Excel 2003", "xls"));

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
                    p.Workbook.Properties.Title = "Danh sách phiếu bảo hành";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Maintenance");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Maintenance"];

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Danh sách phiếu bảo hành từ MotoStore
                    ws.Cells[1, 1].Value = "Danh sách phiếu bảo hành từ MotoStore";
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

                        if (string.Equals(item.Header.ToString(), "Thời gian", StringComparison.OrdinalIgnoreCase))
                            // Format cho ngày
                            ws.Column(colIndex).Style.Numberformat.Format = "dd/MM/yyyy";

                        colIndex++;
                    }

                    // lấy ra danh sách KH từ TableData

                    ObservableCollection<ThongTinBaoHanh> MaintenanceList = new(TableData);

                    // với mỗi kh trong danh sách sẽ ghi trên 1 dòng
                    foreach (var kh in MaintenanceList.Where(kh => mainDataGrid.Items.PassesFilter(kh)))
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = kh.MaBh;
                        ws.Cells[rowIndex, colIndex++].Value = kh.MaHd;
                        ws.Cells[rowIndex, colIndex++].Value = kh.ThoiGian;
                        ws.Cells[rowIndex, colIndex++].Value = kh.GhiChu;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    //Lưu file lại
                    byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(dialog.FileName, bin);
                }
                using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
                con.Open();
                SqlCommand cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'xuất excel bảo hành')", con);
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
