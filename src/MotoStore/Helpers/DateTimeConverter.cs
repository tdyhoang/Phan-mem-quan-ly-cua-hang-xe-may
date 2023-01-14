using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MotoStore.Helpers
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime test)
            {
                if (value == default)
                    return default(string);
                var date = test.ToString("dd/MM/yyyy");
                return (date);
            }

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string test)
            {
                if (string.IsNullOrEmpty(test))
                    return default(DateTime);

                if (DateTime.TryParseExact(test, "d/M/yyyy", culture, DateTimeStyles.AllowWhiteSpaces, out DateTime result))
                    return result;
            }

            return default(DateTime);
        }
    }
}
