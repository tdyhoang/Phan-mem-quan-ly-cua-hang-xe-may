using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using MotoStore.Database;
using Microsoft.Data.SqlClient;

namespace MotoStore.Views.Pages.LoginPages
{
    /// <summary>
    /// Interaction logic for PageGuiMa.xaml
    /// </summary>
    public partial class PageGuiMa
    {
        private readonly PageChinh pgC;
        private readonly DateTime dt= DateTime.Now;
        private string getMa;
        private string getTen;
        public PageGuiMa(PageChinh pageChinh)
        {
            InitializeComponent();
            pgC = pageChinh;
            timer.Tick += Timer_Tick;
        /*Giống như PageQuenMatKhau, dùng hàm khởi tạo cõng theo PageChinh để có thể Back về PageChinh*/
        }

        static private int dem = 0;   //Biến đếm số lần nháy
        private bool Nhay = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (dem == 7)           //dem = 7 Thì Ngừng Nháy
                timer.Stop();
            if (Nhay)
            {
                lbl.Foreground = Brushes.Red;
                dem++;
            }
            else
            {
                lbl.Foreground = Brushes.Black;
                dem++;
            }
            Nhay = !Nhay;
            //Hàm Này Để Nháy Thông Báo 
        }

        private readonly DispatcherTimer timer = new();

        private void buttonXacNhanGuiMa_Click(object sender, RoutedEventArgs e)
        {
            dem = 0;
            MainDatabase mdb = new();
            SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            SqlCommand cmd;
            con.Open();
            if (string.IsNullOrEmpty(txtMa.Text))
            {
                switch (PageQuenMatKhau.ngonngu)
                {
                    case "Tiếng Việt":
                        lbl.Content = "               Vui Lòng Nhập Mã!";
                        timer.Start();
                        break;
                    case "English":
                        lbl.Content = "               Please Fill The Code!";
                        timer.Start();
                        break;
                }
            }
            else if (PageQuenMatKhau.ma==long.Parse(txtMa.Text))
            {
                /*Cập nhật mật khẩu mới lên DataBase
                ==>*/
                cmd = new("Update UserApp\nSet Password = '" + PageQuenMatKhau.getPass+"'" + "\nWhere Email = '" + PageQuenMatKhau.getEmail+"'", con);
                cmd.ExecuteNonQuery();
                foreach(var item in mdb.NhanViens.ToList())
                {
                    if(item.Email==PageQuenMatKhau.getEmail)
                    {
                        getMa = item.MaNv.ToString();
                        getMa = getMa.ToUpper();
                        var hoTenNV = mdb.NhanViens.Where(u => u.MaNv.ToString() == item.MaNv.ToString()).Select(u => u.HoTenNv).FirstOrDefault().ToString();
                        var seperatedHoTenNV = hoTenNV.Split(' ');
                        var tenNV = seperatedHoTenNV[seperatedHoTenNV.Length - 1];
                        getTen = tenNV;
                        break;
                    }
                }
                cmd = new("Set Dateformat dmy\nInsert into LichSuHoatDong values('" + getMa + "', N'" + getTen + "', '" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', N'" + "thay đổi mật khẩu')", con);
                cmd.ExecuteNonQuery();
                con.Close();

                switch (PageQuenMatKhau.ngonngu)
                {
                    case "English":
                        MessageBox.Show("Change Password Successful");
                    break;
                    case "Tiếng Việt":
                        MessageBox.Show("Đổi Mật Khẩu Thành Công");
                        break;
                }
                NavigationService.Navigate(pgC);
            }
            else
            {
                switch(PageQuenMatKhau.ngonngu)
                {
                    case "Tiếng Việt":
                        lbl.Content = "Mật Mã Bạn Nhập Không Khớp, Hãy Kiểm Tra Lại";
                        break;
                    case "English":
                        lbl.Content = "Your Code You Fill Didn't Match, Check Again!";;
                        break;
                }
                timer.Interval = new(0, 0, 0, 0, 200);
                timer.Start();
                txtMa.Clear();
                txtMa.Focus();
            }
        }

    }
}
