using System;
using System.Collections.Generic;

namespace BandBoostAI.Application.DTOs.Exam;

public class SubmitExamDto
{
    public List<QuestionAnswerDto> Answers { get; set; } = new();
}

public class QuestionAnswerDto
{
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
}
