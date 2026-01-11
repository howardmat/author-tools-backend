using AuthorTools.Data.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class DetailSectionValidator : AbstractValidator<DetailSection>
{
    public DetailSectionValidator()
    {
        RuleFor(ds => ds.Id).NotEmpty();
        RuleFor(ds => ds.Title).NotEmpty();
        RuleForEach(ds => ds.Attributes).SetValidator(new AttributeValidator());
    }
}
