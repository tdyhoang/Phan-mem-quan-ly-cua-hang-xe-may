using Wpf.Ui.Common.Interfaces;
using System.Diagnostics;
using Demo;

using System.Windows.Threading;
using System;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : INavigableView<ViewModels.DashboardViewModel>
    {
        public ViewModels.DashboardViewModel ViewModel
        {
            get;
        }

        public DashboardPage(ViewModels.DashboardViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void buttonDangNhapDashBoard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("Demo.exe");  //Bắt đầu chạy form Đăng Nhập
            //MessageBox.Show(PageChinh.DangNhap.ToString());


            if (Demo.PageChinh.getDangNhap())
            {
                lblTen.Content = "Xin Chào, Trung!";
                anhNhanVien.Source = new BitmapImage(new Uri("C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\MotoStore\\Views\\Pages\\anh3.png"));
                lblChucVu.Content = "Nhân Viên Quản Lý";
            }
            else
                lblTen.Content = "Sai";

            //Hàm này để kiểm tra tài khoản mật khẩu form Đăng Nhập và hiện thông tin nhân viên tương ứng
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lblGioHeThong.Content = "    Bây giờ là: "+DateTime.Now.ToLongTimeString();
        }

        private void Lich_SelectedDatesChanged(object sender,SelectionChangedEventArgs e)
        {
            
            RichTextBox rtb = new RichTextBox();
            rtb.Height = 100;
            rtb.Width = 200;
            rtb.FontSize = 11;
            rtb.Foreground = Brushes.Black;
            StkPanelLenLich.Children.Add(rtb);
            
        }

    }
}