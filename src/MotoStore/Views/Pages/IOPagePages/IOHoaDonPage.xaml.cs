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
using System.Globalization;
using MotoStore.Views.Pages.LoginPages;
using MotoStore.Database;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MotoStore.Views.Pages.IOPagePages
    
{
    /// <summary>
    /// Interaction logic for IOHoaDonPage.xaml
    /// </summary>
    public partial class IOHoaDonPage : Page
    {
        internal ObservableCollection<MatHang> matHangs;
        internal ObservableCollection<KhachHang> KhachHangs;
        private readonly DispatcherTimer timer = new();
        private readonly DateTime dt = DateTime.Now;
        static private int dem = 0;   //Biến đếm số lần nháy
        private bool Nhay = false;
        bool checkNgayXHD = false;
        bool checkSoLuong= false;
        private readonly MainDatabase mainDatabase = new();
       
        internal List<HoaDon> TableData = new();
        public IOHoaDonPage()
        {

            InitializeComponent();
            RefreshMatHang();
            RefreshKhachHang();
            txtNgayXuatHD.Text =DateTime.Today.ToShortDateString();

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
        private void btnAddNewHoaDon_Click(object sender, RoutedEventArgs e)
        {
            
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if (!(checkNgayXHD && checkSoLuong))
            {
                MessageBox.Show("Vui lòng nhập đúng thông tin! ");
            }
            else
            {
                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into HoaDon values( NEWID(),  " + "  N'" + cmbMaSPHD.Text + "','" + cmbMaKHHD.Text + "','" + PageChinh.getMa + "','" + txtNgayXuatHD.Text + "','" + txtSoLuongHD.Text + "','" + txtThanhTienHD.Text + " ' )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                
            }
        }  
        public void UpdateGiamGia()
        {       
            string LoaiKH = string.Empty;
            if(cmbMaKHHD.SelectedItem is KhachHang kh )
            {
                LoaiKH = kh.LoaiKh;
            }    
            else
            {
                txtGiamGiaHD.Text = String.Empty;
                return;
            }
            switch (LoaiKH)
            {
                case "Vip": txtGiamGiaHD.Text = "15%";
                    break;
                case "Thân quen":
                    txtGiamGiaHD.Text = "5%";
                    break;
                case "Thường":
                    txtGiamGiaHD.Text = "0%";
                    break;
            }

        }
        public void UpdateThanhTien()
        {
            
            decimal? giamgia = 0;
            decimal? giaban = 0;
            decimal? thanhtien = 0;
            if (string.IsNullOrEmpty(txtGiamGiaHD.Text)  || string.IsNullOrEmpty(txtSoLuongHD.Text))
            {
                txtThanhTienHD.Text = String.Empty;
                return;
            }
        
            if (cmbMaSPHD.SelectedItem is MatHang mh)
            {
                giaban = mh.GiaBanMh;
                giaban ??= 0;
            }    
                
            else
            {
                txtThanhTienHD.Text = string.Empty;
                return;
            }                    
            switch (txtGiamGiaHD.Text)
            {
                case "0%":
                    giamgia = 0;
                    break;
                case "5%":
                    giamgia = (decimal)0.05;
                    break;
                case "15%":
                    giamgia = (decimal)0.15;
                    break;

            }
            thanhtien = (giaban * int.Parse(txtSoLuongHD.Text) * (1 - giamgia));
            txtThanhTienHD.Text = thanhtien.ToString();
        }

       

        private void txtSoLuongHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThanhTien();
        }

        private void txtGiamGiaHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThanhTien();
        } 
        public void  RefreshMatHang()
        {
            MainDatabase dtb = new();
            matHangs = new(dtb.MatHangs);
            foreach (var mat in matHangs.ToList())
            {
                if(mat.DaXoa)
                {
                    matHangs.Remove(mat);
                }    
            }
            cmbMaSPHD.ItemsSource = matHangs;
            cmbMaSPHD.Text=String.Empty;
        }
        public void RefreshKhachHang()
        {
            MainDatabase dtb = new();
            KhachHangs = new(dtb.KhachHangs);
            foreach (var khach in KhachHangs.ToList())
            {
                if (khach.DaXoa)
                {
                    KhachHangs.Remove(khach);
                }
            }
            cmbMaKHHD.ItemsSource = KhachHangs;
            cmbMaKHHD.Text = String.Empty;
        }

        private void cmbMaSPHD_KeyDown(object sender, KeyEventArgs e)
        {
            //if(e.OriginalSource is ListBoxItem)
            //{
            //    ListBoxItem listBoxItem = e.OriginalSource as ListBoxItem;
            //    cmbMaSPHD.Text=listBoxItem.Content as string;
            //    if(e.Key == Key.Enter)
            //    {
            //        cmbMaSPHD.Focus();
            //    }
            //}    
        }
        private void cmbMaSPHD_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbMaSPHD.SelectedItem is MatHang mh)
            {
                cmbMaSPHD.Text = mh.MaMh;
            }


        }

        private void txtNgayXuatHD_LostFocus(object sender, RoutedEventArgs e)
        {
            //Check Ngày xuất HĐon
            checkNgayXHD = true;
            DateTime date;
            if (!(DateTime.TryParseExact(txtNgayXuatHD.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)))
            {
                lblThongBao.Content = "Ngày xuất hóa đơn không hợp lệ!";
                timer.Interval = new(0, 0, 0, 0, 200);
                lblThongBao.Visibility = Visibility.Visible;
                timer.Start();
                checkNgayXHD = false;
            }
            if (checkNgayXHD)
            {
                lblThongBao.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSoLuongHD_LostFocus(object sender, RoutedEventArgs e)
        {
            checkSoLuong = true;
            for (int i = 0; i < txtSoLuongHD.Text.Length; i++)
            {
                if (!(txtSoLuongHD.Text[i] >= 48 && txtSoLuongHD.Text[i] <= 57))
                {

                    lblThongBao1.Content = "Số Lượng không hợp lệ!";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    lblThongBao1.Visibility = Visibility.Visible;
                    timer.Start();
                    checkSoLuong = false;
                    break;
                }

            }
            if (checkSoLuong)
            {
                lblThongBao1.Visibility = Visibility.Collapsed;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshMatHang();    

        }
      


        private void btnRefreshKH_Click(object sender, RoutedEventArgs e)
        {
            RefreshKhachHang();
        }

        private void cmbMaKHHD_DropDownClosed(object sender, EventArgs e)
        {

            if (cmbMaKHHD.SelectedItem is KhachHang kh)
            {
                cmbMaKHHD.Text = kh.MaKh;
            }
        }

        private void cmbMaKHHD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGiamGia();
        }

        private void cmbMaSPHD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateThanhTien();
        }



        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (e.Key == Key.Up || e.Key == Key.Down || e.Key==Key.Return || e.Key==Key.Enter)
                {
                    cmbMaSPHD_TextChanged(sender,null);
                }
            }
        }

        private void cmbMaSPHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (true)
            {
                if (cmbMaSPHD.SelectedItem is MatHang mh)
                {
                    cmbMaSPHD.Text = mh.MaMh;
                }

            }
            
        }

        private void cmbMaSPHD_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            cmbMaSPHD_TextChanged(sender, null);

        }

        private void cmbMaSPHD_DropDownClosed_1(object sender, EventArgs e)
        {

        }

        private void StackPanel_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void cmbMaSPHD_KeyUp(object sender, KeyEventArgs e)
        {
            cmbMaSPHD_TextChanged(sender, null);
        }
    }
}
