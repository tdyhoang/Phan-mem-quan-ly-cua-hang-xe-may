using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace MotoStore.ValidationRules
{
    public class DateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(true, default);
            if (DateTime.TryParseExact(value.ToString(), "d/M/yyyy", cultureInfo, DateTimeStyles.AllowWhiteSpaces, out _))
                return new(true, default);

            return new(false, "Nhập ngày theo định dạng dd/MM/yyyy");
        }
    }
}
