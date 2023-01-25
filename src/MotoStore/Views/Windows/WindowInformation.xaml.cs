using MotoStore.Database;
using System.Windows;
using System.Windows.Input;
using MotoStore.Views.Pages.IOPagePages;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using MotoStore.Views.Pages.LoginPages;
using System.IO;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using MotoStore.Helpers;
using MotoStore.Properties;

namespace MotoStore.Views.Windows
{
    /// <summary>
    /// Interaction logic for WindowInformation.xaml
    /// </summary>
    public partial class WindowInformation : Window
    {
        private readonly SqlConnection con = new(Properties.Settings.Default.ConnectionString);
        static internal Tuple<MatHang, BitmapImage?> mathang;

        private List<string> ListAnhSP = new();
        private IOSanPhamPage IOSPpg = new();
        private string destFile;
        private string newPathToFile;
        private string? OFDFileName = null;
        private int loaiWD = 0;  //Phân Biệt loại WD


        //private static int index = 0; //Dùng cho Trang Biểu Đồ
        List<string> listMaMH = new ();
        List<string> listTenMH = new();
        List<string> listMaHD = new();
        List<string> listMaKH = new();
        List<string> listTenKH = new();
        List<int> listSoLg = new();
        List<decimal> listThanhTien = new();
        List<string> listMaNV = new();
        List<string> listTenNV = new();
        static int SoSP = 0;
        static int index = 0; //Dùng để kiểm soát index của từng sản phẩm bán ra

        public WindowInformation(string ngay)
        {
            //WindowInfor của Trang Biểu Đồ
            InitializeComponent();
            btnCapNhatAnh.Visibility = Visibility.Collapsed;
            btnLuu.Visibility = Visibility.Collapsed;
            btnBack.Visibility = Visibility.Visible;
            btnNext.Visibility = Visibility.Visible;
            lblThongTin.Content = "Các Sản Phẩm Bán Ra Trong Ngày " + ngay;
            lblSoPhanKhoi.Content = "Mã Hoá Đơn:";  //MaHD
            lblGia.Content = "Mã Khách Mua:";
            txtGiaBan.IsReadOnly = true;
            txtMau.IsReadOnly = true;
            lblMau.Content = "Tên Khách Mua:";
            lblDB.Content = "Số Lượng:";
            lblTonKho.Content = "Thành Tiền:";
            txtTonKho.IsReadOnly = true;
            lblHangSanXuat.Content = "Nhân Viên Bán Hàng:";
            lblXX.Content = "Tên Nhân Viên:";
            MainDatabase mdb = new();
            SqlConnection con = new(Properties.Settings.Default.ConnectionString);

            con.Open();
            SqlCommand cmdSoSP = new("Select Count(*) from HoaDon where NgayLapHD = @Today", con);
            cmdSoSP.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
            cmdSoSP.Parameters["@Today"].Value = ngay;
            SqlDataReader sdaSoSP = cmdSoSP.ExecuteReader();
            if (sdaSoSP.Read())
                SoSP = (int)sdaSoSP[0]; //Lấy ra số SP bán ra trong Ngày này

            if (SoSP != 0)
            {
                SqlCommand cmdMaMH = new("Set dateformat dmy\nSelect MaMH from HoaDon where NgayLapHD = @Today", con);
                cmdMaMH.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdMaMH.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaMaMH = cmdMaMH.ExecuteReader();
                while (sdaMaMH.Read())
                {
                    if (sdaMaMH[0] != DBNull.Value)
                        listMaMH.Add((string)sdaMaMH[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblMaMH.Content = listMaMH[0];

                SqlCommand cmdTenMH = new("Set dateformat dmy\nSelect TenMH from HoaDon join MatHang on HoaDon.MaMH=MatHang.MaMH where NgayLapHD = @Today", con);
                cmdTenMH.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdTenMH.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaTenMH = cmdTenMH.ExecuteReader();
                while (sdaTenMH.Read())
                {
                    if (sdaTenMH[0] != DBNull.Value)
                        listTenMH.Add((string)sdaTenMH[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblTenMH.Content = listTenMH[0];

                SqlCommand cmdMaHD = new("Set dateformat dmy\nSelect MaHD from HoaDon where NgayLapHD = @Today", con);
                cmdMaHD.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdMaHD.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaMaHD = cmdMaHD.ExecuteReader();
                while (sdaMaHD.Read())
                {
                    if (sdaMaHD[0] != DBNull.Value)
                        listMaHD.Add((string)sdaMaHD[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblSoPK.Content = listMaHD[0];

                SqlCommand cmdMaKH = new("Set dateformat dmy\nSelect MaKH from HoaDon where NgayLapHD = @Today", con);
                cmdMaKH.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdMaKH.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaMaKH = cmdMaKH.ExecuteReader();
                while (sdaMaKH.Read())
                {
                    if (sdaMaKH[0] != DBNull.Value)
                        listMaKH.Add((string)sdaMaKH[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                txtGiaBan.Text = listMaKH[0];

                SqlCommand cmdTenKH = new("Set dateformat dmy\nSelect HoTenKH from HoaDon join KhachHang on HoaDon.MaKH=KhachHang.MaKH where NgayLapHD = @Today", con);
                cmdTenKH.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdTenKH.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaTenKH = cmdTenKH.ExecuteReader();
                while (sdaTenKH.Read())
                {
                    if (sdaTenKH[0] != DBNull.Value)
                        listTenKH.Add((string)sdaTenKH[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                txtMau.Text = listTenKH[0];

                SqlCommand cmdSoLG = new("Set dateformat dmy\nSelect SoLuong from HoaDon where NgayLapHD = @Today", con);
                cmdSoLG.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdSoLG.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaSoLG = cmdSoLG.ExecuteReader();
                while (sdaSoLG.Read())
                {
                    if (sdaSoLG[0] != DBNull.Value)
                        listSoLg.Add((int)sdaSoLG[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblDaBan.Content = listSoLg[0];

                SqlCommand cmdThanhTien = new("Set dateformat dmy\nSelect ThanhTien from HoaDon where NgayLapHD = @Today", con);
                cmdThanhTien.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdThanhTien.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaThanhTien = cmdThanhTien.ExecuteReader();
                while (sdaThanhTien.Read())
                {
                    if (sdaThanhTien[0] != DBNull.Value)
                        listThanhTien.Add((decimal)sdaThanhTien[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                txtTonKho.Text = string.Format("{0:C}", listThanhTien[0]);

                SqlCommand cmdMaNV = new("Set dateformat dmy\nSelect NhanVien.MaNV from HoaDon join NhanVien on HoaDon.MaNV=NhanVien.MaNV where NgayLapHD = @Today", con);
                cmdMaNV.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdMaNV.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaMaNV = cmdMaNV.ExecuteReader();
                while (sdaMaNV.Read())
                {
                    if (sdaMaNV[0] != DBNull.Value)
                        listMaNV.Add((string)sdaMaNV[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblHangSX.Content = listMaNV[0];

                SqlCommand cmdTenNV = new("Set dateformat dmy\nSelect NhanVien.HoTenNV from HoaDon join NhanVien on HoaDon.MaNV=NhanVien.MaNV where NgayLapHD = @Today", con);
                cmdTenNV.Parameters.Add("@Today", System.Data.SqlDbType.SmallDateTime);
                cmdTenNV.Parameters["@Today"].Value = ngay;
                SqlDataReader sdaTenNV = cmdTenNV.ExecuteReader();
                while (sdaTenNV.Read())
                {
                    if (sdaTenNV[0] != DBNull.Value)
                        listTenNV.Add((string)sdaTenNV[0]);
                    // else ListDoanhThu.Add(0); Ngày không có hàng đc mua
                }
                lblXuatXu.Content = listTenNV[0];

                con.Close();
                anhSP.Source = new BitmapImage(new Uri("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + listMaMH[0] + ".png"));
                index = 0; //Cứ mỗi lần khởi tạo WI biểu đồ thì sẽ gán index = 0 để tránh lỗi index was out of range
            }
            else
            {
                con.Close();
                //MessageBox.Show("Không có sản phẩm nào được bán ra trong ngày này");
            }
        }

        public WindowInformation(Tuple<MatHang,BitmapImage?> thamso)
        {
            //WindowInfor của Trang Sản Phẩm
            InitializeComponent();
            loaiWD = 1;
            MainDatabase mdb = new();
            //IOSPpg = thamsoPage;
            mathang = thamso;
            lblMaMH.Content = mathang.Item1.MaMh;
            lblTenMH.Content = mathang.Item1.TenMh;
            lblSoPK.Content = mathang.Item1.SoPhanKhoi;
            lblHangSX.Content = mathang.Item1.HangSx;
            lblXuatXu.Content = mathang.Item1.XuatXu;
            lblDaBan.Content = mdb.HoaDons.Where(u => u.MaMh == mathang.Item1.MaMh).Select(u => u.SoLuong).Sum().ToString() + " Chiếc";
            int SLtonkho = mdb.MatHangs.Where(u=>u.MaMh == mathang.Item1.MaMh).Select(u => u.SoLuongTonKho).FirstOrDefault();
            int SLdaban = mdb.HoaDons.Where(u => u.MaMh == mathang.Item1.MaMh).Select(u => u.SoLuong).Sum();
            txtTonKho.Text = (SLtonkho - SLdaban).ToString();

            if (mathang.Item1.GiaBanMh != null)
                txtGiaBan.Text = string.Format("{0:0.#####}", mathang.Item1.GiaBanMh);
            if (mathang.Item1.Mau != null)
                txtMau.Text = mathang.Item1.Mau;
            else 
                txtMau.Text=string.Empty;
            if (mathang.Item1.LoaiXe != null)
                txtLX.Text = mathang.Item1.LoaiXe;
            else
                txtLX.Text = string.Empty;
            if (mathang.Item2 != null)
            {
                /*BitmapImage image = new();
                //var stream = File.OpenRead(OFDFileName);
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                //image.StreamSource = stream;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new(mathang.Item2);
                image.EndInit();
                //stream.Close();
                //stream.Dispose();
                anhSP.Source = image;*/
                anhSP.Source = mathang.Item2;
            }
            DataContext = this;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (loaiWD == 1)
            {
                if (MessageBox.Show("Bạn Có Chắc Muốn Thoát? Mọi Chỉnh Sửa Sẽ Không Được Lưu Nếu Bạn Chưa Bấm Nút Lưu!", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                }
                else
                    Close();
            }
            else
                Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void txtGiaBan_LostFocus(object sender, RoutedEventArgs e)
        {
            txtGiaBan.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void txtGiaBan_GotFocus(object sender, RoutedEventArgs e)
        {
            txtGiaBan.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtMau_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMau.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtMau_LostFocus(object sender, RoutedEventArgs e)
        {
            txtMau.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void txtTonKho_GotFocus(object sender, RoutedEventArgs e)
        {
            txtTonKho.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtTonKho_LostFocus(object sender, RoutedEventArgs e)
        {
            txtTonKho.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void txtLX_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLX.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.Black);
        }

        private void txtLX_LostFocus(object sender, RoutedEventArgs e)
        {
            txtLX.SetCurrentValue(ForegroundProperty, System.Windows.Media.Brushes.White);
        }

        private void btnCapNhatAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OFD = new();
            OFD.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            //string destFile = $"/Products Images/{mathang.Item1.MaMh}.png.BKup";
            //string newPathToFile = $"/Products Images/{mathang.Item1.MaMh}.png";
            //dùng 2 dòng trên bị lỗi Could not find a part of the path 'C:\Products Images\ ... 

            if (OFD.ShowDialog() == true)
            {
                OFDFileName = OFD.FileName;
                anhSP.Source = new BitmapImage(new Uri(OFDFileName));
            }
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn Có Chắc Muốn Lưu Chỉnh Sửa?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
            }
            else
            {
                if (string.IsNullOrEmpty(txtGiaBan.Text) || string.IsNullOrWhiteSpace(txtMau.Text) || string.IsNullOrEmpty(txtTonKho.Text) || string.IsNullOrWhiteSpace(txtLX.Text))
                    MessageBox.Show("Có Trường Dữ Liệu Bị Thiếu, Vui Lòng Kiểm Tra Lại!");
                else
                {
                    try
                    {
                        MainDatabase mdb = new();
                        SqlCommand cmd;
                        con.Open();
                        cmd = new SqlCommand("Update MatHang\r\nset GiaBanMH=" + txtGiaBan.Text + ",Mau=N'" + txtMau.Text + "',LoaiXe=N'"+txtLX.Text+"', SoLuongTonKho=" + txtTonKho.Text + " where MaMH='" + mathang.Item1.MaMh + "'", con);
                        cmd.ExecuteNonQuery();
                        DateTime dt = DateTime.Now;
                        cmd = new SqlCommand("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getNV.MaNv + "', '" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', N'chỉnh sửa mặt hàng " + mathang.Item1.MaMh + "')", con);
                        cmd.ExecuteNonQuery();
                        con.Close();

                        string destFile = Path.Combine(Settings.Default.ProductFilePath, mathang.Item1.MaMh + ".BKup");
                        string newPathToFile = Path.Combine(Settings.Default.ProductFilePath, mathang.Item1.MaMh + ".png");
                        //anhSP.Source = null;                       

                        if(mathang.Item2 is not null)
                            mathang.Item2.StreamSource.Dispose();

                        //Trước khi đổi ảnh phải kiểm tra có ảnh được chọn hay không
                        if (OFDFileName != null)
                        {
                            if (File.Exists(newPathToFile)) //Nếu có 1 file ảnh khác tồn tại thì xoá nó đi và cập nhật file ảnh mới
                                File.Move(newPathToFile, destFile); //Đổi tên File
                            File.Copy(OFDFileName, newPathToFile); //Chỉnh tên File ảnh đc chọn

                            MatHang var = mathang.Item1;
                            mathang = Tuple.Create(var, BitmapImageFromFile.BitmapImageFromString(OFDFileName));

                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            File.Delete(destFile); //Xoá file tạm đi
                        }
                        //IOSPpg.Refresh(); //Tự động làm mới sau khi cập nhật thành công
                        MessageBox.Show("Cập Nhật Dữ Liệu Thành Công!");
                        //gọi refresh
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cập Nhật Dữ Liệu Thất Bại, Lỗi: " + ex.Message);
                    }
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (SoSP == 0)
            {
            }
            else
            {
                index += 1;
                if (index == SoSP)
                    --index;
                else
                {
                    lblMaMH.Content = listMaMH[index];
                    lblTenMH.Content = listTenMH[index];
                    lblSoPK.Content = listMaHD[index];
                    txtGiaBan.Text = listMaKH[index];
                    txtMau.Text = listTenKH[index];
                    lblDaBan.Content = listSoLg[index];
                    txtTonKho.Text = string.Format("{0:C}", listThanhTien[index]);
                    lblHangSX.Content = listMaNV[index];
                    lblXuatXu.Content = listTenNV[index];
                    anhSP.Source = new BitmapImage(new Uri("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + listMaMH[index] + ".png"));
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (SoSP == 0)
            {
                //Ignore, bẤM CHO VUI
            }
            else
            {
                index -= 1;
                if (index < 0) 
                    ++index;
                else
                {
                    lblMaMH.Content = listMaMH[index];
                    lblTenMH.Content = listTenMH[index];
                    lblSoPK.Content = listMaHD[index];
                    txtGiaBan.Text = listMaKH[index];
                    txtMau.Text = listTenKH[index];
                    lblDaBan.Content = listSoLg[index];
                    txtTonKho.Text = string.Format("{0:C}", listThanhTien[index]);
                    lblHangSX.Content = listMaNV[index];
                    lblXuatXu.Content = listTenNV[index];
                    anhSP.Source = new BitmapImage(new Uri("D:\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + listMaMH[index] + ".png"));
                }
            }
        }

    }
}
