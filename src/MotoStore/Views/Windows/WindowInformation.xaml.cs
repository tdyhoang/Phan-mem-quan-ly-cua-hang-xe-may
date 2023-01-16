using MotoStore.Database;
using System.Windows;
using System.Windows.Input;
using MotoStore.Views.Pages;

namespace MotoStore.Views.Windows
{
    /// <summary>
    /// Interaction logic for WindowInformation.xaml
    /// </summary>
    public partial class WindowInformation : Window
    {
        public WindowInformation()
        {
            InitializeComponent();
            MainDatabase mdb = new();
            //switch(tung loai thong tin o day)
            rtbThongTin.AppendText("-Tên Mặt Hàng: "+ReportPage.tenXeBanChay+"\n");
            rtbThongTin.AppendText("-Mã Mặt Hàng: " + ReportPage.tenXeBanChay+"\n");
            rtbThongTin.AppendText("-Loại Xe: " + ReportPage.tenXeBanChay + "\n");
            rtbThongTin.AppendText("-Số Phân Khối: " + ReportPage.tenXeBanChay + "\n");
            rtbThongTin.AppendText("-Số Lượng Bán Ra: " + ReportPage.tenXeBanChay + "\n");
            rtbThongTin.AppendText("-Số Lượng Tồn Kho: " + ReportPage.tenXeBanChay + "\n");

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
