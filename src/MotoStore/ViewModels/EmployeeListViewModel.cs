﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Windows.Navigation;
using DataGridExtensions;
using System.Collections.Generic;
using MotoStore.Models;
using System.Linq;
using System.Windows;
using MotoStore.Database;

namespace MotoStore.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }
    }
}