using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    public class SDTValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length > 10)
                    return new(false, "SĐT quá dài, tối đa 10 ký tự!");
                if (!value.ToString().ToCharArray().All(Char.IsDigit))
                    return new(false, "SĐT chỉ được chứa các ký tự số!");
            }

            return new(true, default);
        }
    }
}
