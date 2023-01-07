using MotoStore.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.Data.SqlClient;
using MotoStore.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography;
using MotoStore.ViewModels;
using Wpf.Ui.Mvvm.Contracts;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Net;
using Microsoft.Data.SqlClient;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IONhaSXPage.xaml
    /// </summary>
    public partial class IONhaSXPage : Page
    {

        public IONhaSXPage()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
        }
       
        private int flag = 0;  //Đặt cờ để check xem nút Đăng Nhập có được Click vào hay chưa
        static public bool isValid = false;
        private readonly DispatcherTimer timer = new();
        private readonly DateTime dt = DateTime.Now;
        bool checkTenNSX = false;
        bool checkSDT = true;
        bool checkEmail = true;
        bool checkGia = false;

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
        

        private void btnAddNewNSX_Click(object sender, RoutedEventArgs e)
        {
            
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if (!(checkTenNSX && checkSDT && checkEmail && checkGia)) {
                MessageBox.Show("Vui lòng nhập đúng thông tin! ");
            }
            else
            {                                        
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into NhaCungCap values('" + txtTenNSX.Text + "','" + txtSDTNSX.Text + "','" + txtEmailNSX.Text + "',N'" + txtNuocSX.Text + " ' ,0)", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");            
            }
            timer.Start();


        }
        // Check Tên Nhà Sản XUất
        private void txtTenNSX_LostFocus(object sender, RoutedEventArgs e)
        {
             checkTenNSX = true;
            for (int i = 0; i < txtTenNSX.Text.Length; i++)
            {
                if ((txtTenNSX.Text[i] >= 48 && txtTenNSX.Text[i] <= 57))
                {

                    lblThongBao.Content = "Tên Nhà Cung Cấp không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;    
                    timer.Start();
                    checkTenNSX = false;
                    break;
                }
                
            }
            if(checkTenNSX)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
            
        }

        //Check SĐT
        private void txtSDTNSX_LostFocus(object sender, RoutedEventArgs e)
        {
             checkSDT = true;
            for (int i = 0; i < txtSDTNSX.Text.Length; i++)
            {
                if (!(txtSDTNSX.Text[i] >= 48 && txtSDTNSX.Text[i] <= 57))
                {
                    lblThongBao.Content = "SĐT không chứa các ký tự!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao.Visibility = Visibility.Visible;
                    checkSDT = false;
                    break;

                }
            }
            if (checkSDT)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }
        //check Email
        private void txtEmailNSX_LostFocus(object sender, RoutedEventArgs e)
        {
             checkEmail = true;
            if (!(txtEmailNSX.Text.Contains("@gmail.com")))
            {       
                lblThongBao.Content = "Email không hợp lệ";
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                checkEmail = false;
                
            }
            if (string.IsNullOrEmpty(txtEmailNSX.Text))
            {
                checkEmail = true;
            }    
            if (checkEmail)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
            
        }
        //Check QUốc Gia Nhà Cung Cấp 
        private void txtNuocSX_LostFocus(object sender, RoutedEventArgs e)
        {
             checkGia = true;
            for (int i = 0; i < txtNuocSX.Text.Length; i++)
            {
                if ((txtNuocSX.Text[i] >= 48 && txtNuocSX.Text[i] <= 57))
                {
                    lblThongBao.Content = "Nước Sản Xuất không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    timer.Start();
                    checkGia = false;
                    break;
                }
            }
            if (checkGia)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }
    }
}

