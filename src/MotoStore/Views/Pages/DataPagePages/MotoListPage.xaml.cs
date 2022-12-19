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

            ViewModel.OnNavigatedTo();
            grdMoto.ItemsSource = ViewModel.TableData;
        }
    }
}