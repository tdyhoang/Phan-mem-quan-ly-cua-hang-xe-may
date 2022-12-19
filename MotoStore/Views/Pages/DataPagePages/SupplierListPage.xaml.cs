using System.Data;
using System.Windows;
using System.Linq;
using Wpf.Ui.Common.Interfaces;
using MotoStore.Databases;
using MotoStore.Models;
using System.Collections.Generic;
using System;

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

            try
            {
                MainDatabase con = new MainDatabase();
                List<NhaSanXuat> TableData = con.NhaSanXuats.ToList();
                grdSupplier.ItemsSource = TableData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
