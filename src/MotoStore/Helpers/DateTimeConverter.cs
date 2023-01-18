using System;
using System.Globalization;
using System.Windows.Data;

namespace MotoStore.Helpers
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                if (dt == default)
                    return default(string);
                var date = dt.ToString("dd/MM/yyyy");
                return date;
            }

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dt)
            {
                if (string.IsNullOrEmpty(dt))
                    return default(DateTime);

                if (DateTime.TryParseExact(dt, "d/M/yyyy", culture, DateTimeStyles.AllowWhiteSpaces, out DateTime result))
                    return result;
            }

            return default(DateTime);
        }
    }
}
