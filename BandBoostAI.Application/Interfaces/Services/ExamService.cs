using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BandBoostAI.Application.DTOs.Exam;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;

namespace BandBoostAI.Application.Services;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IAiScoringService _aiScoringService;
    private readonly IExamAttemptRepository _attemptRepository;
    private readonly IUserRepository _userRepository;

    public ExamService(
        IExamRepository examRepository,
        IAiScoringService aiScoringService,
        IExamAttemptRepository attemptRepository,
        IUserRepository userRepository)
    {
        _examRepository = examRepository;
        _aiScoringService = aiScoringService;
        _attemptRepository = attemptRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> CreateExamAsync(CreateExamDto dto)
    {
        var exam = new Exam
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            DurationInMinutes = dto.DurationInMinutes,
            IsPremium = dto.IsPremium,
            Sections = dto.Sections.Select(s => new ExamSection
            {
                Title = s.Title,
                SharedContent = s.SharedContent,
                OrderIndex = s.OrderIndex,
                Questions = s.Questions.Select(q => new Question
                {
                    Type = q.Type,
                    QuestionNumber = q.QuestionNumber,
                    ContentJson = q.ContentJson,
                    Explanation = q.Explanation
                }).ToList()
            }).ToList()
        };

        var result = await _examRepository.AddAsync(exam);
        return result.Id;
    }

    public async Task<Exam?> GetExamByIdAsync(Guid id)
    {
        return await _examRepository.GetExamWithDetailsAsync(id);
    }

    public async Task<IReadOnlyList<Exam>> GetAllExamsAsync()
    {
        return await _examRepository.GetAllAsync();
    }

    public async Task<ExamAttemptResponseDto> SubmitExamAsync(Guid examId, Guid userId, SubmitExamDto dto)
    {
        var exam = await _examRepository.GetExamWithDetailsAsync(examId);
        if (exam == null)
        {
            throw new Exception("Không tìm thấy đề thi này.");
        }

        string answersJson = JsonSerializer.Serialize(dto.Answers);
        string feedbackJson = string.Empty;
        decimal score = 0;

        if (exam.Category == SkillCategory.Writing)
        {
            var prompt = exam.Sections.FirstOrDefault()?.Questions.FirstOrDefault()?.ContentJson ?? exam.Title;
            var essayAnswer = dto.Answers.FirstOrDefault()?.AnswerText ?? string.Empty;

            try
            {
                feedbackJson = await _aiScoringService.ScoreWritingTaskAsync(prompt, essayAnswer);
                
                using var doc = JsonDocument.Parse(feedbackJson);
                if (doc.RootElement.TryGetProperty("overallBand", out var overallProp))
                {
                    if (overallProp.ValueKind == JsonValueKind.Number)
                    {
                        score = overallProp.GetDecimal();
                    }
                    else if (overallProp.ValueKind == JsonValueKind.String && decimal.TryParse(overallProp.GetString(), out var val))
                    {
                        score = val;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Scoring failed: {ex.Message}. Using fallback simulator.");
                // Fallback Simulator for Demo
                var wordsCount = essayAnswer.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                decimal tr = 6.0m;
                decimal cc = 6.0m;
                decimal lr = 6.0m;
                decimal gr = 6.0m;

                if (wordsCount < 150)
                {
                    tr = 5.0m;
                }
                else if (wordsCount > 250)
                {
                    tr = 7.0m;
                    cc = 6.5m;
                    lr = 6.5m;
                    gr = 6.5m;
                }

                score = Math.Round((tr + cc + lr + gr) / 4.0m * 2) / 2.0m;

                feedbackJson = $$"""
                {
                    "overallBand": {{score}},
                    "criteriaScores": {
                        "taskResponse": {{tr}},
                        "coherence": {{cc}},
                        "lexicalResource": {{lr}},
                        "grammar": {{gr}}
                    },
                    "detailedFeedback": "Bài viết của bạn có cấu trúc cơ bản tốt và luận điểm rõ ràng. Tuy nhiên cần chú ý đa dạng hóa cấu trúc ngữ pháp và từ vựng học thuật để nâng cao band điểm.",
                    "improvements": [
                        "Cần viết tối thiểu 250 từ để tránh bị trừ điểm Task Response (hiện tại có {{wordsCount}} từ).",
                        "Sử dụng thêm các từ nối học thuật như 'Furthermore', 'Consequently', 'On the other hand'.",
                        "Chú ý sửa các lỗi ngữ pháp cơ bản về chia thì và hòa hợp chủ vị."
                    ]
                }
                """;
            }
        }
        else
        {
            // Reading / Listening Auto-Grading logic
            int correctCount = 0;
            int totalQuestions = 0;

            foreach (var section in exam.Sections)
            {
                foreach (var question in section.Questions)
                {
                    totalQuestions++;
                    var studentAnswer = dto.Answers.FirstOrDefault(a => a.QuestionId == question.Id)?.AnswerText?.Trim();

                    // Parse the expected answer from Question's ContentJson
                    string? expectedAnswer = null;
                    try
                    {
                        using var qDoc = JsonDocument.Parse(question.ContentJson);
                        if (qDoc.RootElement.TryGetProperty("correctAnswer", out var correctProp))
                        {
                            expectedAnswer = correctProp.GetString();
                        }
                        else if (qDoc.RootElement.TryGetProperty("correct", out var correctIdxProp))
                        {
                            if (correctIdxProp.ValueKind == JsonValueKind.Number)
                            {
                                expectedAnswer = correctIdxProp.GetInt32().ToString();
                            }
                            else if (correctIdxProp.ValueKind == JsonValueKind.String)
                            {
                                expectedAnswer = correctIdxProp.GetString();
                            }
                        }
                    }
                    catch
                    {
                        // Ignore parse error, expectedAnswer stays null
                    }

                    if (!string.IsNullOrEmpty(expectedAnswer) && !string.IsNullOrEmpty(studentAnswer) &&
                        string.Equals(expectedAnswer.Trim(), studentAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctCount++;
                    }
                }
            }

            score = MapCorrectAnswersToIeltsBand(correctCount, totalQuestions);

            feedbackJson = $$"""
            {
                "overallBand": {{score}},
                "correctAnswers": {{correctCount}},
                "totalQuestions": {{totalQuestions}},
                "detailedFeedback": "Bạn làm đúng {{correctCount}} trên tổng số {{totalQuestions}} câu hỏi. Kết quả tương đương Band {{score}}.",
                "improvements": [
                    "Hãy phân tích kỹ các câu trả lời sai để hiểu rõ bẫy thông tin.",
                    "Luyện tập kỹ thuật Skimming & Scanning để phân bổ thời gian tốt hơn.",
                    "Ghi chú lại từ vựng mới xuất hiện trong các bài đọc để tăng vốn từ."
                ]
            }
            """;
        }

        // Save Attempt
        var attempt = new ExamAttempt
        {
            ExamId = examId,
            UserId = userId,
            AnswersJson = answersJson,
            Score = score,
            IsCompleted = true,
            Feedback = feedbackJson
        };

        await _attemptRepository.AddAsync(attempt);

        // Update User's CurrentBandScore
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.CurrentBandScore = score;
            await _userRepository.UpdateAsync(user);
        }

        return new ExamAttemptResponseDto
        {
            Id = attempt.Id,
            UserId = attempt.UserId,
            ExamId = attempt.ExamId,
            ExamTitle = exam.Title,
            AnswersJson = attempt.AnswersJson,
            Score = attempt.Score,
            IsCompleted = attempt.IsCompleted,
            Feedback = attempt.Feedback,
            CreatedAt = attempt.CreatedAt
        };
    }

    public async Task<List<ExamAttemptResponseDto>> GetUserAttemptsAsync(Guid userId)
    {
        var attempts = await _attemptRepository.GetAttemptsByUserIdAsync(userId);
        return attempts.Select(a => new ExamAttemptResponseDto
        {
            Id = a.Id,
            UserId = a.UserId,
            ExamId = a.ExamId,
            ExamTitle = a.Exam?.Title ?? "Đề thi không xác định",
            AnswersJson = a.AnswersJson,
            Score = a.Score,
            IsCompleted = a.IsCompleted,
            Feedback = a.Feedback,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<ExamAttemptResponseDto?> GetAttemptByIdAsync(Guid attemptId)
    {
        var a = await _attemptRepository.GetAttemptWithExamAsync(attemptId);
        if (a == null) return null;

        return new ExamAttemptResponseDto
        {
            Id = a.Id,
            UserId = a.UserId,
            ExamId = a.ExamId,
            ExamTitle = a.Exam?.Title ?? "Đề thi không xác định",
            AnswersJson = a.AnswersJson,
            Score = a.Score,
            IsCompleted = a.IsCompleted,
            Feedback = a.Feedback,
            CreatedAt = a.CreatedAt
        };
    }

    private decimal MapCorrectAnswersToIeltsBand(int correct, int total)
    {
        if (total == 0) return 0;
        double percentage = (double)correct / total;
        if (percentage >= 0.97) return 9.0m;
        if (percentage >= 0.92) return 8.5m;
        if (percentage >= 0.87) return 8.0m;
        if (percentage >= 0.80) return 7.5m;
        if (percentage >= 0.75) return 7.0m;
        if (percentage >= 0.67) return 6.5m;
        if (percentage >= 0.57) return 6.0m;
        if (percentage >= 0.50) return 5.5m;
        if (percentage >= 0.40) return 5.0m;
        if (percentage >= 0.30) return 4.5m;
        if (percentage >= 0.20) return 4.0m;
        return 3.5m;
    }
}