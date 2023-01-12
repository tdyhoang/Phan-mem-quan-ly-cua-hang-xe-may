using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.IO;
using MotoStore.Database;
using System.Collections.ObjectModel;
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
        private int flag = 0;  //Đặt cờ để check xem nút Đăng Nhập có được Click vào hay chưa
        static public bool isValid = false;
        private readonly DispatcherTimer timer = new();
        private readonly DateTime dt = DateTime.Now;
        bool checkTenSP= false;
        bool checkGiaNhapSP=false;
        bool checkPhanKhoi=false;

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
                File.Copy(openFileDialog.FileName, "F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Views\\Pages\\IO_Images\\Temp.png");
            }
        }
       

            private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {

           
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if (!(checkTenSP && checkGiaNhapSP && checkPhanKhoi ))
            {
                MessageBox.Show("Vui lòng nhập đúng thông tin! ");
            }
            else
            {
                    string MaMH = Guid.NewGuid().ToString();
                    File.Move("F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Views\\Pages\\IO_Images\\Temp.png", "F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + MaMH+".png");

                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into MatHang values('" + MaMH +  "', N'" + txtTenSP.Text + "','"  + txtPhanKhoiSP.Text + "','" + txtGiaNhapSP.Text  + "','" + cmbHangSXSP.Text + "',N'" + cmbXuatXuSP.Text + "','" + txtMoTaSP.Text + " ' )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                
            }
        }

        private void txtTenSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkTenSP = true;
            for (int i = 0; i < txtTenSP.Text.Length; i++)
            {
                if ((txtTenSP.Text[i] >= 48 && txtTenSP.Text[i] <= 57))
                {

                    lblThongBao.Content = "Tên Sản Phẩm không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;
                    timer.Start();
                    checkTenSP = false;
                    break;
                }

            }
            if (checkTenSP)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }

        private void txtGiaNhapSP_LostFocus(object sender, RoutedEventArgs e)
        {
            bool checkGiaNhapSP = true;
            for (int i = 0; i < txtGiaNhapSP.Text.Length; i++)
            {
                if (!(txtGiaNhapSP.Text[i] >= 48 && txtGiaNhapSP.Text[i] <= 57))
                {

                    lblThongBao.Content = "Giá Nhập không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;
                    timer.Start();
                    checkGiaNhapSP = false;
                    break;
                }

            }
            if (checkGiaNhapSP)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }

        private void txtPhanKhoiSP_LostFocus(object sender, RoutedEventArgs e) //Check Phân Phối của Xe
        {
            checkPhanKhoi = true;
            for (int i = 0; i < txtPhanKhoiSP.Text.Length; i++)
            {
                if (!(txtPhanKhoiSP.Text[i] >= 48 && txtPhanKhoiSP.Text[i] <= 57))
                {
                    lblThongBao.Content = "Phân Khối không chứa các ký tự!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;
                    checkPhanKhoi = false;
                    break;

                }
            }
            if (checkPhanKhoi)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }
    }
}
