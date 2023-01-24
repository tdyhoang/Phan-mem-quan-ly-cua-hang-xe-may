using DataGridExtensions;
using System;
using System.Windows;

namespace MotoStore.Controls
{
    /// <summary>
    /// Interaction logic for DateFilter.xaml
    /// </summary>
    public partial class DateFilter
    {
        public DateFilter() => InitializeComponent();

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        /// <summary>
        /// Biến dùng cho textblock caption
        /// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(DateFilter), new FrameworkPropertyMetadata("Nhập giới hạn:", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DateTime Minimum
        {
            get => (DateTime)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn dưới (minimum)
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(DateTime), typeof(DateFilter), new FrameworkPropertyMetadata(default(DateTime), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DateFilter)sender).Range_Changed()));

        public DateTime Maximum
        {
            get => (DateTime)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn trên (maximum)
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(DateTime), typeof(DateFilter), new FrameworkPropertyMetadata(default(DateTime), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DateFilter)sender).Range_Changed()));


        public bool IsPopupVisible
        {
            get => (bool)GetValue(IsPopupVisibleProperty);
            set => SetValue(IsPopupVisibleProperty, value);
        }
        /// <summary>
        /// Biến IsPopupVisible quyết định filter ẩn hay hiện
        /// </summary>
        public static readonly DependencyProperty IsPopupVisibleProperty = DependencyProperty.Register("IsPopupVisible", typeof(bool), typeof(DateFilter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void Range_Changed()
        {
            Filter = Maximum >= Minimum ? new DateContentFilter(Minimum, Maximum) : null;
        }

        public IContentFilter? Filter
        {
            get => (IContentFilter?)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }
        /// <summary>
        /// Biến Filter dùng để lọc data
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IContentFilter), typeof(DateFilter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((DateFilter)sender).Filter_Changed()));


        private void Filter_Changed()
        {
            if (Filter is not DateContentFilter filter)
                return;

            Minimum = filter.Min;
            Maximum = filter.Max;
        }

    }

    public partial class DateContentFilter : IContentFilter
    {
        public DateContentFilter(DateTime min, DateTime max)
        {
            Min = min;
            Max = max;
        }

        public DateTime Min { get; }

        public DateTime Max { get; }

        public bool IsMatch(object? value)
        {
            if (value == null)
                return false;

            if (!DateTime.TryParse(value.ToString(), out var number))
                return false;

            if (Max == default)
                return true;

            return (number >= Min) && (number <= Max);
        }
    }
}
