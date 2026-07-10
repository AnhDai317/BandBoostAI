using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if database is created and can connect
        await context.Database.EnsureCreatedAsync();

        if (await context.Exams.AnyAsync())
        {
            return; // Already seeded
        }

        // Seed 1: Reading Exam
        var readingExam = new Exam
        {
            Id = Guid.NewGuid(),
            Title = "IELTS Academic Reading Practice Test 1",
            Description = "A practice reading test with multiple-choice questions regarding the history of glass making.",
            Category = SkillCategory.Reading,
            DurationInMinutes = 60,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow
        };

        var readingSection = new ExamSection
        {
            Id = Guid.NewGuid(),
            ExamId = readingExam.Id,
            Title = "Section 1: The History of Glass",
            SharedContent = "<h3>The History of Glass</h3><p>Glass has a long and rich history, dating back to at least 3500 BC in Mesopotamia. The first glass objects were beads, possibly created as accidental by-products of metalworking. Over the centuries, techniques evolved from core-forming to glassblowing, revolutionized by Syrian craftsmen in the 1st century BC. Today, glass is indispensable in modern science, architecture, and daily life.</p>",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow
        };

        var readingQ1 = new Question
        {
            Id = Guid.NewGuid(),
            ExamSectionId = readingSection.Id,
            Type = QuestionType.MultipleChoice,
            QuestionNumber = 1,
            ContentJson = "{\"text\": \"Where were the first glass objects believed to have been created?\", \"options\": [\"Rome\", \"Mesopotamia\", \"Egypt\", \"Syria\"], \"correctAnswer\": \"Mesopotamia\"}",
            Explanation = "The first paragraph mentions that glass-making dates back to at least 3500 BC in Mesopotamia.",
            CreatedAt = DateTime.UtcNow
        };

        var readingQ2 = new Question
        {
            Id = Guid.NewGuid(),
            ExamSectionId = readingSection.Id,
            Type = QuestionType.MultipleChoice,
            QuestionNumber = 2,
            ContentJson = "{\"text\": \"What type of items were the earliest glass objects?\", \"options\": [\"Cups\", \"Beads\", \"Mirrors\", \"Windows\"], \"correctAnswer\": \"Beads\"}",
            Explanation = "The text states: 'The first glass objects were beads...'",
            CreatedAt = DateTime.UtcNow
        };

        var readingQ3 = new Question
        {
            Id = Guid.NewGuid(),
            ExamSectionId = readingSection.Id,
            Type = QuestionType.MultipleChoice,
            QuestionNumber = 3,
            ContentJson = "{\"text\": \"Who revolutionized glassmaking by inventing glassblowing?\", \"options\": [\"Mesopotamians\", \"Romans\", \"Syrian craftsmen\", \"Egyptians\"], \"correctAnswer\": \"Syrian craftsmen\"}",
            Explanation = "The text states: '...revolutionized by Syrian craftsmen in the 1st century BC.'",
            CreatedAt = DateTime.UtcNow
        };

        readingSection.Questions = new List<Question> { readingQ1, readingQ2, readingQ3 };
        readingExam.Sections = new List<ExamSection> { readingSection };

        // Seed 2: Writing Exam
        var writingExam = new Exam
        {
            Id = Guid.NewGuid(),
            Title = "IELTS Writing Task 2 - Tech & Isolation",
            Description = "Write an essay about the social impacts of modern technology.",
            Category = SkillCategory.Writing,
            DurationInMinutes = 40,
            IsPremium = false,
            CreatedAt = DateTime.UtcNow
        };

        var writingSection = new ExamSection
        {
            Id = Guid.NewGuid(),
            ExamId = writingExam.Id,
            Title = "Writing Task 2 Essay Prompt",
            SharedContent = "<p><strong>You should spend about 40 minutes on this task.</strong></p><p>Write about the following topic:</p><p><em>Some people believe that the development of modern technology has made people more isolated, while others argue that it has connected people in ways never before possible. Discuss both views and give your opinion.</em></p><p>Give reasons for your answer and include any relevant examples from your own knowledge or experience. Write at least 250 words.</p>",
            OrderIndex = 1,
            CreatedAt = DateTime.UtcNow
        };

        var writingQ1 = new Question
        {
            Id = Guid.NewGuid(),
            ExamSectionId = writingSection.Id,
            Type = QuestionType.Essay,
            QuestionNumber = 1,
            ContentJson = "{\"text\": \"Submit your IELTS Writing Task 2 essay here. The AI will evaluate it according to standard IELTS criteria.\"}",
            Explanation = "A good response should discuss both isolation (less physical contact, screen addiction) and connectivity (social networks, instant video calls), and conclude with a clear personal stance.",
            CreatedAt = DateTime.UtcNow
        };

        writingSection.Questions = new List<Question> { writingQ1 };
        writingExam.Sections = new List<ExamSection> { writingSection };

        await context.Exams.AddAsync(readingExam);
        await context.Exams.AddAsync(writingExam);
        await context.SaveChangesAsync();
    }
}
