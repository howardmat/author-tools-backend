using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class CommonEntityCreateRequestValidator : AbstractValidator<CommonEntityCreateRequest>
{
    public CommonEntityCreateRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
    }
}
