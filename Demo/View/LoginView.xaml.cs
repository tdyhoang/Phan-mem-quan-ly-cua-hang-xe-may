using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
/*Trong Quá Trình Chạy Login Form, Nếu Gặp Lỗi 
 Unable to copy file "obj\Debug\Demo.exe" to "bin\Debug\Demo.exe".
 The process cannot access the file 'bin\Debug\Demo.exe' because it is being used by another process.Demo:
1.Tắt VSCode
2.Bật TaskManager
3.Tìm Demo.exe và EndTask nó
4.Bật VSCode và chạy lại
 */

namespace Demo.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        /*QuenMatKhau qmk = new QuenMatKhau();
        static public GuiMa gm = new GuiMa();
        static public string ngonngu; //Đặt biến tĩnh để các form sau có thể truy cập*/
        static public NavigationService nav;
        public LoginView()
        {
            InitializeComponent();
            //Loaded += OnMainWindowLoaded;
            //MainFrame.Navigate(new PageChinh());
            //nav = MainFrame.NavigationService;
            //LoginView lgv = new LoginView();
            
        }
       /* private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
        } */
        /*public void ChangeView(Page view)
        {
            MainFrame.NavigationService.Navigate(view);
        } */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        /*protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
        }*/
        public static class Navigator
        {
            private static NavigationService NavigationService { get; } = (Application.Current.MainWindow as LoginView).MainFrame.NavigationService;

            public static void Navigate(string path, object param = null)
            {
                NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute), param);
            }

            public static void GoBack()
            {
                NavigationService.GoBack();
            }

            public static void GoForward()
            {
                NavigationService.GoForward();
            }
        }
    }
}
