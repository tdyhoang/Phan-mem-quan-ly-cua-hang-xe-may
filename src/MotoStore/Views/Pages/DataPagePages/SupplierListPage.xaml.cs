using MotoStore.Database;
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
using MotoStore.Properties;
using System.Printing;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class SupplierListPage
    {
        internal ObservableCollection<NhaCungCap> TableData;

        public SupplierListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.NhaCungCaps);
            foreach (var ncc in TableData.ToList())
                if (ncc.DaXoa)
                    TableData.Remove(ncc);
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
                        // Trường hợp gặp dòng trắng được người dùng thêm mà chưa chỉnh sửa
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => pi.GetValue(obj) as string).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not NhaCungCap ncc)
                            continue;
                        // Kiểm tra dữ liệu null
                        if (string.IsNullOrWhiteSpace(ncc.TenNcc))
                            throw new("Tên nhà cung cấp không được để trống!");

                        // Thêm mới
                        if (string.IsNullOrEmpty(ncc.MaNcc))
                            cmd.CommandText += $"\nInsert into NhaCungCap values(@TenNCC{loopcount}, @SDT{loopcount}, @Email{loopcount}, @DiaChi{loopcount}, 0)";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate NhaCungCap Set TenNcc = @TenNCC{loopcount}, DiaChi = @DiaChi{loopcount}, Sdt = @SDT{loopcount}, Email = @Email{loopcount} Where MaNcc = '{ncc.MaNcc}';";

                        cmd.Parameters.Add($"@TenNCC{loopcount}", SqlDbType.NVarChar).Value = ncc.TenNcc;
                        cmd.Parameters.Add($"@DiaChi{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ncc.DiaChi) ? DBNull.Value : ncc.DiaChi;
                        cmd.Parameters.Add($"@SDT{loopcount}", SqlDbType.VarChar).Value = string.IsNullOrEmpty(ncc.Sdt) ? DBNull.Value : ncc.Sdt;
                        cmd.Parameters.Add($"@Email{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(ncc.Email) ? DBNull.Value : ncc.Email;
                        loopcount++;
                    }
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database nhà cung cấp')", con);
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
                if (MessageBox.Show("Bạn có chắc muốn xóa? Hành động này không thể hoàn tác!", "Xóa nhà cung cấp", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Nếu đáp ứng đủ điều kiện sẽ bắt đầu vòng lặp để xóa
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
                                if (obj is not NhaCungCap ncc)
                                    continue;
                                // Trường hợp chưa thêm mới nên chưa có mã KH
                                if (string.IsNullOrEmpty(ncc.MaNcc))
                                    continue;
                                // Xóa hàng
                                else
                                    cmd.CommandText += $"Update NhaCungCap Set DaXoa = 1 Where MaNcc = '{ncc.MaNcc}';\n";
                            }
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database nhà cung cấp')", con);
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
                    p.Workbook.Properties.Title = "Danh sách nhà cung cấp";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Supplier");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Supplier"];

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Danh sách nhà cung cấp từ MotoStore
                    ws.Cells[1, 1].Value = "Danh sách nhà cung cấp từ MotoStore";
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

                    // lấy ra danh sách NCC từ TableData

                    ObservableCollection<NhaCungCap> SupplierList = new(TableData);

                    // với mỗi ncc trong danh sách sẽ ghi trên 1 dòng
                    foreach (var ncc in SupplierList.Where(ncc => mainDataGrid.Items.PassesFilter(ncc)))
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = ncc.MaNcc;
                        ws.Cells[rowIndex, colIndex++].Value = ncc.TenNcc;
                        ws.Cells[rowIndex, colIndex++].Value = ncc.Sdt;
                        ws.Cells[rowIndex, colIndex++].Value = ncc.Email;
                        ws.Cells[rowIndex, colIndex++].Value = ncc.DiaChi;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    //Lưu file lại
                    byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(dialog.FileName, bin);
                }
                using SqlConnection con = new(Settings.Default.ConnectionString);
                con.Open();
                SqlCommand cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'xuất excel nhà cung cấp')", con);
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