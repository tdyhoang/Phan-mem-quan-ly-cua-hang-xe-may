﻿using MotoStore.Database;
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
    public partial class EmployeeListPage : INavigableView<ViewModels.EmployeeListViewModel>
    {
        public ViewModels.EmployeeListViewModel ViewModel
        {
            get;
        }

        public EmployeeListPage(ViewModels.EmployeeListViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            ViewModel.OnNavigatedTo();
            grdEmployee.ItemsSource = ViewModel.TableData;
        }
    }
}