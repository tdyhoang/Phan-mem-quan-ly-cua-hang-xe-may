using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using System.Windows;
using System.Linq;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MotoStore.Database;
using MotoStore.Models;
using System.Collections.Generic;

namespace MotoStore.ViewModels
{
    public partial class MotoListViewModel : ObservableObject, INavigationAware
    {
        public List<MatHang> TableData;

        public void OnNavigatedTo()
        {
            RefreshDataGrid();
        }

        public void OnNavigatedFrom()
        {
        }

        private void RefreshDataGrid()
        {
            try
            {
                MainDatabase con = new();
                TableData = con.MatHangs.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}