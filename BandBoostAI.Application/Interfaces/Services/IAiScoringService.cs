namespace BandBoostAI.Application.Interfaces.Services;

public interface IAiScoringService
{
    // Nhận vào Đề bài và Bài làm của học sinh -> Trả về cục JSON đánh giá chi tiết
    Task<string> ScoreWritingTaskAsync(string prompt, string studentAnswer);
}
