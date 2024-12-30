namespace AuthorTools.Api.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }

    public IResult ToHttpResult()
    {
        if (Success)
        {
            return Results.Ok();
        }

        // Assumption for now is that it was a validation error
        return Results.BadRequest(new { Error });
    }
}
