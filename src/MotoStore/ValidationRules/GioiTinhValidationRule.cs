using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    public class GioiTinhValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == "Nam" || value.ToString() == "Nữ")
                return new(true, default);

            return new(false, "Giới tính phải là Nam hoặc Nữ!");
        }
    }
}
