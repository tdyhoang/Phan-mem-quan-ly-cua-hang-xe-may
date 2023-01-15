using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    public class HoTenValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length > 30)
                    return new(false, "Tên quá dài, tối đa 30 ký tự!");
                if (!value.ToString().ToCharArray().All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    return new(false, "Tên không được chứa số hoặc ký tự đặc biệt!");
            }

            return new(true, default);
        }
    }
}
