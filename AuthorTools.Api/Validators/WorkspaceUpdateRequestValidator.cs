using AuthorTools.Api.Models;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class WorkspaceUpdateRequestValidator : AbstractValidator<WorkspaceUpdateRequest>
{
    public WorkspaceUpdateRequestValidator()
    {
        RuleFor(workspace => workspace.Name).NotEmpty();
    }
}
