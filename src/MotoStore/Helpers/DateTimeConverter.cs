using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MotoStore.Helpers
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime test)
            {
                if (test == DateTime.MinValue)
                {
                    return string.Empty;
                }
                var date = test.ToString("dd/MM/yyyy");
                return (date);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string test)
            {
                if (string.IsNullOrEmpty(test))
                    return DateTime.MinValue;

                if (DateTime.TryParseExact(test, "d/M/yyyy", culture, DateTimeStyles.AllowWhiteSpaces, out _))
                    return DateTime.ParseExact(test, "d/M/yyyy", culture, DateTimeStyles.AllowWhiteSpaces);
            }

            return DateTime.MinValue;
        }
    }
}
