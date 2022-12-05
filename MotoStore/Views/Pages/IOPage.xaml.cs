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

        private void btnAddSPPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content=new IOPagePages.IOSanPhamPage();
        }

        private void btnAddNVPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content= new IOPagePages.IONhanVienPage();
        }

        private void btnAddNSXPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content=new IOPagePages.IONhaSXPage();
        }

        private void btnAddKhachHang_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IOMain.Content = new IOPagePages.IOKhachHangPage();
        }
    }
}