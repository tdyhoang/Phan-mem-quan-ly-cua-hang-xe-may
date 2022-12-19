using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Windows.Navigation;
using DataGridExtensions;
using MotoStore.Databases;
using System.Collections.Generic;
using MotoStore.Models;
using System.Linq;
using System.Windows;

namespace MotoStore.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject, INavigationAware
    {
        public List<NhanVien> TableData;

        public void OnNavigatedTo()
        {
            try
            {
                MainDatabase con = new MainDatabase();
                TableData = con.NhanViens.ToList();
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