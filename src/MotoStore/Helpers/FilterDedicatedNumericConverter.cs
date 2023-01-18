using System;
using System.Globalization;
using System.Windows.Data;

namespace MotoStore.Helpers
{
    internal class FilterDedicatedNumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal dt)
                return dt.ToString();
            if (value is int i)
                return i.ToString();

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return targetType switch
            {
                Type intType when intType == typeof(int) => int.TryParse(value.ToString(), out int i) ? i : default,
                Type decimalType when decimalType == typeof(decimal) => (object)(decimal.TryParse(value.ToString(), out decimal d) ? d : default),
                _ => throw new ArgumentException("Undefined Type"),
            };
        }
    }
}
