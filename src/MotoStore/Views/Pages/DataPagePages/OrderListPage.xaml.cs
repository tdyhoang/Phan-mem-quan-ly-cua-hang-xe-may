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
using Microsoft.Win32;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class OrderListPage
    {
        internal ObservableCollection<DonDatHang> TableData;
        bool isQuanLy = default;

        public OrderListPage()
        {
            InitializeComponent();
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new();
            TableData = new(con.DonDatHangs);
            if (isQuanLy)
                grdOrder.ItemsSource = TableData;
            else
            {
                foreach (var ddh in TableData.ToList())
                    if (ddh.MaNv != PageChinh.getMa)
                        TableData.Remove(ddh);
                grdOrder.ItemsSource = TableData;
            }
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            if ((from c in from object i in grdOrder.ItemsSource select grdOrder.ItemContainerGenerator.ContainerFromItem(i) where c != null select Validation.GetHasError(c)).FirstOrDefault(x => x))
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
                    cmd = new("set dateformat dmy", con, trans);
                    // Lý do cứ mỗi lần có cell sai là break:
                    // - Tránh trường hợp hiện MessageBox liên tục
                    // - Người dùng không thể nhớ hết các lỗi sai, mỗi lần chỉ hiện 1 lỗi sẽ dễ hơn với họ
                    foreach (var obj in grdOrder.Items)
                    {
                        // Trường hợp gặp dòng trắng dưới cùng của bảng (để người dùng có thể thêm dòng)
                        if (obj.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string)).Select(pi => (string)pi.GetValue(obj)).All(value => string.IsNullOrEmpty(value) || string.Equals(value, PageChinh.getMa) || string.Equals(value, DateTime.Today.ToString("dd/MM/yyyy"))))
                            continue;
                        if (obj is not DonDatHang ddh)
                            continue;
                        // Kiểm tra dữ liệu null & gán giá trị mặc định
                        if (string.IsNullOrEmpty(ddh.MaKh))
                            throw new("Mã khách hàng không được để trống!");
                        if (string.IsNullOrEmpty(ddh.MaMh))
                            throw new("Mã mặt hàng không được để trống!");
                        if (string.IsNullOrEmpty(ddh.MaNv))
                            throw new("Mã nhân viên không được để trống!");
                        string soLuongHang = ddh.SoLuongHang.HasValue ? $"'{ddh.SoLuongHang.Value}'" : "null";
                        string ngayDatHang = ddh.Ngdh.HasValue ? $"'{ddh.Ngdh.Value:dd/MM/yyyy}'" : "null";

                        // Thêm mới
                        if (string.IsNullOrEmpty(ddh.MaDdh))
                            cmd.CommandText += $"\nInsert into DonDatHang values('{ddh.MaMh}', '{soLuongHang}', '{ddh.MaKh}', {ddh.MaNv}, {ngayDatHang})";

                        // Cập nhật
                        else
                            cmd.CommandText += $"\nUpdate DonDatHang Set MaMh = '{ddh.MaMh}', MaKh = '{ddh.MaKh}', MaNv = '{ddh.MaNv}', Ngdh = {ngayDatHang}, SoLuongHang = {soLuongHang} Where MaDdh = '{ddh.MaDdh}';";
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
                            if (obj is not DonDatHang ddh)
                                continue;
                            // Trường hợp chưa thêm mới nên chưa có mã ddh
                            if (string.IsNullOrEmpty(ddh.MaDdh))
                                continue;
                            // Xóa hàng
                            else
                                cmd.CommandText += $"Delete From DonDatHang Where MaDDH = '{ddh.MaDdh}';\n";
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

        // Chỉ cho phép thao tác trên đơn hàng mình tự tạo nếu không phải quản lý
        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                isQuanLy = string.Equals(PageChinh.getChucVu, "Quản Lý", StringComparison.OrdinalIgnoreCase);
                grdOrder.Columns[3].Visibility = isQuanLy ? Visibility.Visible : Visibility.Collapsed;

                RefreshDataGrid();
            }
        }

        private void AddRow(object sender, RoutedEventArgs e)
            => TableData.Add(new() { MaNv = PageChinh.getMa, Ngdh = DateTime.Today });

        // Đẩy event mousewheel cho scrollviewer xử lý
        private void grdOrder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
            string filePath = string.Empty;
            // tạo SaveFileDialog để lưu file excel
            CommonSaveFileDialog dialog = new();
            dialog.Filters.Add(new("Excel", "*.xlsx"));
            dialog.Filters.Add(new("Excel 2003", "*.xls"));

            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                filePath = dialog.FileName;

            try
            {
                using (ExcelPackage p = new())
                {
                    // đặt tên người tạo file
                    p.Workbook.Properties.Author = "MotoStore App";

                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "Danh sách đơn đặt hàng";

                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Order");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Order"];

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge là Danh sách đơn đặt hàng từ MotoStore
                    ws.Cells[1, 1].Value = "Danh sách đơn đặt hàng từ MotoStore";
                    ws.Cells[1, 1, 1, grdOrder.Columns.Where(c => c.Visibility == Visibility.Visible).Count()].Merge = true;
                    // in đậm
                    ws.Cells[1, 1, 1, grdOrder.Columns.Where(c => c.Visibility == Visibility.Visible).Count()].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[1, 1, 1, grdOrder.Columns.Where(c => c.Visibility == Visibility.Visible).Count()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;

                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in grdOrder.Columns.Where(c => c.Visibility == Visibility.Visible))
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

                    // lấy ra danh sách KH từ TableData

                    ObservableCollection<DonDatHang> orderList = new(TableData);

                    // với mỗi ddh trong danh sách sẽ ghi trên 1 dòng
                    foreach (var ddh in orderList.Where(dh => grdOrder.Items.PassesFilter(dh)))
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = ddh.MaDdh;
                        ws.Cells[rowIndex, colIndex++].Value = ddh.MaMh;
                        ws.Cells[rowIndex, colIndex++].Value = ddh.MaKh;
                        if (isQuanLy)
                            ws.Cells[rowIndex, colIndex++].Value = ddh.MaNv;
                        ws.Cells[rowIndex, colIndex++].Value = ddh.SoLuongHang;
                        ws.Cells[rowIndex, colIndex++].Value = ddh.Ngdh;
                    }
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    //Lưu file lại
                    byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                MessageBox.Show("Xuất excel thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}