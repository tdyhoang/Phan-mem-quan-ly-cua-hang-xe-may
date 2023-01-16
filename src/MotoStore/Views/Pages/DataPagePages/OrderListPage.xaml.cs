using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class OrderListPage : INavigableView<ViewModels.OrderListViewModel>
    {
        public ViewModels.OrderListViewModel ViewModel
        {
            get;
        }

        public OrderListPage(ViewModels.OrderListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
        }
    }
}