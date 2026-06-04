using BandBoostAI.Domain.Common;

namespace BandBoostAI.Domain.Entities;

public class AiEvaluation : BaseEntity
{
    public Guid StudentResponseId { get; set; }

    public decimal TaskResponseScore { get; set; }
    public decimal CoherenceScore { get; set; }
    public decimal LexicalScore { get; set; }
    public decimal GrammarScore { get; set; }
    public decimal OverallBand { get; set; }

    public required string FeedbackJson { get; set; }
}