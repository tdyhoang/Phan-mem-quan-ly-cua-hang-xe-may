using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for IOView.xaml
    /// </summary>
    public partial class IOPage : INavigableView<ViewModels.IOViewModel>
    {
        public ViewModels.IOViewModel ViewModel
        {
            get;
        }

        public IOPage(ViewModels.IOViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}