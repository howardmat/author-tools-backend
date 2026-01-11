using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class CommonEntityUpdateRequestValidator : AbstractValidator<CommonEntityUpdateRequest>
{
    public CommonEntityUpdateRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty();
        RuleForEach(request => request.DetailSections).SetValidator(new DetailSectionValidator());
    }
}
