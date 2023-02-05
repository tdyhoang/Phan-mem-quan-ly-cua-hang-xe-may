using MotoStore.Database;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MotoStore.Helpers
{
    public class CustomValidationRule : ValidationRule
    {
        public CustomValidationRule()
        {
            ValidationMode = default;
            IsNullable = default;
        }

        public ValidationRules ValidationMode { get; set; }
        public bool IsNullable { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is null && IsNullable)
                return new(false, "Không được để trống!");
            value ??= string.Empty;
            return ValidationMode switch
            {
                ValidationRules.None => new(true, default),
                ValidationRules.ChucVuValidation => ChucVuValidation(value),
                ValidationRules.DateValidation => DateValidation(value, cultureInfo),
                ValidationRules.DecimalValidation => DecimalValidation(value),
                ValidationRules.DiaChiValidation => DiaChiValidation(value),
                ValidationRules.EmailValidation => EmailValidation(value),
                ValidationRules.GhiChuValidation => GhiChuValidation(value),
                ValidationRules.GioiTinhValidation => GioiTinhValidation(value),
                ValidationRules.HoTenValidation => HoTenValidation(value),
                ValidationRules.LoaiKhValidation => LoaiKhValidation(value),
                ValidationRules.MaHdValidation => MaHdValidation(value),
                ValidationRules.MaKhValidation => MaKhValidation(value),
                ValidationRules.MaMhValidation => MaMhValidation(value),
                ValidationRules.MaNccValidation => MaNccValidation(value),
                ValidationRules.MaNvValidation => MaNvValidation(value),
                ValidationRules.MauValidation => MauValidation(value),
                ValidationRules.MoTaValidation => MoTaValidation(value),
                ValidationRules.PasswordValidation => PasswordValidation(value),
                ValidationRules.SDTValidation => SDTValidation(value),
                ValidationRules.TenValidation => TenValidation(value),
                ValidationRules.UsernameValidation => UsernameValidation(value),
                _ => throw new("Unknown ValidationRule"),
            };
        }

        private static ValidationResult ChucVuValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 10)
                    return new(false, "Chức vụ quá dài, tối đa 10 ký tự!");

            return new(true, default);
        }

        private static ValidationResult DateValidation(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(true, default);
            if (DateTime.TryParseExact(value.ToString(), "d/M/yyyy", cultureInfo, DateTimeStyles.AllowWhiteSpaces, out var date))
            {
                if (date < new DateTime(1900, 1, 1) || date > new DateTime(2079, 6, 6))
                    return new(false, "Ngày chỉ được nằm trong khoảng từ 01/01/1900 đến 06/06/2079");
                return new(true, default);
            }
            return new(false, "Nhập ngày hợp lệ, có trên theo định dạng dd/MM/yyyy");
        }

        private static ValidationResult DecimalValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(true, default);
            if (decimal.TryParse(value.ToString(), out _))
                return new(true, default);
            return new(false, "Nhập số thập phân hợp lệ!");
        }

        private static ValidationResult DiaChiValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 40)
                    return new(false, "Địa chỉ quá dài, tối đa 40 ký tự!");

            return new(true, default);
        }

        private static ValidationResult EmailValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()) || MailAddress.TryCreate(value.ToString(), out _))
                return new(true, default);

            return new(false, "Email sai định dạng!");
        }

        private static ValidationResult GhiChuValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 60)
                    return new(false, "Ghi chú quá dài, tối đa 60 ký tự!");

            return new(true, default);
        }

        private static ValidationResult GioiTinhValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Giới tính không được để trống!");
            if (value.ToString() == "Nam" || value.ToString() == "Nữ")
                return new(true, default);

            return new(false, "Giới tính phải là Nam hoặc Nữ (có dấu)!");
        }

        private static ValidationResult HoTenValidation(object value)
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

        private static ValidationResult LoaiKhValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Loại khách hàng không được để trống!");
            if (value.ToString() == "Vip" || value.ToString() == "Thân quen" || value.ToString() == "Thường")
                return new(true, default);

            return new(false, "Loại khách hàng phải là Vip, Thân quen hoặc Thường (có dấu)!");
        }

        private static ValidationResult MaHdValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã hóa đơn không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^HD\d{3}$"))
                return new(false, "Mã hóa đơn phải theo cú pháp HD***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            if (mdb.HoaDons.Any(hd => hd.MaHd == value.ToString()))
                return new(true, default);

            return new(false, "Mã hóa đơn không tồn tại hoặc đã xóa");
        }

        private static ValidationResult MaKhValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã khách hàng không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^KH\d{3}$"))
                return new(false, "Mã khách hàng phải theo cú pháp KH***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            if (mdb.KhachHangs.Any(kh => kh.MaKh == value.ToString()))
                return new(true, default);

            return new(false, "Mã khách hàng không tồn tại hoặc đã xóa");
        }

        private static ValidationResult MaMhValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã mặt hàng không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^MH\d{3}$"))
                return new(false, "Mã mặt hàng phải theo cú pháp MH***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            if (mdb.MatHangs.Any(mh => mh.MaMh == value.ToString()))
                return new(true, default);

            return new(false, "Mã mặt hàng không tồn tại hoặc đã xóa");
        }

        private static ValidationResult MaNccValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã mặt hàng không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^CC\d{3}$"))
                return new(false, "Mã nhà cung cấp phải theo cú pháp CC***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            if (mdb.NhaCungCaps.Any(ncc => ncc.MaNcc == value.ToString()))
                return new(true, default);

            return new(false, "Mã mặt hàng không tồn tại hoặc đã xóa");
        }

        private static ValidationResult MaNvValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã nhân viên không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^NV\d{3}$"))
                return new(false, "Mã nhân viên phải theo cú pháp NV***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            if (mdb.NhanViens.Any(nv => nv.MaNv == value.ToString()))
                return new(true, default);

            return new(false, "Mã nhân viên không tồn tại hoặc đã xóa");
        }

        private static ValidationResult MauValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 15)
                    return new(false, "Màu quá dài, tối đa 15 ký tự!");

            return new(true, default);
        }

        private static ValidationResult MoTaValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 75)
                    return new(false, "Mô tả quá dài, tối đa 75 ký tự!");

            return new(true, default);
        }

        private static ValidationResult PasswordValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length < 8)
                    return new(false, "Password quá ngắn, tối thiểu 8 ký tự!");
                if (value.ToString().Length > 30)
                    return new(false, "Password quá dài, tối đa 30 ký tự!");
            }

            return new(true, default);
        }

        private static ValidationResult SDTValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length > 30)
                    return new(false, "SĐT quá dài, tối đa 30 ký tự!");
                string sdt = Regex.Replace(value.ToString(), @"[^0-9]+", string.Empty);
                if (sdt.Length < 8 || sdt.Length > 15)
                    return new(false, "SĐT chỉ được chứa từ 8 đến 15 ký tự số!");
            }

            return new(true, default);
        }

        private static ValidationResult TenValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 30)
                    return new(false, "Tên quá dài, tối đa 30 ký tự!");

            return new(true, default);
        }

        private static ValidationResult UsernameValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length < 5)
                    return new(false, "Username quá ngắn, tối thiểu 5 ký tự!");
                if (value.ToString().Length > 20)
                    return new(false, "Username quá dài, tối đa 20 ký tự!");
                if (!value.ToString().ToCharArray().All(char.IsLetterOrDigit))
                    return new(false, "Username chỉ bao gồm ký tự chữ cái hoặc số!");
            }

            return new(true, default);
        }

        public enum ValidationRules
        {
            None,
            ChucVuValidation,
            DateValidation,
            DecimalValidation,
            DiaChiValidation,
            EmailValidation,
            GhiChuValidation,
            GioiTinhValidation,
            HoTenValidation,
            LoaiKhValidation,
            MaHdValidation,
            MaKhValidation,
            MaMhValidation,
            MaNccValidation,
            MaNvValidation,
            MauValidation,
            MoTaValidation,
            PasswordValidation,
            SDTValidation,
            TenValidation,
            UsernameValidation
        }
    }
}
