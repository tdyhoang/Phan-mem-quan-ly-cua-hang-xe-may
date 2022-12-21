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

namespace MotoStore.ViewModels
{
    public partial class CustomerListViewModel : ObservableObject, INavigationAware
    {
        internal List<KhachHang> TableData;

        public void OnNavigatedTo()
        {
            RefreshDataGrid();
        }

        public void OnNavigatedFrom()
        {
        }

        private void RefreshDataGrid()
        {
            MainDatabase con = new MainDatabase();
            TableData = con.KhachHangs.ToList();
        }
    }
}