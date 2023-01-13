using DataGridExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotoStore.Controls
{
    /// <summary>
    /// Interaction logic for DecimalFilter.xaml
    /// </summary>
    public partial class DecimalFilter
    {
        public DecimalFilter()
        {
            InitializeComponent();
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        /// <summary>
        /// Biến dùng cho textblock caption
        /// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(DecimalFilter), new FrameworkPropertyMetadata("Nhập giới hạn:", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public decimal Minimum
        {
            get => (decimal)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn dưới (minimum)
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(DecimalFilter), new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DecimalFilter)sender).Range_Changed()));

        public decimal Maximum
        {
            get => (decimal)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn trên (maximum)
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(DecimalFilter), new FrameworkPropertyMetadata(default(decimal), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DecimalFilter)sender).Range_Changed()));


        public bool IsPopupVisible
        {
            get => (bool)GetValue(IsPopupVisibleProperty);
            set => SetValue(IsPopupVisibleProperty, value);
        }
        /// <summary>
        /// Biến IsPopupVisible quyết định filter ẩn hay hiện
        /// </summary>
        public static readonly DependencyProperty IsPopupVisibleProperty = DependencyProperty.Register("IsPopupVisible", typeof(bool), typeof(DecimalFilter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void Range_Changed()
        {
            Filter = Maximum >= Minimum ? new DecimalContentFilter(Minimum, Maximum) : null;
        }

        public IContentFilter? Filter
        {
            get => (IContentFilter?)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }
        /// <summary>
        /// Biến Filter dùng để lọc data
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IContentFilter), typeof(DecimalFilter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DecimalFilter)sender).Filter_Changed()));


        private void Filter_Changed()
        {
            if (Filter is not DecimalContentFilter filter)
                return;

            Minimum = filter.Min;
            Maximum = filter.Max;
        }

    }

    public partial class DecimalContentFilter : IContentFilter
    {
        public DecimalContentFilter(decimal min, decimal max)
        {
            Min = min;
            Max = max;
        }

        public decimal Min { get; }

        public decimal Max { get; }

        public bool IsMatch(object? value)
        {
            if (value == null)
                return false;

            if (!decimal.TryParse(value.ToString(), out var number))
                return false;

            return (number >= Min) && (number <= Max);
        }
    }
}
