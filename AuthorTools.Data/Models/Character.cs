using AuthorTools.SharedLib.Common;

namespace AuthorTools.Data.Models;

public class Character : BaseCosmosModel
{
    public string? Name { get; set; }
    public string? Alias { get; set; }
    public CodeValue? Archetype { get; set; }
    public string? ImageFileId { get; set; }
    public DateTimeOffset? BirthDate { get; set; }
    public int? Age { get; set; }
    public string? Profession { get; set; }
    public string? LoveInterest { get; set; }
    public CodeValue? Gender { get; set; }
    public string? EyeColor { get; set; }
    public string? HairType { get; set; }
    public string? HairColor { get; set; }
    public string? HairLength { get; set; }
    public string? BodyShape { get; set; }
    public string? PersonalTraits { get; set; }
    public string? Abilities { get; set; }
    public string? Strength { get; set; }
    public string? Weakness { get; set; }
    public string? FriendsAndFamily { get; set; }
    public string? History { get; set; }
    public string? FamilyHistory { get; set; }
    public int? Order { get; set; }
    public IEnumerable<CharacterRelationship> Relationships { get; set; } = [];
}
