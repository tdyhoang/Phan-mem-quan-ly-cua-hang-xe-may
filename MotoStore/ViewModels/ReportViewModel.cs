using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;

namespace MotoStore.ViewModels
{
    public partial class ReportViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;

        public ReportViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private int _counter = 0;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }
    }
}