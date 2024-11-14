namespace AuthorTools.Api.Models;

public class CodeValue(string code, string value)
{
    public string Code { get; set; } = code;
    public string Value { get; set; } = value;
}
