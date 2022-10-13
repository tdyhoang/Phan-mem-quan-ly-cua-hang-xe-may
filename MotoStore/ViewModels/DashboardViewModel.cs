using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;
using System.Collections.Generic;

namespace MotoStore.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private int _counter = 0;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        [ICommand]
        private void OnCounterIncrement()
        {
            Counter++;
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
