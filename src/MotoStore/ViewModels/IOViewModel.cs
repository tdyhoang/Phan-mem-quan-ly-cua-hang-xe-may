using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using MotoStore.Views.Pages.IOPagePages;

namespace MotoStore.ViewModels
{
    public partial class IOViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;

        public IOViewModel(INavigationService navigationService) => _navigationService = navigationService;

        [ObservableProperty]
        private int _counter = 0;
        internal IOSanPhamPage iosppage=new();
        internal IOHoaDonPage iohoadonpage=new();
        internal IOKhachHangPage iokhachhangpage = new();
        internal IONhaSXPage ionhasxpage = new();
        public void OnNavigatedTo()
        {
              
        }

        public void OnNavigatedFrom()
        {
            //MessageBox.Show("SIUUU FROM");
        }
    }
}

