using BandBoostAI.Domain.Entities;
using BandBoostAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BandBoostAI.Infrastructure.Persistence;

public static class SupplementalVocabularySeeder
{
    private const string VocabularyData = """
daily-life|A1|breakfast|noun|bữa sáng|I have breakfast with my family every morning.|Tôi ăn sáng cùng gia đình mỗi buổi sáng.
daily-life|A1|shower|noun|vòi sen; việc tắm|I take a quick shower before work.|Tôi tắm nhanh trước khi đi làm.
daily-life|A2|appointment|noun|cuộc hẹn|I have a dentist appointment at three o'clock.|Tôi có lịch hẹn nha sĩ lúc ba giờ.
daily-life|A2|household|noun|hộ gia đình; việc nhà|Everyone helps with the household chores.|Mọi người đều giúp làm việc nhà.
daily-life|B1|balance|noun|sự cân bằng|A good balance between study and rest improves focus.|Sự cân bằng tốt giữa học và nghỉ giúp tăng tập trung.
daily-life|B1|organize|verb|sắp xếp, tổ chức|I organize my tasks before starting the day.|Tôi sắp xếp công việc trước khi bắt đầu ngày mới.
daily-life|B2|prioritize|verb|ưu tiên|You should prioritize the most important tasks.|Bạn nên ưu tiên những công việc quan trọng nhất.
daily-life|B2|efficient|adjective|hiệu quả|This routine is simple but highly efficient.|Thói quen này đơn giản nhưng rất hiệu quả.
daily-life|C1|spontaneous|adjective|tự phát, ngẫu hứng|We made a spontaneous decision to cook together.|Chúng tôi ngẫu hứng quyết định nấu ăn cùng nhau.
daily-life|C1|rejuvenate|verb|làm trẻ lại, phục hồi năng lượng|A short walk can rejuvenate you after a long day.|Một buổi đi bộ ngắn có thể giúp bạn phục hồi năng lượng sau ngày dài.
daily-life|C2|humdrum|adjective|đơn điệu, nhàm chán|She adds small adventures to her humdrum routine.|Cô ấy thêm những cuộc phiêu lưu nhỏ vào nhịp sống đơn điệu.
daily-life|C2|fastidious|adjective|cầu kỳ, kỹ tính|He is fastidious about keeping his workspace clean.|Anh ấy rất kỹ tính trong việc giữ nơi làm việc sạch sẽ.
travel|A1|ticket|noun|vé|I bought a train ticket to Hue.|Tôi đã mua vé tàu đi Huế.
travel|A1|passport|noun|hộ chiếu|Please show your passport at the counter.|Vui lòng xuất trình hộ chiếu tại quầy.
travel|A2|reservation|noun|sự đặt chỗ|We made a hotel reservation online.|Chúng tôi đã đặt phòng khách sạn trực tuyến.
travel|A2|departure|noun|sự khởi hành|Our departure is scheduled for nine o'clock.|Chuyến khởi hành của chúng tôi được lên lịch lúc chín giờ.
travel|B1|accommodation|noun|chỗ ở|The website offers affordable accommodation near the beach.|Trang web cung cấp chỗ ở giá phải chăng gần biển.
travel|B1|explore|verb|khám phá|We spent the afternoon exploring the old town.|Chúng tôi dành buổi chiều khám phá phố cổ.
travel|B2|excursion|noun|chuyến tham quan ngắn|The cruise includes an excursion to a nearby island.|Chuyến du thuyền gồm một chuyến tham quan đảo gần đó.
travel|B2|hospitality|noun|lòng hiếu khách|We were impressed by the warmth and hospitality of local people.|Chúng tôi ấn tượng với sự nồng hậu và hiếu khách của người dân địa phương.
travel|C1|breathtaking|adjective|ngoạn mục|The mountain view from our room was breathtaking.|Cảnh núi nhìn từ phòng chúng tôi thật ngoạn mục.
travel|C1|immerse|verb|đắm mình|Travelling allows you to immerse yourself in another culture.|Du lịch cho phép bạn đắm mình trong một nền văn hóa khác.
travel|C2|serendipitous|adjective|tình cờ nhưng may mắn|A serendipitous meeting led us to a hidden café.|Một cuộc gặp tình cờ may mắn đã đưa chúng tôi đến quán cà phê bí mật.
travel|C2|cosmopolitan|adjective|mang tính quốc tế, đa văn hóa|Singapore is a vibrant and cosmopolitan city.|Singapore là một thành phố sôi động và đa văn hóa.
work-career|A1|salary|noun|tiền lương|My salary is paid at the end of each month.|Lương của tôi được trả vào cuối mỗi tháng.
work-career|A1|meeting|noun|cuộc họp|We have a team meeting every Monday.|Chúng tôi họp nhóm vào mỗi thứ Hai.
work-career|A2|interview|noun|buổi phỏng vấn|She prepared carefully for her job interview.|Cô ấy chuẩn bị kỹ cho buổi phỏng vấn xin việc.
work-career|A2|schedule|noun|lịch trình|My work schedule is flexible this week.|Lịch làm việc của tôi tuần này khá linh hoạt.
work-career|B1|responsibility|noun|trách nhiệm|Managing the budget is one of my responsibilities.|Quản lý ngân sách là một trong những trách nhiệm của tôi.
work-career|B1|negotiate|verb|đàm phán|They negotiated a better contract with the supplier.|Họ đã đàm phán hợp đồng tốt hơn với nhà cung cấp.
work-career|B2|productivity|noun|năng suất|Regular breaks can improve workplace productivity.|Nghỉ giải lao đều đặn có thể cải thiện năng suất làm việc.
work-career|B2|delegate|verb|giao phó, ủy quyền|Good managers know when to delegate tasks.|Nhà quản lý giỏi biết khi nào cần giao việc.
work-career|C1|remuneration|noun|thù lao|The role offers competitive remuneration and benefits.|Vị trí này cung cấp thù lao và phúc lợi cạnh tranh.
work-career|C1|consensus|noun|sự đồng thuận|The team reached a consensus after a lengthy discussion.|Nhóm đã đạt được đồng thuận sau cuộc thảo luận dài.
work-career|C2|micromanage|verb|quản lý quá chi tiết|Leaders who micromanage can reduce employee motivation.|Lãnh đạo quản lý quá chi tiết có thể làm giảm động lực nhân viên.
work-career|C2|meritocracy|noun|chế độ trọng dụng nhân tài|The company aims to build a genuine meritocracy.|Công ty hướng tới xây dựng một môi trường thực sự trọng dụng nhân tài.
education|A1|student|noun|học sinh, sinh viên|Every student has a new English book.|Mỗi học sinh đều có một cuốn sách tiếng Anh mới.
education|A1|homework|noun|bài tập về nhà|I finish my homework before dinner.|Tôi hoàn thành bài tập về nhà trước bữa tối.
education|A2|textbook|noun|sách giáo khoa|This textbook contains useful practice exercises.|Cuốn sách giáo khoa này có các bài tập hữu ích.
education|A2|graduate|verb|tốt nghiệp|She hopes to graduate next summer.|Cô ấy hy vọng tốt nghiệp vào mùa hè tới.
education|B1|scholarship|noun|học bổng|He received a scholarship to study abroad.|Anh ấy nhận được học bổng du học.
education|B1|concentrate|verb|tập trung|It is easier to concentrate in a quiet room.|Sẽ dễ tập trung hơn trong một căn phòng yên tĩnh.
education|B2|dissertation|noun|luận văn|Her dissertation examines language learning habits.|Luận văn của cô ấy nghiên cứu thói quen học ngôn ngữ.
education|B2|compulsory|adjective|bắt buộc|English is a compulsory subject at this school.|Tiếng Anh là môn học bắt buộc tại trường này.
education|C1|plagiarism|noun|đạo văn|Universities use software to detect plagiarism.|Các trường đại học dùng phần mềm để phát hiện đạo văn.
education|C1|interdisciplinary|adjective|liên ngành|The course takes an interdisciplinary approach to climate change.|Khóa học tiếp cận biến đổi khí hậu theo hướng liên ngành.
education|C2|epistemology|noun|nhận thức luận|The seminar introduced students to modern epistemology.|Hội thảo giới thiệu sinh viên với nhận thức luận hiện đại.
education|C2|didactic|adjective|mang tính giáo huấn|The novel is engaging without being overly didactic.|Cuốn tiểu thuyết hấp dẫn mà không quá giáo huấn.
technology|A1|screen|noun|màn hình|The message appears on the screen.|Tin nhắn xuất hiện trên màn hình.
technology|A1|password|noun|mật khẩu|Never share your password with anyone.|Đừng bao giờ chia sẻ mật khẩu với người khác.
technology|A2|website|noun|trang web|The school has launched a new website.|Trường đã ra mắt một trang web mới.
technology|A2|upload|verb|tải lên|Please upload your document before Friday.|Vui lòng tải tài liệu lên trước thứ Sáu.
technology|B1|software|noun|phần mềm|This software helps us manage customer data.|Phần mềm này giúp chúng tôi quản lý dữ liệu khách hàng.
technology|B1|update|verb|cập nhật|Remember to update the app regularly.|Hãy nhớ cập nhật ứng dụng thường xuyên.
technology|B2|algorithm|noun|thuật toán|The platform uses an algorithm to recommend lessons.|Nền tảng sử dụng thuật toán để đề xuất bài học.
technology|B2|cybersecurity|noun|an ninh mạng|Cybersecurity training is essential for all employees.|Đào tạo an ninh mạng rất cần thiết cho mọi nhân viên.
technology|C1|automation|noun|sự tự động hóa|Automation has transformed many manufacturing processes.|Tự động hóa đã thay đổi nhiều quy trình sản xuất.
technology|C1|interoperability|noun|khả năng tương tác|Interoperability allows different systems to exchange data.|Khả năng tương tác cho phép các hệ thống khác nhau trao đổi dữ liệu.
technology|C2|cryptography|noun|mật mã học|Modern cryptography protects sensitive digital information.|Mật mã học hiện đại bảo vệ thông tin số nhạy cảm.
technology|C2|decentralization|noun|sự phi tập trung|Blockchain is often associated with decentralization.|Blockchain thường gắn liền với sự phi tập trung.
environment|A1|forest|noun|rừng|Many animals live in this forest.|Nhiều loài động vật sống trong khu rừng này.
environment|A1|waste|noun|rác thải, sự lãng phí|We should reduce food waste at home.|Chúng ta nên giảm lãng phí thực phẩm tại nhà.
environment|A2|climate|noun|khí hậu|The island has a warm and humid climate.|Hòn đảo có khí hậu ấm và ẩm.
environment|A2|protect|verb|bảo vệ|We must protect rivers from pollution.|Chúng ta phải bảo vệ sông khỏi ô nhiễm.
environment|B1|habitat|noun|môi trường sống|The wetland is an important habitat for birds.|Vùng đất ngập nước là môi trường sống quan trọng của chim.
environment|B1|conserve|verb|bảo tồn, tiết kiệm|Turning off unused lights helps conserve energy.|Tắt đèn không sử dụng giúp tiết kiệm năng lượng.
environment|B2|emissions|noun|khí thải|The policy aims to reduce carbon emissions.|Chính sách nhằm giảm lượng khí thải carbon.
environment|B2|ecosystem|noun|hệ sinh thái|Plastic waste can damage the marine ecosystem.|Rác thải nhựa có thể gây hại cho hệ sinh thái biển.
environment|C1|deforestation|noun|nạn phá rừng|Deforestation threatens wildlife and local communities.|Nạn phá rừng đe dọa động vật hoang dã và cộng đồng địa phương.
environment|C1|biodegradable|adjective|có thể phân hủy sinh học|These bags are made from biodegradable materials.|Những chiếc túi này được làm từ vật liệu có thể phân hủy sinh học.
environment|C2|desertification|noun|sự sa mạc hóa|Poor land management can accelerate desertification.|Quản lý đất kém có thể đẩy nhanh quá trình sa mạc hóa.
environment|C2|decarbonization|noun|quá trình khử carbon|Decarbonization requires investment in clean energy.|Quá trình khử carbon cần đầu tư vào năng lượng sạch.
""";

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var topics = await context.VocabularyTopics.ToDictionaryAsync(topic => topic.Slug);
        if (topics.Count == 0) return;

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
            if (values.Length != 7 || !topics.TryGetValue(values[0], out var topic)) continue;
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
                ExampleSentence = values[5],
                ExampleTranslation = values[6]
            });
        }

        if (additions.Count == 0) return;
        await context.VocabularyWords.AddRangeAsync(additions);
        await context.SaveChangesAsync();
    }
}
