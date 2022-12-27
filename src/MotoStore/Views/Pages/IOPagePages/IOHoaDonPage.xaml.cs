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
namespace MotoStore.Views.Pages.IOPagePages
    
{
    /// <summary>
    /// Interaction logic for IOHoaDonPage.xaml
    /// </summary>
    public partial class IOHoaDonPage : Page
    {
        private MainDatabase mainDatabase = new MainDatabase(); 
        public IOHoaDonPage()
        {

            InitializeComponent();
        }

        private void btnAddNewHoaDon_Click(object sender, RoutedEventArgs e)
        {
            bool check = true;
            SqlConnection con = new SqlConnection(@"Data Source=.\SQLExpress;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
            SqlCommand cmd;
            if ((string.IsNullOrWhiteSpace(txtMaKHHD.Text)) || (string.IsNullOrWhiteSpace(txtMaSPHD.Text)) || (string.IsNullOrWhiteSpace(txtNgayXuatHD.Text)) || (string.IsNullOrWhiteSpace(txtSoLuongHD.Text)) || (string.IsNullOrWhiteSpace(txtGiamGiaHD.Text)) || (string.IsNullOrWhiteSpace(txtThanhTienHD.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
            }
            else
            {
                
                DateTime date;
                if (!(DateTime.TryParseExact(txtNgayXuatHD.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)))
                {
                    MessageBox.Show("Ngày xuất Hóa Đơn không hợp lệ !");
                    check = false;
                }

                for (int i = 0; i < txtSoLuongHD.Text.Length; i++) //Check Số Luọng SanPham Trong Hóa Đơn
                {
                    if (!(txtSoLuongHD.Text[i] >= 48 && txtSoLuongHD.Text[i] <= 57))
                    {
                        MessageBox.Show("So luong không được chứa các ký tự ");
                        check = false;
                    }
                }
             

                if (check)
                {
                    con.Open();
                    cmd = new SqlCommand("Set Dateformat dmy\nInsert into HoaDon values( NEWID(),  " + "  N'" + txtMaSPHD.Text + "','" + txtMaKHHD.Text + "','" + PageChinh.getMa + "','" + txtNgayXuatHD.Text + "','" + txtSoLuongHD.Text + "','" + txtThanhTienHD.Text +  " ' )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                }
            }
        }

        private void txtMaKHHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGiamGia();
        }
        public void UpdateGiamGia()
        {
            bool check = false;
            string LoaiKH=string.Empty;
            foreach ( KhachHang item in mainDatabase.KhachHangs.ToList())
            {
                if(txtMaKHHD.Text==item.MaKh.ToString())
                {
                    check = true;
                    LoaiKH = item.LoaiKh.ToString();
                    break;
                }

            }
            if (!check)
            {
                txtGiamGiaHD.Text = String.Empty;
                return;
            }
            switch(LoaiKH)
            {
                case "Vip" : txtGiamGiaHD.Text = "15%";
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
            bool check = false;
            decimal? giamgia = 0;
            decimal? giaban = 0;
            decimal? thanhtien = 0;
            if (string.IsNullOrEmpty(txtGiamGiaHD.Text ) || string.IsNullOrEmpty(txtMaSPHD.Text) || string.IsNullOrEmpty(txtSoLuongHD.Text))
            {
                txtThanhTienHD.Text = String.Empty;
                return;
            }
            
            
            foreach (MatHang item in mainDatabase.MatHangs.ToList())
            {
                if (txtMaSPHD.Text == item.MaMh.ToString())
                {
                    if(item.GiaBanMh is null)
                    {
                        giaban = 0;
                    }
                    else
                    {
                        giaban = item.GiaBanMh;  
                    }
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                txtThanhTienHD.Text = string.Empty;
                return;
            }
            check = false;
            for (int i = 0; i < txtSoLuongHD.Text.Length; i++) //Check Số Luọng SanPham Trong Hóa Đơn
            {
                if ((txtSoLuongHD.Text[i] >= 48 && txtSoLuongHD.Text[i] <= 57))
                {
                    check = true;
                }
            }
            if (!check)
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
                    giamgia=(decimal)0.05;
                    break;
                case "15%":
                    giamgia =(decimal) 0.15;
                    break;

            }
            thanhtien = (giaban * int.Parse(txtSoLuongHD.Text) * (1 + giamgia));
            txtThanhTienHD.Text = thanhtien.ToString();
        }

        private void txtMaSPHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThanhTien();
        }

        private void txtSoLuongHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThanhTien();
        }

        private void txtGiamGiaHD_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateThanhTien();
        }
    }
}
