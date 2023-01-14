using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    public class DiaChiValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 40)
                    return new(false, "Địa chỉ quá dài, tối đa 40 ký tự!");

            return new(true, default);
        }
    }
}
