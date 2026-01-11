using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories.Interfaces;
using FluentValidation;

namespace AuthorTools.Api.Validators;

public class WorkspaceDeleteValidator : AbstractValidator<Workspace>
{
    private readonly IRepository<Workspace> _workspaceRepo;

    public WorkspaceDeleteValidator(IRepository<Workspace> workspaceRepo)
    {
        _workspaceRepo = workspaceRepo;

        // User shouldn't be able to delete their only Workspace
        RuleFor(workspace => workspace).MustAsync(async (workspace, cancellation) =>
        {
            if (workspace?.Owner?.Id == null) return true;

            return (await _workspaceRepo.GetAllAsync(workspace.Owner.Id)).Count() > 1;
        }).WithMessage("WORKSPACE_IS_LAST");
    }
}
