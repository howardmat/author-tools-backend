﻿using AuthorTools.Data.Models.Interfaces;

namespace AuthorTools.Data.Models;

public class CommonEntity : BaseMongoModel, ISortableModel
{
    public string? Name { get; set; }
    public string? ImageFileId { get; set; }
    public int? Order { get; set; }

    public IEnumerable<DetailSection> DetailSections { get; set; } = [];
}
