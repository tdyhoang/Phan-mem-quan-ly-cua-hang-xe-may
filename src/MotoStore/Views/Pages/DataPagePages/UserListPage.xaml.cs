using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class UserListPage : INavigableView<ViewModels.UserListViewModel>
    {
        public ViewModels.UserListViewModel ViewModel
        {
            get;
        }

        public UserListPage(ViewModels.UserListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
        }
    }
}