using System.Net.Http.Json;
using System.Text.Json;
using BandBoostAI.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BandBoostAI.Infrastructure.Providers;

public class AiScoringService : IAiScoringService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AiScoringService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // Lấy API Key từ appsettings.json
        _apiKey = configuration["AiSettings:ApiKey"] ?? throw new Exception("Thiếu AI API Key!");
    }

    public async Task<string> ScoreWritingTaskAsync(string prompt, string studentAnswer)
    {
        // 1. KỸ NGHỆ PROMPT (System Prompt) - Đây là linh hồn của tính năng
        var systemInstruction = @"
            You are an expert, strict, and highly qualified IELTS examiner. 
            Your task is to evaluate the student's Writing Task 2 essay based on the exact 4 IELTS criteria: 
            Task Response, Coherence and Cohesion, Lexical Resource, and Grammatical Range and Accuracy.

            You MUST return the evaluation STRICTLY in the following JSON format, and nothing else:
            {
                ""overallBand"": 6.5,
                ""criteriaScores"": {
                    ""taskResponse"": 6.0,
                    ""coherence"": 6.5,
                    ""lexicalResource"": 7.0,
                    ""grammar"": 6.5
                },
                ""detailedFeedback"": ""Write a brief, constructive overall feedback here."",
                ""improvements"": [
                    ""Suggestion 1"",
                    ""Suggestion 2""
                ]
            }";

        // 2. Gói tin gửi đi (Payload) - Ví dụ theo cấu trúc API của OpenAI/Gemini
        var requestPayload = new
        {
            model = "gpt-4o-mini", // Hoặc model tương đương
            messages = new[]
            {
                new { role = "system", content = systemInstruction },
                new { role = "user", content = $"Question: {prompt}\n\nStudent's Essay: {studentAnswer}" }
            },
            temperature = 0.2 // Set nhiệt độ thấp để AI chấm điểm ổn định, bớt ảo giác
        };

        // 3. Gọi API (Giả lập gọi sang OpenAI)
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        
        var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestPayload);
        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        // 4. Bóc tách cục JSON mà AI trả về (nằm sâu trong choices[0].message.content)
        var aiResponseContent = responseData
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return aiResponseContent ?? "{}";
    }
}
