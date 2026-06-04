namespace BandBoostAI.Domain.Enums;

public enum QuestionType
{
    MultipleChoice = 1,     // Trắc nghiệm (A,B,C,D)
    TrueFalseNotGiven = 2,  // True/False/Not Given hoặc Yes/No/Not Given
    FillInTheBlank = 3,     // Điền vào chỗ trống
    Matching = 4,           // Nối thông tin
    Essay = 5,              // Bài luận (Writing Task 1, Task 2)
    SpeakingPrompt = 6      // Câu hỏi Speaking
}