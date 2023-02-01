using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MotoStore.Views.Pages.LoginPages
{
    /// <summary>
    /// Interaction logic for PageGuiMa.xaml
    /// </summary>
    public partial class PageGuiMa
    {
        private readonly PageChinh pgC;
        private readonly PageQuenMatKhau pgQMK;
        public PageGuiMa(PageChinh pageChinh, PageQuenMatKhau pageQMK)
        {
            InitializeComponent();
            pgC = pageChinh;
            pgQMK= pageQMK;
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
            SqlCommand cmd;
            using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            try
            {
                con.Open();
                if (string.IsNullOrEmpty(txtMa.Text))
                {
                    lbl.Content = "                         Vui Lòng Nhập Mã!";
                    timer.Start();
                }
                else if (string.Equals(PageQuenMatKhau.ma.ToString("D6"), txtMa.Text))
                {
                    /*Cập nhật mật khẩu mới lên DataBase
                    ==>*/
                    cmd = new($"Update UserApp Set Password = @Password Where MaNv = @MaNV", con);
                    cmd.Parameters.Add($"@MaNV", SqlDbType.VarChar);
                    cmd.Parameters[$"@MaNV"].Value = PageQuenMatKhau.getUser.MaNv;
                    cmd.Parameters.Add($"@Password", SqlDbType.NVarChar);
                    cmd.Parameters[$"@Password"].Value = PageQuenMatKhau.getUser.Password;
                    cmd.ExecuteNonQuery();
                    cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values('{PageQuenMatKhau.getUser.MaNv}', '{DateTime.Now:dd/MM/yyyy HH:mm:ss}', N'thay đổi mật khẩu')", con);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Đổi Mật Khẩu Thành Công");
                    NavigationService.Navigate(pgC);
                }
                else
                {
                    lbl.Content = "Mật Mã Bạn Nhập Không Khớp, Hãy Kiểm Tra Lại";
                    timer.Interval = new(0, 0, 0, 0, 200);
                    timer.Start();
                    txtMa.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtMa_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
                buttonXacNhanGuiMa_Click(sender, e);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(pgQMK);
        }

        private void buttonGuiLaiMa_Click(object sender, RoutedEventArgs e)
        {
            pgQMK.GuiMail();
        }
    }
}
