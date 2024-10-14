using FluentValidation;
using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Commons
{

    public class AccountRequestValidator : AbstractValidator<AccountRequest>
    {
        public AccountRequestValidator()
        {
            RuleFor(ar => ar.Email)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.")
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
                .Must(BeValidEmailOrPhone).WithMessage("{PropertyName} must be a valid email or phone number.");

            RuleFor(ar => ar.Password)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} cannot be null.")
                .NotEmpty().WithMessage("{PropertyName} cannot be empty.");
        }

        private bool BeValidEmailOrPhone(string emailOrPhone)
        {
            // Kiểm tra nếu là email hợp lệ
            if (emailOrPhone.Contains("@"))
            {
                return IsValidEmail(emailOrPhone);
            }

            // Kiểm tra nếu là số điện thoại hợp lệ
            return IsValidPhoneNumber(emailOrPhone);
        }

        private bool IsValidEmail(string email)
        {
            // Sử dụng phương thức kiểm tra định dạng email
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Kiểm tra nếu số điện thoại chỉ chứa chữ số và độ dài hợp lệ (ví dụ: 10-15 ký tự)
            return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 10 && phoneNumber.Length <= 15;
        }
    }



}
