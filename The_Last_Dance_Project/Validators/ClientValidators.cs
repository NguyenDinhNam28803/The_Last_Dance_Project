using System;
using System.Globalization;
using System.Linq;
using FluentValidation;
using The_Last_Dance_Project.Dtos;

namespace The_Last_Dance_Project.Validators
{
    /// <summary>
    /// Validator cho tạo mới Khách hàng (Client) theo các ràng buộc nghiệp vụ URD mục 3.2.3.
    /// </summary>
    public class ClientCreateValidator : AbstractValidator<ClientCreateDto>
    {
        public ClientCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên khách hàng không được để trống");

            RuleFor(x => x.RegistrationType)
                .NotEmpty().WithMessage("Loại hình khách hàng không được để trống");

            // ClientID: nếu có nhập (khác rỗng/"#") phải gồm đúng 6 chữ số
            RuleFor(x => x.ClientId)
                .Must(BeValidClientId)
                .WithMessage("Mã khách hàng phải gồm đúng 6 chữ số")
                .When(x => !string.IsNullOrWhiteSpace(x.ClientId) && x.ClientId != "#");

            // KH cá nhân (Retail): bắt buộc Giới tính + Ngày sinh, đủ 18 tuổi
            When(x => IsIndividual(x.RegistrationType), () =>
            {
                RuleFor(x => x.Gender)
                    .NotEmpty().WithMessage("Giới tính bắt buộc với khách hàng cá nhân");

                RuleFor(x => x.DateOfBirth)
                    .NotEmpty().WithMessage("Ngày sinh bắt buộc với khách hàng cá nhân")
                    .Must(IsAtLeast18).WithMessage("Khách hàng phải đủ 18 tuổi");
            });

            // KH tổ chức (Institution): bắt buộc Loại hình tổ chức
            When(x => IsInstitution(x.RegistrationType), () =>
            {
                RuleFor(x => x.InstitutionType)
                    .NotEmpty().WithMessage("Loại hình tổ chức bắt buộc với khách hàng tổ chức");
            });

            // KH nước ngoài: bắt buộc Investor code (mã NĐT do VSD cấp)
            When(x => IsForeign(x.RegistrationType), () =>
            {
                RuleFor(x => x.InvestorCode)
                    .NotEmpty().WithMessage("Investor code bắt buộc với khách hàng nước ngoài");
            });
        }

        private static bool BeValidClientId(string? clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return true; // đã chặn bởi When
            return clientId.Length == 6 && clientId.All(char.IsDigit);
        }

        private static bool IsIndividual(string? registrationType)
            => (registrationType ?? string.Empty).ToUpperInvariant().Contains("RETAIL");

        private static bool IsInstitution(string? registrationType)
            => (registrationType ?? string.Empty).ToUpperInvariant().Contains("INSTITUTION");

        private static bool IsForeign(string? registrationType)
            => (registrationType ?? string.Empty).ToUpperInvariant().Contains("FOREIGN");

        private static bool IsAtLeast18(string? dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(dateOfBirth)) return false;

            string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "MM/dd/yyyy" };
            DateTime dob;
            var parsed = DateTime.TryParseExact(dateOfBirth.Trim(), formats,
                             CultureInfo.InvariantCulture, DateTimeStyles.None, out dob)
                         || DateTime.TryParse(dateOfBirth.Trim(), CultureInfo.InvariantCulture,
                             DateTimeStyles.None, out dob);
            if (!parsed) return false;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }
}
