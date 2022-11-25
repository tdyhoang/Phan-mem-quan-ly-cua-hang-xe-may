using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using System.Windows;
using System.Linq;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using MotoStore.Databases;
using MotoStore.Models;
using System.Collections.Generic;

namespace MotoStore.ViewModels
{
    public partial class MotoListViewModel : ObservableObject, INavigationAware
    {
        public List<MatHang> TableData;

        public void OnNavigatedTo()
        {
            try
            {
                MainDatabase con = new MainDatabase();
                TableData = con.MatHangs.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}