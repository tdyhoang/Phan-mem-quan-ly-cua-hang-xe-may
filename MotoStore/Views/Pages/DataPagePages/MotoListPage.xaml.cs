using System.Data;
using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class MotoListPage : INavigableView<ViewModels.MotoListViewModel>
    {
        public ViewModels.MotoListViewModel ViewModel
        {
            get;
        }

        public MotoListPage(ViewModels.MotoListViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.FillDataGrid();

            InitializeComponent();
            grdMoto.ItemsSource = ViewModel.MotoDataView;
        }
    }
}
