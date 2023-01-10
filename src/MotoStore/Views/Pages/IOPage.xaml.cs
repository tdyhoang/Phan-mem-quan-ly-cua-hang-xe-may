using Wpf.Ui.Common.Interfaces;
namespace MotoStore.Views.Pages
{
    
    /// <summary>
    /// Interaction logic for IOPage.xaml
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

        private void btnAddSPPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content = ViewModel.iosppage;
        }



        private void btnAddNSXPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content = ViewModel.ionhasxpage;
        }

        private void btnAddKhachHang_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content = ViewModel.iokhachhangpage;
        }

        private void btnAddHoaDon_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content = ViewModel.iohoadonpage;
        }
    }
}


