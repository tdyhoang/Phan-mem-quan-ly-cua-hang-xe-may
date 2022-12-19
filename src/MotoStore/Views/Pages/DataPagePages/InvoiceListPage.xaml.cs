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
    public partial class InvoiceListPage : INavigableView<ViewModels.InvoiceListViewModel>
    {
        public ViewModels.InvoiceListViewModel ViewModel
        {
            get;
        }

        public InvoiceListPage(ViewModels.InvoiceListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
            grdInvoice.ItemsSource = ViewModel.TableData;
        }
    }
}