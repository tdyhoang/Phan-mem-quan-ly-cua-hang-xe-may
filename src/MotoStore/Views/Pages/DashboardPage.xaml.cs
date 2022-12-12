using MotoStore.Views.Pages.LoginPages;
using MotoStore.Views.Windows;
using System.Windows;
using Wpf.Ui.Common.Interfaces;

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
            
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

       /* private void btnDgXuat_Click(object sender, RoutedEventArgs e)
        {
            var getWindw = Window.GetWindow(this);
            getWindw.Close();
            LoginView lgv = new LoginView();
            lgv.Show();
        } */

        private void btnDgNhapDashBoard_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageChinh());
        }

        private void btnDgXuatDashBoard_Click(object sender, RoutedEventArgs e)
        {
            var getWindw = Window.GetWindow(this);
            getWindw.Hide();
            LoginView lgv = new LoginView();
            lgv.ShowDialog();
        }
    }
}