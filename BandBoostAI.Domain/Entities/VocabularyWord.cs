using BandBoostAI.Domain.Common;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Domain.Entities;

public class VocabularyWord : BaseEntity
{
    public Guid TopicId { get; set; }
    public EnglishLevel Level { get; set; }
    public required string Word { get; set; }
    public string Pronunciation { get; set; } = string.Empty;
    public string PartOfSpeech { get; set; } = string.Empty;
    public required string Meaning { get; set; }
    public required string MeaningVietnamese { get; set; }
    public required string ExampleSentence { get; set; }
    public required string ExampleTranslation { get; set; }

    public VocabularyTopic? Topic { get; set; }
}
