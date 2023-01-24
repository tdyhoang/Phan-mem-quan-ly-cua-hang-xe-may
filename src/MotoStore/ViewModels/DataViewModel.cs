using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using MotoStore.Views.Pages.LoginPages;

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
        private ObservableCollection<INavigationControl> _navigationItems = new();

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            if (PageChinh.getChucVu.ToLower() == "quản lý")
            {
                NavigationItems = new()
                {
                    new NavigationItem()
                    {
                        Content = "Danh Sách Mặt Hàng",
                        PageTag = "motolist",
                        Icon = SymbolRegular.VehicleBicycle24,
                        PageType = typeof(Views.Pages.DataPagePages.MotoListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Nhân Viên",
                        PageTag = "employeelist",
                        Icon = SymbolRegular.ContactCard24,
                        PageType = typeof(Views.Pages.DataPagePages.EmployeeListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Khách Hàng",
                        PageTag = "customerlist",
                        Icon = SymbolRegular.PeopleCommunity24,
                        PageType = typeof(Views.Pages.DataPagePages.CustomerListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Hóa Đơn",
                        PageTag = "invoicelist",
                        Icon = SymbolRegular.TextBulletListSquare24,
                        PageType = typeof(Views.Pages.DataPagePages.InvoiceListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Nhà Cung Cấp",
                        PageTag = "supplierlist",
                        Icon = SymbolRegular.PeopleTeamToolbox24,
                        PageType = typeof(Views.Pages.DataPagePages.SupplierListPage)
                    },
                    new NavigationItem()
                    {
                    Content = "Danh Sách Đơn Đặt Hàng",
                    PageTag = "orderlist",
                    Icon = SymbolRegular.DocumentBulletListClock24,
                    PageType = typeof(Views.Pages.DataPagePages.OrderListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Phiếu Bảo Hành",
                        PageTag = "maintenancelist",
                        Icon = SymbolRegular.VideoPersonCall24,
                        PageType = typeof(Views.Pages.DataPagePages.MaintenanceListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Tài Khoản",
                        PageTag = "userlist",
                        Icon = SymbolRegular.PersonCircle24,
                        PageType = typeof(Views.Pages.DataPagePages.UserListPage)
                    }
                };
            }
            else
            {
                NavigationItems = new()
                {
                    new NavigationItem()
                    {
                        Content = "Danh Sách Mặt Hàng",
                        PageTag = "motolist",
                        Icon = SymbolRegular.VehicleBicycle24,
                        PageType = typeof(Views.Pages.DataPagePages.MotoListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Khách Hàng",
                        PageTag = "customerlist",
                        Icon = SymbolRegular.PeopleCommunity24,
                        PageType = typeof(Views.Pages.DataPagePages.CustomerListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Hóa Đơn",
                        PageTag = "invoicelist",
                        Icon = SymbolRegular.TextBulletListSquare24,
                        PageType = typeof(Views.Pages.DataPagePages.InvoiceListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Nhà Cung Cấp",
                        PageTag = "supplierlist",
                        Icon = SymbolRegular.PeopleTeamToolbox24,
                        PageType = typeof(Views.Pages.DataPagePages.SupplierListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Đơn Đặt Hàng",
                        PageTag = "orderlist",
                        Icon = SymbolRegular.DocumentBulletListClock24,
                        PageType = typeof(Views.Pages.DataPagePages.OrderListPage)
                    },
                    new NavigationItem()
                    {
                        Content = "Danh Sách Phiếu Bảo Hành",
                        PageTag = "maintenancelist",
                        Icon = SymbolRegular.VideoPersonCall24,
                        PageType = typeof(Views.Pages.DataPagePages.MaintenanceListPage)
                    }
                };
            }

            _navigationService.Navigate(typeof(Views.Pages.DataPagePages.MotoListPage));
        }

        internal void VisibleChanged()
        {
            InitializeViewModel();
        }
    }
}