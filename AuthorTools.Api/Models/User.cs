namespace AuthorTools.Api.Models;

public class User(string id, string email, string? firstname, string? lastname)
{
    public string Id { get; set; } = id;
    public string Email { get; set; } = email;
    public string? Firstname { get; set; } = firstname;
    public string? Lastname { get; set; } = lastname;
}
