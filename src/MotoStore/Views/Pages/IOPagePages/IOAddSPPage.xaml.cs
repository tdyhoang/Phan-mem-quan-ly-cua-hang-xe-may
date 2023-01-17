using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Data.SqlClient;
using System.IO;
using MotoStore.Database;
using System.Windows.Threading;


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
        }
        private readonly DispatcherTimer timer = new();
        private MainDatabase mdb = new();
        private readonly DateTime dt = DateTime.Now;
        private SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
        static private int DcPhepThem = 0; //Check xem có đc phép thêm(các ô (*) kh đc bỏ trống)
        static private int dem = 0;   //Biến đếm số lần nháy
        private bool Nhay = false;

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
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new(openFileDialog.FileName);

                ImageSP.Source = new BitmapImage(fileUri);
               // File.Move(openFileDialog.FileName, "C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Products Images\\Temp.png");
            }
        }
       

        private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd;
            if (DcPhepThem != 5)
            {
                MessageBox.Show("Các trường dữ liệu quan trọng (Có dấu(*)) bị thiếu, vui lòng xem lại!");
            }
            else
            {

                //File.Move("C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Products Images\\Temp.png", "C:\\Users\\ADMIN\\Documents\\Github\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Products Images\\" + MaMH+".png");
                con.Open();
                foreach (var xe in mdb.MatHangs)
                {
                    if (xe.MaNccNavigation.TenNcc == txtHangSXSP.Text) //Prob here
                    {
                        cmd = new("Set Dateformat dmy\nInsert into MatHang values('" + txtTenSP.Text + "', " + txtPhanKhoiSP.Text + ", null, '" + txtGiaNhapSP.Text + "', null, null, '"+xe.MaNcc+"', '" + txtHangSXSP.Text + "', N'" + txtXuatXuSP.Text + "', '" + txtMoTaSP.Text + "',0)", con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Thêm dữ liệu thành công");
                        DcPhepThem = 0;
                    }
                }
            }
        }

        private void txtTenSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkTenSP = true;
            if (string.IsNullOrWhiteSpace(txtTenSP.Text)) 
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkTenSP = false;
                if (DcPhepThem > 0)
                    DcPhepThem--;
            }
            if (checkTenSP)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
                DcPhepThem++;
            }
        }

        private void txtGiaNhapSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkGiaNhapSP = true;
            if (string.IsNullOrEmpty(txtGiaNhapSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkGiaNhapSP = false;
                if (DcPhepThem > 0)
                    DcPhepThem--;
            }
            if (checkGiaNhapSP)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
                DcPhepThem++;
            }
        }

        private void txtXuatXuSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkXS = true;
            if (string.IsNullOrWhiteSpace(txtXuatXuSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkXS = false;
                if (DcPhepThem > 0)
                    DcPhepThem--;
            }
            if (checkXS)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
                DcPhepThem++;
            }
        }

        private void txtPhanKhoiSP_LostFocus(object sender, RoutedEventArgs e) //Check Phân Phối của Xe
        {
            bool checkPhanKhoi = true;
            if (string.IsNullOrEmpty(txtPhanKhoiSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkPhanKhoi = false;
                if (DcPhepThem > 0)
                    DcPhepThem--;
            }
            if (checkPhanKhoi)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
                DcPhepThem++;
            }
        }

        private void txtHangSXSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkHangSP = true;
            if (string.IsNullOrEmpty(txtHangSXSP.Text))
            {
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkHangSP = false;
                if (DcPhepThem > 0)
                    DcPhepThem--;
            }
            if (checkHangSP)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
                DcPhepThem++;
            }
        }
    }
}