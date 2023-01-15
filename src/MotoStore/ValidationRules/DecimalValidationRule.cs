using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    internal class DecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()) || decimal.TryParse(value.ToString(), out _))
                return new(true, default);

            return new(false, "Số quá dài hoặc quá lớn!");
        }
    }
}
