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
            IOMainFr.Content = ViewModel.iosppage; //Khởi tạo trang này sẽ gán Content của trang chính là trang IOSP
        }

        private void btnSPPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMainFr.Content = ViewModel.iosppage;
        }

        private void btnAddNSXPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMainFr.Content = ViewModel.ionhasxpage;
        }

        private void btnAddKhachHang_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMainFr.Content = ViewModel.iokhachhangpage;
        }

        private void btnAddHoaDon_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMainFr.Content = ViewModel.iohoadonpage;
        }

        private void btnAddPhieuBH_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMainFr.Content = ViewModel.iophieubaohanhpage;
        }
    }
}


