using MotoStore.Database;
using System;
using System.Windows;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using MotoStore.Views.Pages.LoginPages;
using System.Collections.ObjectModel;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class MotoListPage
    {

        internal ObservableCollection<MatHang> TableData;

        public MotoListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.MatHangs);
            foreach (var MatHang in TableData.ToList())
                if (MatHang.DaXoa)
                    TableData.Remove(MatHang);
            grdMoto.ItemsSource = TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdMoto.ItemsSource select grdMoto.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                    foreach (var obj in grdMoto.Items)
                    {
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => pi.GetValue(obj) as string).All(value => string.IsNullOrEmpty(value)))
                            continue;
                        if (obj is not MatHang mh)
                            continue;
                        // Kiểm tra dữ liệu null
                        if (string.IsNullOrWhiteSpace(mh.TenMh))
                            throw new("Tên mặt hàng không được để trống!");
                        if (string.IsNullOrEmpty(mh.MaNcc))
                            throw new("Mã nhà cung cấp không được để trống!");
                        // Thêm mới
                        if (string.IsNullOrEmpty(mh.MaMh))
                            cmd.CommandText += $"\nInsert into MatHang values(@TenMH{loopcount}, @SoPhanKhoi{loopcount}, @Mau{loopcount}, @GiaNhapMH{loopcount}, @GiaBanMH{loopcount}, @TonKho{loopcount}, @MaNCC{loopcount}, @HangSX{loopcount}, @XuatXu{loopcount}, @MoTa{loopcount}, 0)";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate MatHang Set TenMh = @TenMH{loopcount}, SoPhanKhoi = @SoPhanKhoi{loopcount}, Mau = @Mau{loopcount}, GiaNhapMh = @GiaNhapMH{loopcount}, GiaBanMh = @GiaBanMH{loopcount}, SoLuongTonKho = @TonKho{loopcount}, MaNCC = @MaNCC{loopcount}, HangSx = @HangSX{loopcount}, XuatXu = @XuatXu{loopcount}, MoTa = @MoTa{loopcount} Where MaMh = '{mh.MaMh}';";

                        cmd.Parameters.Add($"@TenMH{loopcount}", SqlDbType.NVarChar).Value = mh.TenMh;
                        cmd.Parameters.Add($"@SoPhanKhoi{loopcount}", SqlDbType.Int).Value = mh.SoPhanKhoi;
                        cmd.Parameters.Add($"@Mau{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(mh.Mau) ? DBNull.Value : mh.Mau;
                        cmd.Parameters.Add($"@GiaNhapMH{loopcount}", SqlDbType.Decimal).Value = mh.GiaNhapMh.HasValue ? mh.GiaNhapMh.Value : DBNull.Value;
                        cmd.Parameters.Add($"@GiaBanMH{loopcount}", SqlDbType.Decimal).Value = mh.GiaBanMh.HasValue ? mh.GiaBanMh.Value : DBNull.Value;
                        cmd.Parameters.Add($"@TonKho{loopcount}", SqlDbType.Int).Value = mh.SoLuongTonKho;
                        cmd.Parameters.Add($"@MaNCC{loopcount}", SqlDbType.NVarChar).Value = mh.MaNcc;
                        cmd.Parameters.Add($"@HangSX{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(mh.HangSx) ? DBNull.Value : mh.HangSx;
                        cmd.Parameters.Add($"@XuatXu{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(mh.XuatXu) ? DBNull.Value : mh.XuatXu;
                        cmd.Parameters.Add($"@MoTa{loopcount}", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(mh.MoTa) ? DBNull.Value : mh.MoTa;
                        loopcount++;
                    }
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database hóa đơn')", con);
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
                if (MessageBox.Show("Bạn có chắc muốn xóa? Hành động này không thể hoàn tác!", "Xóa mặt hàng", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                                if (obj is not MatHang mh)
                                    continue;
                                // Trường hợp chưa thêm mới nên chưa có mã mh
                                if (string.IsNullOrEmpty(mh.MaMh))
                                    continue;
                                // Xóa hàng
                                else
                                    cmd.CommandText += $"Update MatHang Set DaXoa = 1 Where MaMh = '{mh.MaMh}';\n";
                            }
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                            cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'chỉnh sửa database hóa đơn')", con);
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
        {
            RefreshDataGrid();
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                bool isQuanLy = string.Equals(PageChinh.getNV.ChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);

                grdMoto.IsReadOnly = !isQuanLy;

                if (sender is Button button)
                    button.Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                RefreshDataGrid();
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new());

        private void grdMoto_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
                    p.Workbook.Properties.Title = "Danh sách mặt hàng";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Product");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Product"];

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Danh sách mặt hàng từ MotoStore
                    ws.Cells[1, 1].Value = "Danh sách mặt hàng từ MotoStore";
                    ws.Cells[1, 1, 1, grdMoto.Columns.Count].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, grdMoto.Columns.Count].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, grdMoto.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in grdMoto.Columns)
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

                    // lấy ra danh sách MH từ TableData

                    ObservableCollection<MatHang> motoList = new(TableData);

                    // với mỗi mh trong danh sách sẽ ghi trên 1 dòng
                    foreach (var mh in motoList.Where(mh => grdMoto.Items.PassesFilter(mh)))
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = mh.MaMh;
                        ws.Cells[rowIndex, colIndex++].Value = mh.TenMh;
                        ws.Cells[rowIndex, colIndex++].Value = mh.SoPhanKhoi;
                        ws.Cells[rowIndex, colIndex++].Value = mh.Mau;
                        ws.Cells[rowIndex, colIndex++].Value = mh.GiaNhapMh;
                        ws.Cells[rowIndex, colIndex++].Value = mh.GiaBanMh;
                        ws.Cells[rowIndex, colIndex++].Value = mh.SoLuongTonKho;
                        ws.Cells[rowIndex, colIndex++].Value = mh.MaNcc;
                        ws.Cells[rowIndex, colIndex++].Value = mh.HangSx;
                        ws.Cells[rowIndex, colIndex++].Value = mh.XuatXu;
                        ws.Cells[rowIndex, colIndex++].Value = mh.MoTa;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    //Lưu file lại
                    byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(dialog.FileName, bin);
                }
                using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
                con.Open();
                SqlCommand cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(newid(), '{PageChinh.getNV.MaNv}', '{DateTime.Now:dd-MM-yyyy HH:mm:ss}', N'xuất excel mặt hàng')", con);
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