using FluentValidation;
using Attribute = AuthorTools.Data.Models.Attribute;

namespace AuthorTools.Api.Validators;

public class AttributeValidator:AbstractValidator<Attribute>
{
    public AttributeValidator()
    {
        RuleFor(attribute => attribute.Id).NotEmpty();
        RuleFor(attribute => attribute.Label).NotEmpty();
    }
}
