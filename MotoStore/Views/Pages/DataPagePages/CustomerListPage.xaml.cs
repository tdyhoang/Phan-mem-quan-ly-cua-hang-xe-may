using System.Data;
using System.Diagnostics;
using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class CustomerListPage : INavigableView<ViewModels.CustomerListViewModel>
    {
        public ViewModels.CustomerListViewModel ViewModel
        {
            get;
        }

        public CustomerListPage(ViewModels.CustomerListViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.FillDataGrid();

            InitializeComponent();
            grdCustomer.ItemsSource = ViewModel.CustomerDataView;
        }
    }
}
