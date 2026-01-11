using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class UserSettingCreateRequestValidator : AbstractValidator<UserSettingCreateRequest>
{
    public UserSettingCreateRequestValidator()
    {
        RuleFor(userSetting => userSetting.Theme).NotEmpty();
    }
}
