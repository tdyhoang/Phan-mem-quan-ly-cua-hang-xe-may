using MotoStore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;
using MotoStore.Views.Pages.LoginPages;
using Microsoft.Data.SqlClient;
using System.Windows.Markup;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private PageChinh pgChinh;
        private RichTextBox rtb;
        private ComboBox cbGioBD;
        private ComboBox cbPhutBD;
        private ComboBox cbGioKT;
        private ComboBox cbPhutKT;
        public DashboardPage()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public DashboardPage(PageChinh pgC)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            pgChinh = pgC;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblGioHeThong.Content = "Bây giờ là: " + DateTime.Now.ToLongTimeString();
        }

        private void btnDgXuat_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Visibility = Visibility.Collapsed;  //Ẩn Màn hình chính đi
            Windows.LoginView loginView = new Windows.LoginView();
            loginView.Show();           //Hiện màn hình đăng nhập
        }

        private void Lich_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            stkNoiDung.Children.Clear();
            RichTextBox rtbNoiDung = new RichTextBox();
            rtbNoiDung.Height = 150;
            rtbNoiDung.Width = 250;
            rtbNoiDung.Foreground = Brushes.Red;
            rtbNoiDung.FontSize = 13;
            rtbNoiDung.IsReadOnly = true;
            stkNoiDung.Children.Add(rtbNoiDung);
            MainDatabase mainDatabase = new MainDatabase();

            foreach(var ngay in mainDatabase.LenLichs.ToList())
            {
                if (Lich.SelectedDate.Value.ToString("dd/MM/yyyy") == ngay.NgLenLichBD.Value.ToString("dd/MM/yyyy"))
                {
                    rtbNoiDung.AppendText("Bắt Đầu: "+ngay.NgLenLichBD.Value.ToString("HH:mm")+ " - Kết Thúc: "+ngay.NgLenLichKT.Value.ToString("HH:mm")+ "\nNội Dung: " + ngay.NoiDungLenLich);
                }
                else
                { 
                    rtbNoiDung.AppendText("Không có sự kiện nổi bật");
                    break;
                }
            }

            stkLich.Children.Clear();
            rtb = new RichTextBox();
            rtb.Height = 100;
            rtb.Width = 240;
            rtb.Foreground = Brushes.Black;
            rtb.FontSize = 14;
            stkLich.Children.Add(rtb);
            cbGioBD = new ComboBox();
            cbGioBD.Height = 40;
            cbGioBD.Width = 70;
            cbGioBD.Margin= new Thickness(15,0,0,0);
            for (int i = 0; i <= 24; i++)
                cbGioBD.Items.Add(i);
            cbPhutBD = new ComboBox();
            cbPhutBD.Height = 40;
            cbPhutBD.Width = 70;
            cbPhutBD.Margin = new Thickness(170, -40, 0, 0);
            for (int i = 0; i <= 60; i++)
                cbPhutBD.Items.Add(i);
            stkLich.Children.Add(cbGioBD);
            stkLich.Children.Add(cbPhutBD);
            Label lblBatDau = new Label();
            lblBatDau.Content = "Giờ Bắt Đầu: ";
            lblBatDau.Height = 30;
            lblBatDau.Width = 85;
            lblBatDau.Margin = new Thickness(-160,-20,0,0);
            lblBatDau.FontSize = 14;
            lblBatDau.FontWeight = FontWeights.Medium;
            lblBatDau.Foreground = Brushes.Black;
            stkLich.Children.Add(lblBatDau);
            Label lblKetThuc = new Label();
            lblKetThuc.Content = "Giờ Kết Thúc: ";
            lblKetThuc.Height = 30;
            lblKetThuc.Width = 85;
            lblKetThuc.Margin = new Thickness(-160, 35, 0, 0);
            lblKetThuc.FontSize = 14;
            lblKetThuc.FontWeight= FontWeights.Medium;
            lblKetThuc.Foreground = Brushes.Black;
            stkLich.Children.Add(lblKetThuc);
            cbGioKT = new ComboBox();
            cbGioKT.Height = 40;
            cbGioKT.Width = 70;
            cbGioKT.Margin = new Thickness(15, -55, 0, 0);
            for (int i = 0; i <= 24; i++)
                cbGioKT.Items.Add(i);
            cbPhutKT = new ComboBox();
            cbPhutKT.Height = 40;
            cbPhutKT.Width = 70;
            cbPhutKT.Margin = new Thickness(170, -55, 0, 0);
            for (int i = 0; i <= 60; i++)
                cbPhutKT.Items.Add(i);
            stkLich.Children.Add(cbGioKT);
            stkLich.Children.Add(cbPhutKT);

            /*Hàm này hơi dài, nhưng tóm gọn lại là
              mỗi lần nhấn chuột vào ngày bất kì trên
              lịch thì sẽ hiện ra ngày đó có nội dung 
              đặc biệt hay không và các ô nội dung, giờ
              giấc nếu muốn lên lịch
            */
        }

        private void btnLenLich_Click(object sender, RoutedEventArgs e)
        {
            //if có ngày được select
            if (Lich.SelectedDate.HasValue)
            {
                string richText = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
                if (string.IsNullOrEmpty(cbGioBD.Text) || string.IsNullOrEmpty(cbPhutBD.Text) || string.IsNullOrEmpty(cbGioKT.Text) || string.IsNullOrEmpty(cbPhutKT.Text))
                    MessageBox.Show("Vui lòng chọn giờ cụ thể");
                else if(string.IsNullOrEmpty(richText))
                {
                    MessageBox.Show("Vui lòng ghi nội dung");
                }
                else
                {
                    if (int.Parse(cbGioBD.Text) > int.Parse(cbGioKT.Text) || int.Parse(cbGioBD.Text) == int.Parse(cbGioKT.Text) && int.Parse(cbPhutBD.Text) >= int.Parse(cbPhutKT.Text))
                        MessageBox.Show("Giờ kết thúc không thể nhỏ hơn hoặc bằng giờ bắt đầu");
                    else
                    {
                        SqlConnection con = new SqlConnection(@"Data Source=LAPTOP-5TG85UFG\SQLEXPRESS;Initial Catalog=QLYCHBANXEMAY;Integrated Security=True;TrustServerCertificate=True");
                        /*Dòng trên ae cop cái connection string của máy mình thay cho cái dòng trên
                        Nhớ chạy lại dòng lệnh databasePMQLCHBXM.sql trên máy mình*/

                        SqlCommand cmd;
                        con.Open();
                        string lich = Lich.SelectedDate.Value.ToString("dd/MM/yyyy");

                        //Giải thích dòng trên:
                        //Vì SelectedDate.Value sẽ cho ra ngày/tháng/năm + giờ/phút/giây nên ta lược bớt phần sau (chỉ giữ lại ngày tháng năm)

                        cmd = new SqlCommand("set dateformat dmy\nInsert into LenLich values('" + PageChinh.getMa.ToString() + "', '" + lich +" "+ cbGioBD.Text + ":" + cbPhutBD.Text + ":00" + "', '" + lich+" " + cbGioKT.Text + ":" + cbPhutKT.Text + ":00" + "', '" + richText + "')", con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Lên lịch thành công!");
                        stkLich.Children.Clear();
                        stkNoiDung.Children.Clear();
                    }
                }
            }
            else
                MessageBox.Show("Vui lòng chọn ngày lên lịch");
        }

        // Event Handler cho VisibleChanged
        private void DashboardPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                DashboardPage_Initialize();
            }
            else
            {
                // Collapse code here
            }
        }

        // Hàm khởi tạo DashboardPage, nên đặt tên khác cho dễ hiểu hơn
        private void DashboardPage_Initialize()
        {
            MainDatabase mdb = new MainDatabase();
            DateTime dt = DateTime.Now;
            switch (PageChinh.getLoaiNV)
            {
                case 1:  //Nhân viên loại 1
                    lblXinChao.Content = "Xin Chào, " + mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.TenNV).FirstOrDefault().ToString();
                    txtblSoNV.Text = "   Số Nhân Viên\n   Bạn Quản Lý:\n" + "".PadRight(12) + (mdb.NhanViens.Select(d => d.MaNv).Count() - 1).ToString();
                    txtblSoXe.FontSize = 21.5;
                    txtblSoXe.Text = "".PadRight(9) + "Số Xe\n" + "".PadRight(5) + "Trong Kho:\n" + "".PadRight(11) + mdb.MatHangs.Sum(d => d.SoLuongTonKho).ToString();
                    anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\anh3.png"));
                    break;
                case 2:  //Nhân viên loại 2
                    lblXinChao.Content = "Xin Chào, " + mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.TenNV).FirstOrDefault().ToString(); 

                    //3 dòng dưới để lấy ngày vào làm của nhân viên, tính số ngày từ đó đến nay và hiển thị nó
                    var dx = mdb.NhanViens.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.NgVl).FirstOrDefault();
                    int d3 = (int)(dt - dx).Value.TotalDays;
                    txtblSoNV.Text = " Bạn Đã Gắn Bó\n" + " Với Chúng Tôi:\n" + "".PadRight(6) + d3.ToString() + " Ngày";

                    var slg = mdb.HoaDons.Where(u => u.MaNv.ToString() == PageChinh.getMa).Select(u => u.SoLuong).Sum();
                    txtblSoXe.Text = "".PadRight(12) + slg.ToString() + "\n Là Số Xe\n Bạn Bán Được";

                    if (PageChinh.getSex == "Nữ")
                        anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNu.png"));
                    else
                        anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Assets\\userNam.png"));
                    break;
            }
        }
    }
}
