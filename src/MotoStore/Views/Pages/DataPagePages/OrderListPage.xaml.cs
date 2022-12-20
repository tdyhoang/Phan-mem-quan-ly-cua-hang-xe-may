using MotoStore.Database;
using MotoStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using Wpf.Ui.Common.Interfaces;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class OrderListPage : INavigableView<ViewModels.OrderListViewModel>
    {
        public ViewModels.OrderListViewModel ViewModel
        {
            get;
        }

        public OrderListPage(ViewModels.OrderListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
            grdOrder.ItemsSource = ViewModel.TableData;
        }
    }
}