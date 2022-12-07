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
using Wpf.Ui.Mvvm.Contracts;
using System.Windows.Input;
using MotoStore.Services.Contracts;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace MotoStore.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly IWindowService _windowService;
        //ĐẶT BIẾN BOOL

        private ICommand _openWindowCommand;

        public ICommand OpenWindowCommand => _openWindowCommand ??= new RelayCommand<string>(OnOpenWindow);

        public DashboardViewModel(INavigationService navigationService, IWindowService windowService)
        {
            _navigationService = navigationService;
            _windowService = windowService;
        }

        public void OnNavigatedTo()
        {
            //Check here
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnOpenWindow(string parameter)
        {
            switch (parameter)
            {
                case "open_login_window":
                    _windowService.Show<Views.Windows.LoginWindow>();
                    return;
            }
        }



        /*  public ISeries[] Chart1 { get; set; }
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
          };*/
    }
}
