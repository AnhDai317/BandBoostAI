namespace BandBoostAI.Domain.Enums;

public enum Role
{
    Student,
    Teacher,
    Admin
}

public enum QuestionType
{
    MultipleChoice,
    FillInBlank,
    TrueFalseNotGiven,
    Essay // Phục vụ Writing Task 1, Task 2 để AI chấm
}

public enum SkillCategory
{
    Listening,
    Reading,
    Writing,
    Speaking,
    Grammar,
    Vocabulary
}