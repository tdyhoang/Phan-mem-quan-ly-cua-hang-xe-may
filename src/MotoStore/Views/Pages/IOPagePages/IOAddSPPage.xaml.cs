using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using MotoStore.Views.Pages.LoginPages;
using System.Security.Cryptography;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOAddSPPage.xaml
    /// </summary>
    public partial class IOAddSPPage : Page
    {
        public IOAddSPPage()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
            RefreshMatHang();
            DataContext = this;
        }
        private readonly DispatcherTimer timer = new();
        private readonly MainDatabase mdb = new();
        private readonly SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        static private int dem = 0;   //Biến đếm số lần nháy
        private bool Nhay = false;
        internal ObservableCollection<NhaCungCap> nhaCungCaps;

        public void RefreshMatHang()
        {
            MainDatabase dtb = new();
            nhaCungCaps = new(dtb.NhaCungCaps);
            foreach (var mat in nhaCungCaps.ToList())
            {
                if (mat.DaXoa)
                {
                    nhaCungCaps.Remove(mat);
                }
            }
            cmbMaNCC.ItemsSource = nhaCungCaps;
            cmbMaNCC.Text = string.Empty;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dem == 7)           //dem = 7 Thì Ngừng Nháy
                timer.Stop();
            if (Nhay)
            {
                lblThongBao.Foreground = Brushes.Red;
                dem++;
            }
            else
            {
                lblThongBao.Foreground = Brushes.Black;
                dem++;
            }
            Nhay = !Nhay;
            //Hàm Này Để Nháy Thông Báo 
        }
        private void btnLoadImageSP_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            //string destFile = @$"pack://application:,,,/Avatars/{}.BackUp";
            //string newPathToFile = @$"pack://application:,,,/Avatars/{PageChinh.getMa}";
            //destFile: file dự phòng
            //newPathToFile: file ảnh mới
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new(openFileDialog.FileName);
                ImageSP.Source = new BitmapImage(fileUri);
            }
        }
       

        private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd;
            if (string.IsNullOrWhiteSpace(txtTenSP.Text)|| string.IsNullOrEmpty(txtGiaNhapSP.Text)|| string.IsNullOrWhiteSpace(txtXuatXuSP.Text)|| string.IsNullOrEmpty(txtPhanKhoiSP.Text)|| string.IsNullOrWhiteSpace(cmbMaNCC.Text))
            {
                MessageBox.Show("Các trường dữ liệu quan trọng (Có dấu(*)) bị thiếu, vui lòng xem lại!");
            }
            else
            {
                //File.Move("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Products Images\\Temp.png", "C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + MaMH+".png");
                con.Open();
                cmd = new("Set Dateformat dmy\nInsert into MatHang values('" + txtTenSP.Text + "', " + txtPhanKhoiSP.Text + ", null, '" + txtGiaNhapSP.Text + "', null, 0, '" + cmbMaNCC.Text + "', '" + txtHangSXSP.Text + "', N'" + txtXuatXuSP.Text + "', N'" + txtMoTaSP.Text + "', 0)", con);
                cmd.ExecuteNonQuery();
                DateTime dt = DateTime.Now;
                cmd = new("Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '" + PageChinh.getMa + "', '" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', N'thêm mặt hàng mới')", con);
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Thêm dữ liệu thành công");
            }
        }

        private void txtTenSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
            }
        }

        private void txtGiaNhapSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtGiaNhapSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
            }
        }

        private void txtXuatXuSP_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtXuatXuSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
            }
        }

        private void txtPhanKhoiSP_LostFocus(object sender, RoutedEventArgs e) //Check Phân Phối của Xe
        {
            if (string.IsNullOrEmpty(txtPhanKhoiSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
            }
        }

        private void cmbMaNCC_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbMaNCC.SelectedItem is NhaCungCap ncc)
            {
                cmbMaNCC.Text = ncc.MaNcc;
            }
        }

        private void cmbMaNCC_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkMaNCC = true;
            if (string.IsNullOrWhiteSpace(cmbMaNCC.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
            }
        }
    }
}