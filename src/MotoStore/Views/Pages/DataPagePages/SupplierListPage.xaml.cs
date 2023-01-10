using System.Data;
using System.Windows;
using System.Linq;
using Wpf.Ui.Common.Interfaces;
using MotoStore.Models;
using System.Collections.Generic;
using System;
using MotoStore.Database;

namespace MotoStore.Views.Pages.DataPagePages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class SupplierListPage : INavigableView<ViewModels.SupplierListViewModel>
    {
        public ViewModels.SupplierListViewModel ViewModel
        {
            get;
        }

        public SupplierListPage(ViewModels.SupplierListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.OnNavigatedTo();
            grdSupplier.ItemsSource = ViewModel.TableData;
        }

        private void UiPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}