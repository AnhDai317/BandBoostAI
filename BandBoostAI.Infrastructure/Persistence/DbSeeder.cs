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
        if (!await context.Exams.AnyAsync())
        {
            await SeedExamsAsync(context);
        }

        if (!await context.VocabularyTopics.AnyAsync())
        {
            await SeedVocabularyAsync(context);
        }

        await SupplementalVocabularySeeder.SeedAsync(context);
        await ExtendedVocabularySeeder.SeedAsync(context);
    }

    private static async Task SeedExamsAsync(ApplicationDbContext context)
    {

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

    private static async Task SeedVocabularyAsync(ApplicationDbContext context)
    {
        var topics = new[]
        {
            new VocabularyTopic { Slug = "daily-life", Name = "Cuộc sống hằng ngày", Description = "Những từ thiết thực dùng trong sinh hoạt và giao tiếp mỗi ngày.", Icon = "coffee", SortOrder = 1 },
            new VocabularyTopic { Slug = "travel", Name = "Du lịch", Description = "Tự tin tại sân bay, khách sạn và những hành trình mới.", Icon = "flight_takeoff", SortOrder = 2 },
            new VocabularyTopic { Slug = "work-career", Name = "Công việc", Description = "Từ vựng cho môi trường chuyên nghiệp và phát triển sự nghiệp.", Icon = "business_center", SortOrder = 3 },
            new VocabularyTopic { Slug = "education", Name = "Giáo dục", Description = "Học tập, trường lớp và các chủ đề học thuật thường gặp.", Icon = "school", SortOrder = 4 },
            new VocabularyTopic { Slug = "technology", Name = "Công nghệ", Description = "Thế giới số, đổi mới và xu hướng công nghệ hiện đại.", Icon = "devices", SortOrder = 5 },
            new VocabularyTopic { Slug = "environment", Name = "Môi trường", Description = "Thiên nhiên, khí hậu và lối sống bền vững.", Icon = "eco", SortOrder = 6 }
        };

        await context.VocabularyTopics.AddRangeAsync(topics);
        await context.SaveChangesAsync();
        var topicBySlug = topics.ToDictionary(topic => topic.Slug);

        var words = new[]
        {
            Word(topicBySlug["daily-life"], EnglishLevel.A1, "routine", "/ruːˈtiːn/", "noun", "the usual order in which you do things", "thói quen hằng ngày", "My morning routine starts with a glass of water.", "Thói quen buổi sáng của tôi bắt đầu bằng một cốc nước."),
            Word(topicBySlug["daily-life"], EnglishLevel.A2, "neighbourhood", "/ˈneɪbəhʊd/", "noun", "the area around where you live", "khu dân cư", "There is a quiet park in my neighbourhood.", "Có một công viên yên tĩnh trong khu tôi sống."),
            Word(topicBySlug["daily-life"], EnglishLevel.B1, "convenient", "/kənˈviːniənt/", "adjective", "easy to use or suitable for your needs", "thuận tiện", "Online shopping is convenient for busy people.", "Mua sắm trực tuyến thuận tiện cho người bận rộn."),
            Word(topicBySlug["daily-life"], EnglishLevel.B2, "maintain", "/meɪnˈteɪn/", "verb", "to keep something at the same level or standard", "duy trì", "It is important to maintain a healthy work-life balance.", "Duy trì sự cân bằng lành mạnh giữa công việc và cuộc sống rất quan trọng."),
            Word(topicBySlug["daily-life"], EnglishLevel.C1, "meticulous", "/məˈtɪkjələs/", "adjective", "showing great attention to every detail", "tỉ mỉ", "She keeps meticulous records of her monthly expenses.", "Cô ấy ghi chép tỉ mỉ các khoản chi tiêu hằng tháng."),
            Word(topicBySlug["daily-life"], EnglishLevel.C2, "mundane", "/mʌnˈdeɪn/", "adjective", "ordinary and not interesting", "tẻ nhạt, đời thường", "Music makes even mundane household tasks enjoyable.", "Âm nhạc khiến cả những việc nhà tẻ nhạt trở nên thú vị."),

            Word(topicBySlug["travel"], EnglishLevel.A1, "journey", "/ˈdʒɜːni/", "noun", "the act of travelling from one place to another", "hành trình", "The train journey takes two hours.", "Hành trình bằng tàu mất hai giờ."),
            Word(topicBySlug["travel"], EnglishLevel.A2, "luggage", "/ˈlʌɡɪdʒ/", "noun", "bags and cases that you take on a trip", "hành lý", "Please keep your luggage with you at all times.", "Vui lòng luôn giữ hành lý bên mình."),
            Word(topicBySlug["travel"], EnglishLevel.B1, "destination", "/ˌdestɪˈneɪʃn/", "noun", "the place to which someone is going", "điểm đến", "Da Nang is a popular beach destination.", "Đà Nẵng là một điểm đến biển nổi tiếng."),
            Word(topicBySlug["travel"], EnglishLevel.B2, "itinerary", "/aɪˈtɪnərəri/", "noun", "a detailed plan for a journey", "lịch trình", "Our itinerary includes three days in Kyoto.", "Lịch trình của chúng tôi gồm ba ngày ở Kyoto."),
            Word(topicBySlug["travel"], EnglishLevel.C1, "picturesque", "/ˌpɪktʃəˈresk/", "adjective", "visually attractive, especially in a charming way", "đẹp như tranh", "We stayed in a picturesque village by the lake.", "Chúng tôi ở trong một ngôi làng đẹp như tranh bên hồ."),
            Word(topicBySlug["travel"], EnglishLevel.C2, "wanderlust", "/ˈwɒndəlʌst/", "noun", "a strong desire to travel", "niềm đam mê xê dịch", "Her wanderlust led her to explore five continents.", "Niềm đam mê xê dịch đã đưa cô ấy khám phá năm châu lục."),

            Word(topicBySlug["work-career"], EnglishLevel.A1, "colleague", "/ˈkɒliːɡ/", "noun", "a person that you work with", "đồng nghiệp", "I have lunch with my colleagues.", "Tôi ăn trưa cùng các đồng nghiệp."),
            Word(topicBySlug["work-career"], EnglishLevel.A2, "deadline", "/ˈdedlaɪn/", "noun", "the latest time by which something must be finished", "hạn chót", "The deadline for this report is Friday.", "Hạn chót của báo cáo này là thứ Sáu."),
            Word(topicBySlug["work-career"], EnglishLevel.B1, "promotion", "/prəˈməʊʃn/", "noun", "a move to a more important job or rank", "sự thăng chức", "She earned a promotion after leading the project.", "Cô ấy được thăng chức sau khi dẫn dắt dự án."),
            Word(topicBySlug["work-career"], EnglishLevel.B2, "collaborate", "/kəˈlæbəreɪt/", "verb", "to work with someone to produce something", "hợp tác", "Our teams collaborate across different time zones.", "Các nhóm của chúng tôi hợp tác qua nhiều múi giờ."),
            Word(topicBySlug["work-career"], EnglishLevel.C1, "proficiency", "/prəˈfɪʃnsi/", "noun", "a high degree of skill or expertise", "sự thành thạo", "English proficiency is required for this position.", "Vị trí này yêu cầu sự thành thạo tiếng Anh."),
            Word(topicBySlug["work-career"], EnglishLevel.C2, "entrepreneurial", "/ˌɒntrəprəˈnɜːriəl/", "adjective", "showing the qualities of starting and managing a business", "có tinh thần khởi nghiệp", "The company encourages entrepreneurial thinking.", "Công ty khuyến khích tư duy khởi nghiệp."),

            Word(topicBySlug["education"], EnglishLevel.A1, "lesson", "/ˈlesn/", "noun", "a period of time in which someone is taught", "bài học", "Today's English lesson is about food.", "Bài học tiếng Anh hôm nay nói về đồ ăn."),
            Word(topicBySlug["education"], EnglishLevel.A2, "assignment", "/əˈsaɪnmənt/", "noun", "a task given as part of study", "bài tập được giao", "I finished my science assignment early.", "Tôi đã hoàn thành sớm bài tập khoa học."),
            Word(topicBySlug["education"], EnglishLevel.B1, "curriculum", "/kəˈrɪkjələm/", "noun", "the subjects included in a course of study", "chương trình học", "The school updated its English curriculum.", "Trường đã cập nhật chương trình học tiếng Anh."),
            Word(topicBySlug["education"], EnglishLevel.B2, "comprehensive", "/ˌkɒmprɪˈhensɪv/", "adjective", "including all or nearly all necessary aspects", "toàn diện", "The course provides a comprehensive introduction to economics.", "Khóa học cung cấp phần nhập môn kinh tế toàn diện."),
            Word(topicBySlug["education"], EnglishLevel.C1, "pedagogy", "/ˈpedəɡɒdʒi/", "noun", "the method and practice of teaching", "phương pháp sư phạm", "Modern pedagogy places learners at the centre.", "Phương pháp sư phạm hiện đại đặt người học ở trung tâm."),
            Word(topicBySlug["education"], EnglishLevel.C2, "erudite", "/ˈerudaɪt/", "adjective", "having or showing great knowledge", "uyên bác", "The professor gave an erudite lecture on linguistics.", "Giáo sư đã có một bài giảng uyên bác về ngôn ngữ học."),

            Word(topicBySlug["technology"], EnglishLevel.A1, "device", "/dɪˈvaɪs/", "noun", "an object or machine made for a particular purpose", "thiết bị", "This device helps me track my steps.", "Thiết bị này giúp tôi theo dõi số bước chân."),
            Word(topicBySlug["technology"], EnglishLevel.A2, "download", "/ˌdaʊnˈləʊd/", "verb", "to copy data from the internet to your device", "tải xuống", "You can download the app for free.", "Bạn có thể tải ứng dụng miễn phí."),
            Word(topicBySlug["technology"], EnglishLevel.B1, "privacy", "/ˈprɪvəsi/", "noun", "the state of being free from unwanted attention", "quyền riêng tư", "Users are increasingly concerned about online privacy.", "Người dùng ngày càng quan tâm đến quyền riêng tư trực tuyến."),
            Word(topicBySlug["technology"], EnglishLevel.B2, "innovation", "/ˌɪnəˈveɪʃn/", "noun", "a new idea, method, or product", "sự đổi mới", "Innovation can improve the quality of public services.", "Đổi mới có thể cải thiện chất lượng dịch vụ công."),
            Word(topicBySlug["technology"], EnglishLevel.C1, "ubiquitous", "/juːˈbɪkwɪtəs/", "adjective", "present or found everywhere", "hiện diện khắp nơi", "Smartphones have become ubiquitous in modern life.", "Điện thoại thông minh đã hiện diện khắp nơi trong cuộc sống hiện đại."),
            Word(topicBySlug["technology"], EnglishLevel.C2, "obsolescence", "/ˌɒbsəˈlesns/", "noun", "the process of becoming outdated and no longer useful", "sự lỗi thời", "Rapid innovation accelerates technological obsolescence.", "Đổi mới nhanh chóng đẩy nhanh sự lỗi thời của công nghệ."),

            Word(topicBySlug["environment"], EnglishLevel.A1, "recycle", "/ˌriːˈsaɪkl/", "verb", "to process waste so it can be used again", "tái chế", "We recycle paper, glass, and plastic.", "Chúng tôi tái chế giấy, thủy tinh và nhựa."),
            Word(topicBySlug["environment"], EnglishLevel.A2, "pollution", "/pəˈluːʃn/", "noun", "damage caused to the environment by harmful substances", "ô nhiễm", "Air pollution is a serious problem in large cities.", "Ô nhiễm không khí là vấn đề nghiêm trọng ở các thành phố lớn."),
            Word(topicBySlug["environment"], EnglishLevel.B1, "renewable", "/rɪˈnjuːəbl/", "adjective", "able to be replaced naturally and not used up", "có thể tái tạo", "Solar power is a renewable source of energy.", "Năng lượng mặt trời là một nguồn năng lượng tái tạo."),
            Word(topicBySlug["environment"], EnglishLevel.B2, "sustainable", "/səˈsteɪnəbl/", "adjective", "able to continue without damaging the environment", "bền vững", "Cities need more sustainable transport systems.", "Các thành phố cần hệ thống giao thông bền vững hơn."),
            Word(topicBySlug["environment"], EnglishLevel.C1, "biodiversity", "/ˌbaɪəʊdaɪˈvɜːsəti/", "noun", "the variety of plants and animals in an area", "đa dạng sinh học", "The forest supports remarkable biodiversity.", "Khu rừng duy trì sự đa dạng sinh học đáng kể."),
            Word(topicBySlug["environment"], EnglishLevel.C2, "anthropogenic", "/ˌænθrəpəˈdʒenɪk/", "adjective", "caused by human activity", "do con người gây ra", "Scientists monitor anthropogenic changes to the climate.", "Các nhà khoa học theo dõi những biến đổi khí hậu do con người gây ra.")
        };

        await context.VocabularyWords.AddRangeAsync(words);
        await context.SaveChangesAsync();
    }

    private static VocabularyWord Word(
        VocabularyTopic topic,
        EnglishLevel level,
        string word,
        string pronunciation,
        string partOfSpeech,
        string meaning,
        string meaningVietnamese,
        string exampleSentence,
        string exampleTranslation) => new()
    {
        TopicId = topic.Id,
        Level = level,
        Word = word,
        Pronunciation = pronunciation,
        PartOfSpeech = partOfSpeech,
        Meaning = meaning,
        MeaningVietnamese = meaningVietnamese,
        ExampleSentence = exampleSentence,
        ExampleTranslation = exampleTranslation
    };
}
