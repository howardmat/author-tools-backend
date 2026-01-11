using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class WorkspaceCreateRequestValidator : AbstractValidator<WorkspaceCreateRequest>
{
    public WorkspaceCreateRequestValidator()
    {
        RuleFor(workspace => workspace.Name).NotEmpty();
    }
}
