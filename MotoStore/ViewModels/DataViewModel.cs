using CommunityToolkit.Mvvm.ComponentModel;
using MotoStore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls;
using System.Windows.Navigation;
using Wpf.Ui.Controls.Navigation;
using System.ComponentModel.Design.Serialization;
using System.Windows.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace MotoStore.ViewModels
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;

        public DataViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private IEnumerable<DataColor> _colors;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        public void OnNavigatedTo()
        {
            InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Danh sách xe",
                    PageTag = "motolist",
                    Icon = SymbolRegular.TextBulletListSquare20,
                    PageType = typeof(Views.Pages.DataPagePages.MotoListPage)
                },
                new NavigationItem()
                {
                    Content = "Danh sách nhân viên",
                    PageTag = "employeelist",
                    Icon = SymbolRegular.ContactCard20,
                    PageType = typeof(Views.Pages.DataPagePages.EmployeeListPage)
                },
                new NavigationItem()
                {
                    Content = "Danh sách khách hàng",
                    PageTag = "customerlist",
                    Icon = SymbolRegular.PeopleQueue20,
                    PageType = typeof(Views.Pages.DataPagePages.CustomerListPage)
                },
                new NavigationItem()
                {
                    Content = "Nhà cung cấp",
                    PageTag = "supplierlist",
                    Icon = SymbolRegular.PeopleCall20,
                    PageType = typeof(Views.Pages.DataPagePages.SupplierListPage)
                }
            };
        }
    }
}
