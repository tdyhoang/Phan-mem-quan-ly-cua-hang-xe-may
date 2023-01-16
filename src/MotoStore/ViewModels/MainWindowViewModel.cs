using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace MotoStore.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();


        public MainWindowViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
            {
                InitializeViewModel();             
            }
        }

        private void InitializeViewModel()      
        {
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Trang Chủ",
                    PageTag = "trangchu",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationItem()
                {
                    Content = "Nhập Xuất",
                    PageTag = "io",
                    Icon = SymbolRegular.Stream24,
                    PageType = typeof(Views.Pages.IOPage)
                },
                new NavigationItem()
                {
                    Content = "Danh Mục",
                    PageTag = "data",
                    Icon = SymbolRegular.DataHistogram24,
                    PageType = typeof(Views.Pages.DataPage)
                },
                new NavigationItem()
                {
                    Content = "Báo Cáo",
                    PageTag = "report",
                    Icon = SymbolRegular.Info24,
                    PageType = typeof(Views.Pages.ReportPage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Cài Đặt",
                    PageTag = "settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(Views.Pages.SettingsPage)
                }
            };

            TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };

            _isInitialized = true;
        }
    }
}
