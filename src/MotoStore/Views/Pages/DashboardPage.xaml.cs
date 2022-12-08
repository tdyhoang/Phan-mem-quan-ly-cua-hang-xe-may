using MotoStore.Views.Pages.LoginPages;
using MotoStore.Views.Windows;
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

        private void btnDgNhapDashBoard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new PageChinh());
        }

    }
}