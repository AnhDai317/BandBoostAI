using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Persistence;

public static class ExtendedVocabularySeeder
{
    private const string VocabularyData = """
daily-life|A1|lunch|noun|bữa trưa
daily-life|A1|dinner|noun|bữa tối
daily-life|A1|kitchen|noun|nhà bếp
daily-life|A1|bedroom|noun|phòng ngủ
daily-life|A1|clean|verb|lau dọn, làm sạch
daily-life|A1|clothes|noun|quần áo
daily-life|A2|grocery|noun|thực phẩm, hàng tạp hóa
daily-life|A2|laundry|noun|quần áo cần giặt; việc giặt giũ
daily-life|A2|commute|verb|đi làm hằng ngày
daily-life|A2|hobby|noun|sở thích
daily-life|A2|invite|verb|mời
daily-life|A2|repair|verb|sửa chữa
daily-life|B1|habit|noun|thói quen
daily-life|B1|budget|noun|ngân sách
daily-life|B1|relax|verb|thư giãn
daily-life|B1|prepare|verb|chuẩn bị
daily-life|B1|socialize|verb|giao lưu
daily-life|B2|adaptable|adjective|dễ thích nghi
daily-life|B2|demanding|adjective|đòi hỏi cao
daily-life|B2|moderation|noun|sự điều độ
daily-life|B2|streamline|verb|tinh giản
daily-life|B2|procrastinate|verb|trì hoãn
daily-life|C1|sedentary|adjective|ít vận động
daily-life|C1|clutter|noun|sự bừa bộn
daily-life|C1|mindfulness|noun|sự chú tâm
daily-life|C1|wholesome|adjective|lành mạnh, bổ ích
daily-life|C1|errand|noun|việc vặt cần đi ra ngoài
daily-life|C2|domesticity|noun|đời sống gia đình
daily-life|C2|perfunctory|adjective|qua loa, chiếu lệ
daily-life|C2|convivial|adjective|vui vẻ, thân thiện
daily-life|C2|unremitting|adjective|không ngừng nghỉ
daily-life|C2|idiosyncratic|adjective|mang nét riêng khác thường
travel|A1|hotel|noun|khách sạn
travel|A1|map|noun|bản đồ
travel|A1|beach|noun|bãi biển
travel|A1|airport|noun|sân bay
travel|A1|plane|noun|máy bay
travel|A1|trip|noun|chuyến đi
travel|A2|tourist|noun|khách du lịch
travel|A2|guide|noun|hướng dẫn viên; sách hướng dẫn
travel|A2|arrival|noun|sự đến nơi
travel|A2|platform|noun|sân ga
travel|A2|visa|noun|thị thực
travel|A2|suitcase|noun|va li
travel|B1|sightseeing|noun|việc tham quan
travel|B1|landmark|noun|địa danh nổi bật
travel|B1|currency|noun|tiền tệ
travel|B1|abroad|adverb|ở nước ngoài
travel|B1|adventure|noun|cuộc phiêu lưu
travel|B2|layover|noun|thời gian quá cảnh
travel|B2|customs|noun|hải quan
travel|B2|overseas|adverb|ở nước ngoài, hải ngoại
travel|B2|jetlag|noun|sự mệt mỏi do lệch múi giờ
travel|B2|backpacking|noun|du lịch bụi
travel|C1|exhilarating|adjective|đầy phấn khích
travel|C1|secluded|adjective|hẻo lánh, kín đáo
travel|C1|unspoiled|adjective|còn nguyên sơ
travel|C1|nomadic|adjective|du mục
travel|C1|authentic|adjective|đích thực, nguyên bản
travel|C2|peripatetic|adjective|hay đi đây đó
travel|C2|sojourn|noun|kỳ lưu trú ngắn
travel|C2|globetrotter|noun|người đi du lịch khắp thế giới
travel|C2|escapade|noun|cuộc phiêu lưu táo bạo
travel|C2|odyssey|noun|hành trình dài nhiều trải nghiệm
work-career|A1|boss|noun|sếp
work-career|A1|office|noun|văn phòng
work-career|A1|job|noun|công việc
work-career|A1|company|noun|công ty
work-career|A1|customer|noun|khách hàng
work-career|A1|manager|noun|quản lý
work-career|A2|employee|noun|nhân viên
work-career|A2|career|noun|sự nghiệp
work-career|A2|project|noun|dự án
work-career|A2|report|noun|báo cáo
work-career|A2|contract|noun|hợp đồng
work-career|A2|training|noun|việc đào tạo
work-career|B1|recruit|verb|tuyển dụng
work-career|B1|leadership|noun|khả năng lãnh đạo
work-career|B1|teamwork|noun|khả năng làm việc nhóm
work-career|B1|qualification|noun|bằng cấp, trình độ chuyên môn
work-career|B1|achievement|noun|thành tựu
work-career|B2|appraisal|noun|sự đánh giá hiệu suất
work-career|B2|incentive|noun|sự khích lệ, ưu đãi
work-career|B2|workload|noun|khối lượng công việc
work-career|B2|stakeholder|noun|bên liên quan
work-career|B2|initiative|noun|sáng kiến
work-career|C1|autonomy|noun|quyền tự chủ
work-career|C1|accountability|noun|trách nhiệm giải trình
work-career|C1|competency|noun|năng lực
work-career|C1|hierarchy|noun|hệ thống cấp bậc
work-career|C1|retention|noun|sự giữ chân nhân sự
work-career|C2|bureaucracy|noun|bộ máy quan liêu
work-career|C2|corporatization|noun|quá trình doanh nghiệp hóa
work-career|C2|underemployment|noun|tình trạng thiếu việc làm phù hợp
work-career|C2|workaholic|noun|người nghiện công việc
work-career|C2|remunerative|adjective|mang lại thù lao cao
education|A1|teacher|noun|giáo viên
education|A1|class|noun|lớp học
education|A1|school|noun|trường học
education|A1|book|noun|sách
education|A1|exam|noun|kỳ thi
education|A1|pencil|noun|bút chì
education|A2|course|noun|khóa học
education|A2|subject|noun|môn học
education|A2|library|noun|thư viện
education|A2|grade|noun|điểm số; cấp lớp
education|A2|quiz|noun|bài kiểm tra ngắn
education|A2|knowledge|noun|kiến thức
education|B1|research|noun|nghiên cứu
education|B1|lecture|noun|bài giảng
education|B1|degree|noun|bằng cấp
education|B1|skill|noun|kỹ năng
education|B1|revise|verb|ôn tập
education|B2|assessment|noun|sự đánh giá
education|B2|vocational|adjective|thuộc hướng nghiệp
education|B2|literacy|noun|khả năng đọc viết
education|B2|undergraduate|noun|sinh viên đại học
education|B2|seminar|noun|buổi hội thảo
education|C1|methodology|noun|phương pháp luận
education|C1|cognition|noun|nhận thức
education|C1|accreditation|noun|sự kiểm định
education|C1|empirical|adjective|dựa trên thực nghiệm
education|C1|syllabus|noun|đề cương môn học
education|C2|heutagogy|noun|phương pháp học tự định hướng
education|C2|scholastic|adjective|thuộc học thuật
education|C2|metacognition|noun|siêu nhận thức
education|C2|andragogy|noun|phương pháp giáo dục người lớn
education|C2|elucidate|verb|làm sáng tỏ
technology|A1|computer|noun|máy tính
technology|A1|phone|noun|điện thoại
technology|A1|mouse|noun|chuột máy tính
technology|A1|keyboard|noun|bàn phím
technology|A1|app|noun|ứng dụng
technology|A1|internet|noun|mạng Internet
technology|A2|online|adjective|trực tuyến
technology|A2|file|noun|tệp tin
technology|A2|folder|noun|thư mục
technology|A2|search|verb|tìm kiếm
technology|A2|login|verb|đăng nhập
technology|A2|click|verb|nhấp chuột
technology|B1|database|noun|cơ sở dữ liệu
technology|B1|network|noun|mạng lưới
technology|B1|digital|adjective|kỹ thuật số
technology|B1|browser|noun|trình duyệt
technology|B1|storage|noun|bộ nhớ lưu trữ
technology|B2|encryption|noun|sự mã hóa
technology|B2|interface|noun|giao diện
technology|B2|analytics|noun|phân tích dữ liệu
technology|B2|processor|noun|bộ xử lý
technology|B2|cloud|noun|điện toán đám mây
technology|C1|scalability|noun|khả năng mở rộng
technology|C1|virtualization|noun|sự ảo hóa
technology|C1|latency|noun|độ trễ
technology|C1|bandwidth|noun|băng thông
technology|C1|architecture|noun|kiến trúc hệ thống
technology|C2|quantum|adjective|thuộc lượng tử
technology|C2|tokenization|noun|sự mã hóa thành token
technology|C2|obfuscation|noun|sự làm rối mã
technology|C2|deterministic|adjective|có tính tất định
technology|C2|computational|adjective|thuộc tính toán
environment|A1|river|noun|dòng sông
environment|A1|animal|noun|động vật
environment|A1|plant|noun|thực vật
environment|A1|water|noun|nước
environment|A1|earth|noun|Trái Đất; đất
environment|A1|plastic|noun|nhựa
environment|A2|nature|noun|thiên nhiên
environment|A2|energy|noun|năng lượng
environment|A2|ocean|noun|đại dương
environment|A2|weather|noun|thời tiết
environment|A2|reduce|verb|giảm bớt
environment|A2|reuse|verb|tái sử dụng
environment|B1|drought|noun|hạn hán
environment|B1|flood|noun|lũ lụt
environment|B1|wildlife|noun|động vật hoang dã
environment|B1|solar|adjective|thuộc năng lượng mặt trời
environment|B1|organic|adjective|hữu cơ
environment|B2|conservation|noun|sự bảo tồn
environment|B2|ecological|adjective|thuộc sinh thái
environment|B2|footprint|noun|dấu chân; mức tác động
environment|B2|landfill|noun|bãi chôn lấp rác
environment|B2|scarcity|noun|sự khan hiếm
environment|C1|mitigation|noun|sự giảm nhẹ tác động
environment|C1|resilience|noun|khả năng phục hồi
environment|C1|contamination|noun|sự ô nhiễm
environment|C1|restoration|noun|sự phục hồi
environment|C1|ecotourism|noun|du lịch sinh thái
environment|C2|rewilding|noun|tái hoang dã hóa
environment|C2|bioremediation|noun|xử lý ô nhiễm bằng sinh học
environment|C2|eutrophication|noun|sự phú dưỡng nguồn nước
environment|C2|anthropocene|noun|kỷ Nhân sinh
environment|C2|afforestation|noun|việc trồng rừng mới
""";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var topics = await context.VocabularyTopics.ToDictionaryAsync(topic => topic.Slug);
        var existingWords = await context.VocabularyWords
            .Select(word => new { word.TopicId, word.Word })
            .ToListAsync();
        var existingKeys = existingWords
            .Select(word => $"{word.TopicId}:{word.Word.ToLowerInvariant()}")
            .ToHashSet();
        var additions = new List<VocabularyWord>();

        foreach (var line in VocabularyData.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var values = line.Split('|');
            if (values.Length != 5 || !topics.TryGetValue(values[0], out var topic)) continue;
            var key = $"{topic.Id}:{values[2].ToLowerInvariant()}";
            if (!existingKeys.Add(key)) continue;

            additions.Add(new VocabularyWord
            {
                TopicId = topic.Id,
                Level = Enum.Parse<EnglishLevel>(values[1]),
                Word = values[2],
                PartOfSpeech = values[3],
                MeaningVietnamese = values[4],
                Meaning = string.Empty,
                Pronunciation = string.Empty,
                ExampleSentence = string.Empty,
                ExampleTranslation = string.Empty
            });
        }

        if (additions.Count == 0) return;
        await context.VocabularyWords.AddRangeAsync(additions);
        await context.SaveChangesAsync();
    }
}
