using MotoStore.Database;
using MotoStore.Models;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using System.Diagnostics;
using Wpf.Ui.Common.Interfaces;
using System.Windows.Data;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class CustomerListPage : INavigableView<ViewModels.CustomerListViewModel>
    {
        public ViewModels.CustomerListViewModel ViewModel
        {
            get;
        }

        public CustomerListPage(ViewModels.CustomerListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            CollectionViewSource customerCollectionViewSource = (CollectionViewSource)(FindResource("CustomerCollectionViewSource"));
            customerCollectionViewSource.Source = ViewModel.TableData;
        }

        private void SaveToDatabase(object sender, RoutedEventArgs e)
        {
            foreach (KhachHang kh in grdCustomer.Items)
            {
                //if (kh.MaKh == "")
            }
        }
    }
}