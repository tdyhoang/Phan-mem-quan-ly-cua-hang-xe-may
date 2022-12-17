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
    public partial class UserListPage : INavigableView<ViewModels.UserListViewModel>
    {
        public ViewModels.UserListViewModel ViewModel
        {
            get;
        }

        public UserListPage(ViewModels.UserListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
            grdUser.ItemsSource = ViewModel.TableData;
        }
    }
}