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
    public partial class MotoListPage : INavigableView<ViewModels.MotoListViewModel>
    {
        public ViewModels.MotoListViewModel ViewModel
        {
            get;
        }

        public MotoListPage(ViewModels.MotoListViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            try
            {
                MainDatabase con = new MainDatabase();
                List<MatHang> TableData = con.MatHangs.ToList();
                grdMoto.ItemsSource = TableData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
