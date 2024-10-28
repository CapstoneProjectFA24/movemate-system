using FluentValidation;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.Service.Commons.Validates
{
    public class AccountTokenValidator : AbstractValidator<AccountTokenRequest>
    {
        public AccountTokenValidator()
        {
            RuleFor(at => at.AccessToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");

            RuleFor(at => at.RefreshToken)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{PropertyName} is not empty.");
        }
    }
}