using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MotoStore.ValidationRules
{
    public class LoaiKhValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString() == "Vip" || value.ToString() == "Thân quen" || value.ToString() == "Thường")
                return new(true, default);

            return new(false, "Loại khách hàng phải là Vip, Thân quen hoặc Thường!");
        }
    }
}
