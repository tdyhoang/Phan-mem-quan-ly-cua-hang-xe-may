using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class SupplierListPage : INavigableView<ViewModels.SupplierListViewModel>
    {
        public ViewModels.SupplierListViewModel ViewModel
        {
            get;
        }

        public SupplierListPage(ViewModels.SupplierListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}