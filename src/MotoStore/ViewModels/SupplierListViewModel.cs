using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using System.Windows;
using System.Linq;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MotoStore.Models;
using System.Collections.Generic;
using MotoStore.Database;

namespace MotoStore.ViewModels
{
    public partial class SupplierListViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }
    }
}