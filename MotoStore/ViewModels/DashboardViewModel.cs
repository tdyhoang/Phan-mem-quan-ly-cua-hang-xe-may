using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;
using System.Threading.Tasks;
using MotoStore.Properties;

namespace MotoStore.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private int _counter = 0;
        private string _txtServerName;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
            if (App.Current.Properties["IsConnected"] is null || !(bool)App.Current.Properties["IsConnected"])
            {
                MessageBox.Show("Vui lòng kết nối tới server CSDL trước khi tiếp tục!");
                // Cancel navigating
            }
        }

        public string TxtServerName
        {
            get => _txtServerName;
            set => SetProperty(ref _txtServerName, value);
        }

        [RelayCommand]
        private void ConnectToServer()
        {
            string OldServerName = ConfigurationManager.ConnectionStrings["ServerName"].ConnectionString;
            string NewServerName = TxtServerName;
            string OldConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            string NewConString = OldConString.Replace("Data Source=" + OldServerName, "Data Source=" + NewServerName);

            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.ConnectionStrings.ConnectionStrings["ServerName"].ConnectionString = NewServerName;
            cfa.ConnectionStrings.ConnectionStrings["ConString"].ConnectionString = NewConString;
            cfa.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            var csb = new SqlConnectionStringBuilder(NewConString);
            csb.ConnectTimeout = 5;

            try
            {
                using(var con = new SqlConnection(NewConString))
                {
                    con.Open();
                    con.Close();
                }
                System.Windows.MessageBox.Show("Kết nối thành công!");
                App.Current.Properties["IsConnected"] = true;
            }
            catch
            {
                System.Windows.MessageBox.Show("Sai tên server CSDL, vui lòng nhập lại!");
                App.Current.Properties["IsConnected"] = false;
            }
        }

        public ISeries[] Chart1 { get; set; }
            = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
            };

        public ISeries[] Chart2 { get; set; }
            = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
            };
        public ISeries[] Chart3 { get; set; }
            = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
            };
        public ISeries[] Chart4 { get; set; }
            = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
                },
            };

        public List<Axis> XAxes1 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labels = new string[] { "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10" }
            }
        };
        public List<Axis> XAxes2 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labels = new string[] { "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10" }
            }
        };
        public List<Axis> XAxes3 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labels = new string[] { "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10" }
            }
        };
        public List<Axis> XAxes4 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labels = new string[] { "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10" }
            }
        };

        public List<Axis> YAxes1 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labeler = Labelers.Currency
            }
        };
        public List<Axis> YAxes2 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labeler = Labelers.Currency
            }
        };
        public List<Axis> YAxes3 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labeler = Labelers.Currency
            }
        };
        public List<Axis> YAxes4 { get; set; }
        = new List<Axis>
        {
            new Axis
            {
                Labeler = Labelers.Currency
            }
        };
    }
}
