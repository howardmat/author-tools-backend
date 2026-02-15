using AuthorTools.Api.Validators;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuthorTools.Tests.Api.Validators;

public class ValidationExtensionsTests
{
    [Fact]
    public void ToBadRequest_GroupsFailuresByPropertyName()
    {
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Name", "Name must be <= 100 chars"),
            new("Description", "Description is required")
        };

        BadRequest<Microsoft.AspNetCore.Mvc.ValidationProblemDetails> result = failures.ToBadRequest();

        result.Value.Should().NotBeNull();
        result.Value!.Errors.Should().ContainKey("Name");
        result.Value.Errors["Name"].Should().BeEquivalentTo(["Name is required", "Name must be <= 100 chars"]);
        result.Value.Errors.Should().ContainKey("Description");
        result.Value.Errors["Description"].Should().BeEquivalentTo(["Description is required"]);
    }

    [Fact]
    public void ToConflict_JoinsMessagesAndSetsProblemDetails()
    {
        var failures = new List<ValidationFailure>
        {
            new("", "First"),
            new("", "Second")
        };

        var result = failures.ToConflict("Cannot delete Workspace");

        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Cannot delete Workspace");
        result.Value.Detail.Should().Be("First|Second");
        result.Value.Status.Should().Be(StatusCodes.Status409Conflict);
    }
}
