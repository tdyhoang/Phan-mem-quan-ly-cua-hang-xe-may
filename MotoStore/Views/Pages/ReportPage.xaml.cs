using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class ReportPage : INavigableView<ViewModels.ReportViewModel>
    {
        public ViewModels.ReportViewModel ViewModel
        {
            get;
        }

        public ReportPage(ViewModels.ReportViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}