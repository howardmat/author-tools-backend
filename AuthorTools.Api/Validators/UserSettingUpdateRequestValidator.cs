using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class UserSettingUpdateRequestValidator : AbstractValidator<UserSettingUpdateRequest>
{
    public UserSettingUpdateRequestValidator()
    {
        RuleFor(userSetting => userSetting.Theme).NotEmpty();
    }
}
