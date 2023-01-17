﻿using MotoStore.Database;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MotoStore.Helpers
{
    public class CustomValidationRule : ValidationRule
    {
        public CustomValidationRule()
            => ValidationMode = ValidationRules.None;

        public ValidationRules ValidationMode { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            switch (ValidationMode)
            {
                case ValidationRules.None: return new(true, default);
                case ValidationRules.ChucVuValidation: return ChucVuValidation(value);
                case ValidationRules.DateValidation: return DateValidation(value, cultureInfo);
                case ValidationRules.DiaChiValidation: return DiaChiValidation(value);
                case ValidationRules.EmailValidation: return EmailValidation(value);
                case ValidationRules.GioiTinhValidation: return GioiTinhValidation(value);
                case ValidationRules.HoTenValidation: return HoTenValidation(value);
                case ValidationRules.LoaiKhValidation: return LoaiKhValidation(value);
                case ValidationRules.MaKhValidation: return MaKhValidation(value);
                case ValidationRules.MaMhValidation: return MaMhValidation(value);
                case ValidationRules.MaNccValidation: return MaNccValidation(value);
                case ValidationRules.MaNvValidation: return MaNvValidation(value);
                case ValidationRules.SDTValidation: return SDTValidation(value);
                default: throw new("Unknown ValidationRule");
            }
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
            if (DateTime.TryParseExact(value.ToString(), "d/M/yyyy", cultureInfo, DateTimeStyles.AllowWhiteSpaces, out _))
                return new(true, default);

            return new(false, "Nhập ngày hợp lệ, có trên theo định dạng dd/MM/yyyy");
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
            if (!string.IsNullOrEmpty(value.ToString()))
                if (value.ToString().Length > 40)
                    return new(false, "Địa chỉ quá dài, tối đa 40 ký tự!");

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

        private static ValidationResult MaKhValidation(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return new(false, "Mã khách hàng không được để trống!");
            if (!Regex.IsMatch(value.ToString(), @"^KH\d{3}$"))
                return new(false, "Mã khách hàng phải theo cú pháp KH***, trong đó * là các chữ số");
            MainDatabase mdb = new();
            foreach (var kh in mdb.KhachHangs)
                if (!kh.DaXoa && value.ToString() == kh.MaKh)
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
            foreach (var mh in mdb.MatHangs)
                if (!mh.DaXoa && value.ToString() == mh.MaMh)
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
            foreach (var mh in mdb.MatHangs)
                if (!mh.DaXoa && value.ToString() == mh.MaMh)
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
            foreach (var nv in mdb.NhanViens)
                if (!nv.DaXoa && value.ToString() == nv.MaNv)
                    return new(true, default);

            return new(false, "Mã nhân viên không tồn tại hoặc đã xóa");
        }

        private static ValidationResult SDTValidation(object value)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().Length > 10)
                    return new(false, "SĐT quá dài, tối đa 10 ký tự!");
                if (!value.ToString().ToCharArray().All(char.IsDigit))
                    return new(false, "SĐT chỉ được chứa các ký tự số!");
            }

            return new(true, default);
        }

        public enum ValidationRules
        {
            None,
            ChucVuValidation,
            DateValidation,
            DiaChiValidation,
            EmailValidation,
            GioiTinhValidation,
            HoTenValidation,
            LoaiKhValidation,
            MaKhValidation,
            MaMhValidation,
            MaNccValidation,
            MaNvValidation,
            SDTValidation
        }
    }
}