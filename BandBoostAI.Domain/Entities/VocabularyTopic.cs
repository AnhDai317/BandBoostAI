using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class VocabularyTopic : BaseEntity
{
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = "menu_book";
    public int SortOrder { get; set; }

    public ICollection<VocabularyWord> Words { get; set; } = new List<VocabularyWord>();
}
