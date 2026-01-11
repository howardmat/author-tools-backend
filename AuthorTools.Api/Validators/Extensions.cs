using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthorTools.Api.Validators;

public static class Extensions
{
    public static BadRequest<ValidationProblemDetails> ToBadRequest (this List<ValidationFailure> validationFailures)
    {
        var errors = validationFailures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return TypedResults.BadRequest(new ValidationProblemDetails(errors));
    }

    public static Conflict<ProblemDetails> ToConflict(this List<ValidationFailure> validationFailures, string? title = "")
    {
        var errors = string.Join("|", validationFailures.Select(e => e.ErrorMessage));

        return TypedResults.Conflict(new ProblemDetails
        {
            Title = title,
            Detail = errors,
            Status = StatusCodes.Status409Conflict
        });
    }
}
