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
using MotoStore.Databases;
using MotoStore.Models;
using System.Collections.Generic;

namespace MotoStore.ViewModels
{
    public partial class CustomerListViewModel : ObservableObject, INavigationAware
    {
        public List<KhachHang> TableData;

        public void OnNavigatedTo()
        {
            try
            {
                MainDatabase con = new MainDatabase();
                TableData = con.KhachHangs.ToList();
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