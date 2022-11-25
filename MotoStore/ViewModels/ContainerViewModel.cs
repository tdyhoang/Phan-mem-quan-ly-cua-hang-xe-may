using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoStore.Models;
using MotoStore.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using MessageBox = System.Windows.MessageBox;

namespace MotoStore.ViewModels
{
    public partial class ContainerViewModel : ObservableObject
    {
        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand SuplierCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ObjectCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand InputCommand { get; set; }
        public ICommand OutputCommand { get; set; }
        private bool _isInitialized = false;

        [ObservableProperty]
        private IEnumerable<DataColor> _colors;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public ContainerViewModel()
        {
            // Login Window
            /*LoadedWindowCommand = new RelayCommand<Window>((p) => {
                Isloaded = true;
                if (p == null)
                    return;
                p.Hide();
                LoginViewModel loginViewModel = new LoginViewModel();
                LoginWindow loginWindow = new LoginWindow(loginViewModel);
                bool? isOpened = loginWindow.ShowDialog();
                MessageBox.Show(isOpened.ToString());

                var loginVM = loginWindow.DataContext as LoginViewModel;

                if (loginVM.IsLogin)
                {
                    if (!_isInitialized)
                        InitializeViewModel();
                    p.Show();
                }
                else
                {
                    p.Hide();
                }
            }
            );*/
            //MessageBox.Show(DataProvider.Ins.DB.Users.First().DisplayName);

            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Phần mềm quản lý cửa hàng xe máy";

            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Dashboard",
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationItem()
                {
                    Content = "Nhập xuất",
                    PageTag = "io",
                    Icon = SymbolRegular.Stream24,
                    PageType = typeof(Views.Pages.IOPage)
                },
                new NavigationItem()
                {
                    Content = "Danh mục",
                    PageTag = "data",
                    Icon = SymbolRegular.DataHistogram24,
                    PageType = typeof(Views.Pages.DataPage)
                },
                new NavigationItem()
                {
                    Content = "Báo cáo",
                    PageTag = "report",
                    Icon = SymbolRegular.Info24,
                    PageType = typeof(Views.Pages.ReportPage)
                }
            };

            NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Cài đặt",
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
