using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Linq;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Drawing;
using Wpf.Ui.Mvvm.Interfaces;
using MotoStore.Models;
using System.Collections.Generic;
using MotoStore.Database;
using System.Windows.Controls;

namespace MotoStore.ViewModels
{
    public partial class CustomerListViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }
    }
}