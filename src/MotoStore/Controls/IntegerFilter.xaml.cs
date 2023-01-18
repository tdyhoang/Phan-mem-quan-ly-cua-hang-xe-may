using DataGridExtensions;
using System.Windows;

namespace MotoStore.Controls
{
    /// <summary>
    /// Interaction logic for IntegerFilter.xaml
    /// </summary>
    public partial class IntegerFilter
    {
        public IntegerFilter()
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
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(IntegerFilter), new FrameworkPropertyMetadata("Nhập giới hạn:", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int Minimum
        {
            get => (int)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn dưới (minimum)
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerFilter), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((IntegerFilter)sender).Range_Changed()));

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        /// <summary>
        /// Biến giá trị giới hạn trên (maximum)
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerFilter), new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((IntegerFilter)sender).Range_Changed()));


        public bool IsPopupVisible
        {
            get => (bool)GetValue(IsPopupVisibleProperty);
            set => SetValue(IsPopupVisibleProperty, value);
        }
        /// <summary>
        /// Biến IsPopupVisible quyết định filter ẩn hay hiện
        /// </summary>
        public static readonly DependencyProperty IsPopupVisibleProperty = DependencyProperty.Register("IsPopupVisible", typeof(bool), typeof(IntegerFilter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void Range_Changed()
        {
            Filter = Maximum >= Minimum ? new IntegerContentFilter(Minimum, Maximum) : null;
        }

        public IContentFilter? Filter
        {
            get => (IContentFilter?)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }
        /// <summary>
        /// Biến Filter dùng để lọc data
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(IContentFilter), typeof(IntegerFilter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, _) => ((IntegerFilter)sender).Filter_Changed()));


        private void Filter_Changed()
        {
            if (Filter is not IntegerContentFilter filter)
                return;

            Minimum = filter.Min;
            Maximum = filter.Max;
        }

    }

    public class IntegerContentFilter : IContentFilter
    {
        public IntegerContentFilter(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }

        public int Max { get; }

        public bool IsMatch(object? value)
        {
            if (value == null)
                return false;

            if (!int.TryParse(value.ToString(), out var number))
                return false;

            if (Max == default && Min == default)
                return true;
            if (Max == default)
                return number >= Min;
            if (Min == default)
                return number <= Max;

            return (number >= Min) && (number <= Max);
        }
    }
}
