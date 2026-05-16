using backend.Entities;
using backend.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>Seeds Vietnamese articles, tags, categories, author, and comments.</summary>
public static class ArticleSeeder
{
    /// <summary>Seeds all article-related data. Skips if any article already exists.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Articles.AnyAsync()) return;

        var now = DateTimeOffset.UtcNow;

        // ── Author ───────────────────────────────────────────────────────────
        var author = await db.Users.FirstOrDefaultAsync();
        if (author is null)
        {
            var hasher = new PasswordHasher<User>();
            author = new User
            {
                UserName    = "Ninh",
                Email       = "ninhlk@nik.com",
                Password    = hasher.HashPassword(null!, "Admin@123"),
                Bio         = "Lập trình viên, nhiếp ảnh gia và người yêu thích du lịch. Tôi chia sẻ những câu chuyện từ những chuyến đi và bài học từ công việc.",
                Avatar      = "https://res.cloudinary.com/djvpvcj9g/image/upload/v1778947411/593829710_707055712480882_4581758384697286757_n_jyqgwu.jpg",
                Slug        = "ninh".ToSlug(),
                CreatedDate = now,
                UpdatedDate = now,
            };
            db.Users.Add(author);
        }

        // ── Tags ─────────────────────────────────────────────────────────────
        var tagLapTrinh  = MakeTag("lap-trinh",        "Lập trình",         "Các bài viết về lập trình, code và công nghệ phần mềm.",       "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800", now);
        var tagAI        = MakeTag("tri-tue-nhan-tao", "Trí tuệ nhân tạo", "AI, machine learning và các xu hướng công nghệ mới nổi.",     "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=800", now);
        var tagDuLich    = MakeTag("du-lich",           "Du lịch",           "Câu chuyện và kinh nghiệm từ những chuyến đi khắp nơi.",       "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800", now);
        var tagNhiepAnh  = MakeTag("nhiep-anh",         "Nhiếp ảnh",         "Kỹ thuật và nghệ thuật nhiếp ảnh.",                           "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=800", now);
        var tagPhongCach = MakeTag("phong-cach-song",   "Phong cách sống",   "Thói quen, năng suất và cuộc sống hằng ngày.",                "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800", now);
        var tagThoiTrang = MakeTag("thoi-trang",        "Thời trang",        "Xu hướng và phong cách thời trang đương đại.",                "https://images.unsplash.com/photo-1529626455594-4ff0802cfb7e?w=800", now);

        db.Tags.AddRange(tagLapTrinh, tagAI, tagDuLich, tagNhiepAnh, tagPhongCach, tagThoiTrang);

        // ── Categories ───────────────────────────────────────────────────────
        var catCongNghe  = MakeCategory("cong-nghe", "Công nghệ",  "Bài viết về lập trình, AI và các công nghệ mới nổi.",          "https://images.unsplash.com/photo-1518770660439-4636190af475?w=800", now);
        var catDuLich    = MakeCategory("du-lich",   "Du lịch",    "Hướng dẫn du lịch, điểm đến và trải nghiệm trên đường.",       "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800", now);
        var catNhiepAnh  = MakeCategory("nhiep-anh", "Nhiếp ảnh",  "Kỹ thuật, thiết bị và nghệ thuật nhiếp ảnh.",                  "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=800", now);
        var catDoiSong   = MakeCategory("doi-song",  "Đời sống",   "Lối sống, tối giản và phát triển bản thân.",                   "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800", now);

        db.Categories.AddRange(catCongNghe, catDuLich, catNhiepAnh, catDoiSong);
        await db.SaveChangesAsync();

        // ── Articles ─────────────────────────────────────────────────────────
        var a0 = MakeArticle(
            "Tương lai của Trí tuệ nhân tạo trong cuộc sống hàng ngày",
            "Trí tuệ nhân tạo đang âm thầm định hình lại cách chúng ta làm việc, giao tiếp và sáng tạo — đây là những gì có thể xảy ra trong 5 năm tới.",
            "<p>AI không còn bó hẹp trong phòng nghiên cứu nữa. Từ trợ lý thông minh đến chẩn đoán y tế cá nhân hóa, các mô hình học máy đã ăn sâu vào cấu trúc của cuộc sống hiện đại. Chúng ta sẽ khám phá những xu hướng chủ đạo đang thúc đẩy sự áp dụng AI và ý nghĩa của chúng đối với người dùng hằng ngày.</p><p>Các mô hình ngôn ngữ lớn, mô hình khuếch tán và học tăng cường đang hội tụ theo những cách không thể tưởng tượng được vài năm trước. Tác động đến công việc tri thức, ngành công nghiệp sáng tạo và giáo dục là vô cùng sâu sắc và toàn diện.</p>",
            "https://images.unsplash.com/photo-1677442136019-21780ecad995?w=800",
            author.Id, see: 320, like: 45, heart: 28, now.AddDays(-30));

        var a1 = MakeArticle(
            "Khám phá Vịnh Hạ Long: Góc nhìn của một nhiếp ảnh gia",
            "Vịnh Hạ Long mang đến những cảnh quan bờ biển ấn tượng nhất Việt Nam — đây là tất cả những gì bạn cần để ghi lại khoảnh khắc hoàn hảo.",
            "<p>Những khối đá vôi sừng sững, làn nước xanh ngọc và ánh sáng giờ vàng biến Vịnh Hạ Long thành thiên đường của các nhiếp ảnh gia. Vịnh thay đổi sắc thái suốt cả ngày, từ làn sương huyền ảo buổi sáng đến màu xanh rực rỡ buổi chiều và sắc cam ấm áp khi hoàng hôn.</p><p>Chúng tôi chia sẻ những điểm ngắm cảnh đẹp nhất, thời điểm lý tưởng và những lựa chọn thiết bị tạo nên sự khác biệt giữa một bức ảnh thông thường và một tác phẩm nhiếp ảnh thực sự đáng nhớ.</p>",
            "https://images.unsplash.com/photo-1528127269322-539801943592?w=800",
            author.Id, see: 270, like: 38, heart: 22, now.AddDays(-25));

        var a2 = MakeArticle(
            "Nhiếp ảnh đường phố: Bắt trọn khoảnh khắc thật",
            "Nhiếp ảnh đường phố là một trong những thể loại thú vị và đầy thách thức nhất — những kỹ thuật này sẽ giúp bạn ghi lại những khoảnh khắc chân thực nhất.",
            "<p>Khoảnh khắc quyết định, như Cartier-Bresson gọi, là sự kết hợp giữa kỹ thuật và trực giác. Lấy nét trước ở khoảng cách cố định, làm việc với ánh sáng tự nhiên và học cách trở nên vô hình trước đối tượng là những kỹ năng cần thời gian nhưng tạo ra sự thay đổi đáng kể trong kết quả.</p><p>Hướng dẫn này bao gồm tất cả mọi thứ từ việc lựa chọn ống kính đến đạo đức của việc chụp ảnh người lạ ở nơi công cộng, giúp bạn tự tin hơn khi bước ra đường phố với máy ảnh.</p>",
            "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?w=800",
            author.Id, see: 210, like: 30, heart: 18, now.AddDays(-20));

        var a3 = MakeArticle(
            "Xây dựng thói quen buổi sáng thực sự hiệu quả",
            "Hầu hết các thói quen buổi sáng thất bại vì chúng bỏ qua nhịp sinh học cá nhân — đây là cách thiết kế một thói quen thực sự bền vững.",
            "<p>Chìa khóa để duy trì thói quen buổi sáng là hiểu nhịp năng lượng tự nhiên của bạn thay vì sao chép lịch trình dậy lúc 5 giờ sáng của người khác. Nghiên cứu về nhịp sinh học cho thấy thời điểm thức dậy tối ưu khác nhau đáng kể giữa các cá nhân.</p><p>Bài viết này hướng dẫn các chiến lược dựa trên bằng chứng khoa học để xây dựng thói quen giúp nâng cao thay vì làm cạn kiệt năng lượng của bạn suốt cả ngày dài làm việc.</p>",
            "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800",
            author.Id, see: 180, like: 25, heart: 15, now.AddDays(-18));

        var a4 = MakeArticle(
            "TypeScript 5.0: Những tính năng mới đáng chú ý",
            "TypeScript 5.0 mang đến decorator, const type parameters và cải thiện hiệu suất đáng kể — đây là hướng dẫn thực tế chi tiết.",
            "<p>TypeScript 5.0 là bản phát hành quan trọng nhất kể từ phiên bản 4.0. Tiêu chuẩn decorator mới cuối cùng đã phù hợp với đề xuất TC39, xử lý enum được cải thiện loại bỏ một lớp lỗi lâu năm, và giảm kích thước bundle cải thiện hiệu suất khởi động nguội trên các dự án lớn.</p><p>Chúng tôi đi qua từng thay đổi với ví dụ code thực tế và mẹo di chuyển phiên bản, giúp bạn nâng cấp dự án một cách suôn sẻ và tận dụng tối đa các tính năng mới.</p>",
            "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800",
            author.Id, see: 290, like: 42, heart: 26, now.AddDays(-15));

        var a5 = MakeArticle(
            "Nhật Bản mùa thu: Một tuần trải nghiệm tại Kyoto",
            "Sắc màu mùa thu Kyoto nổi tiếng khắp thế giới — đây là lịch trình từng ngày để trải nghiệm trọn vẹn nhất mùa lá đỏ.",
            "<p>Từ những cổng torii màu đỏ son của Fushimi Inari phản chiếu trên lớp lá rụng đến kim các lâu Kinkakuji được bao quanh bởi cây phong rực rỡ, mùa thu Kyoto là trải nghiệm độc đáo không nơi nào trên trái đất có được. Mùa koyo thường đạt đỉnh vào giữa tháng 11.</p><p>Lịch trình từng ngày này cân bằng giữa các địa danh nổi tiếng và những vườn đền yên tĩnh mà hầu hết du khách thường bỏ lỡ trong hành trình của mình.</p>",
            "https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=800",
            author.Id, see: 340, like: 50, heart: 32, now.AddDays(-12));

        var a6 = MakeArticle(
            "Docker cho lập trình viên: Từ cơ bản đến thực chiến",
            "Container đã thay đổi cách chúng ta xây dựng và triển khai phần mềm — hướng dẫn này giải thích Docker rõ ràng cho lập trình viên.",
            "<p>Docker giải quyết vấn đề \"chạy được trên máy tôi\" bằng cách đóng gói ứng dụng cùng toàn bộ môi trường runtime vào các container có thể di chuyển được. Hiểu về image, layer, volume và networking mở ra một cách phát triển và triển khai phần mềm tốt hơn về cơ bản.</p><p>Hướng dẫn này dành cho các lập trình viên đã nghe về Docker nhưng chưa đưa nó vào quy trình làm việc hằng ngày của mình, với các ví dụ thực tế từ đơn giản đến phức tạp.</p>",
            "https://images.unsplash.com/photo-1605745341112-85968b19335b?w=800",
            author.Id, see: 260, like: 36, heart: 21, now.AddDays(-10));

        var a7 = MakeArticle(
            "Nghệ thuật nhiếp ảnh phong cảnh",
            "Nhiếp ảnh phong cảnh đỉnh cao không chỉ là hướng máy vào cảnh đẹp — bố cục, ánh sáng và sự kiên nhẫn mới là yếu tố quyết định.",
            "<p>Hiểu mối quan hệ giữa tiền cảnh, đường dẫn hướng và bầu trời là nền tảng của nhiếp ảnh phong cảnh đầy sức hút. Những giờ vàng — giờ đầu và giờ cuối của ánh sáng ban ngày — mang đến ánh sáng có hướng mềm mại biến những cảnh bình thường thành phi thường.</p><p>Chúng tôi chia sẻ kỹ thuật cho mọi trình độ, từ quy tắc một phần ba cơ bản đến xếp chồng kính lọc ND nâng cao, giúp bạn chụp được những bức ảnh phong cảnh ấn tượng hơn.</p>",
            "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800",
            author.Id, see: 195, like: 28, heart: 17, now.AddDays(-8));

        var a8 = MakeArticle(
            "Sống tối giản trong thế giới số",
            "Sự lộn xộn kỹ thuật số tiêu tốn của bạn nhiều hơn bạn nghĩ — đây là cách áp dụng chủ nghĩa tối giản vào thiết bị và thói quen trực tuyến.",
            "<p>Giữa thông báo, dịch vụ đăng ký và nguồn cấp thuật toán vô tận, môi trường kỹ thuật số của chúng ta đã trở nên quá tải. Gánh nặng nhận thức của việc quản lý cuộc sống kỹ thuật số lộn xộn âm thầm làm cạn kiệt sự chú ý và năng lượng sáng tạo.</p><p>Hướng dẫn này cung cấp các bước thực tế để đơn giản hóa thế giới kỹ thuật số của bạn — từ màn hình chính đến hộp thư đến thói quen mạng xã hội hằng ngày.</p>",
            "https://images.unsplash.com/photo-1484480974693-6ca0a78fb36b?w=800",
            author.Id, see: 155, like: 22, heart: 13, now.AddDays(-6));

        var a9 = MakeArticle(
            "Hành trình xuyên Việt trên chiếc xe máy",
            "Chạy xe từ Bắc vào Nam theo đường Hồ Chí Minh là một trong những cuộc phiêu lưu vĩ đại nhất Đông Nam Á — đây là cách lên kế hoạch.",
            "<p>Đường Hồ Chí Minh xuyên qua những đèo núi hẻo lánh, những bản làng miền cao yên bình và một số cảnh quan ấn tượng nhất Đông Nam Á. Cưỡi xe số tay từ Hà Nội vào thành phố Hồ Chí Minh là nghi thức của những người đam mê du lịch Việt Nam.</p><p>Chúng tôi chia sẻ kế hoạch lộ trình chi tiết, trang bị cần thiết, những chỗ nghỉ tốt nhất và những điểm dừng ẩn mà hầu hết tour du lịch thường bỏ qua trong hành trình của họ.</p>",
            "https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=800",
            author.Id, see: 230, like: 33, heart: 20, now.AddDays(-4));

        var a10 = MakeArticle(
            "React và Angular năm 2025: So sánh toàn diện",
            "Hai framework frontend phổ biến nhất tiếp tục phát triển — đây là so sánh trung thực cho các nhóm đang lựa chọn công nghệ năm 2025.",
            "<p>Sự linh hoạt của React và cấu trúc opinionated của Angular phục vụ những nhu cầu nhóm thực sự khác nhau. Độ rộng hệ sinh thái của React và công cụ tích hợp sẵn của Angular đại diện cho những triết lý khác nhau về nơi các quyết định kiến trúc nên được đặt ra.</p><p>Bài viết này so sánh trải nghiệm lập trình viên, đặc điểm hiệu suất, độ trưởng thành của hệ sinh thái và khả năng bảo trì lâu dài để giúp bạn đưa ra quyết định sáng suốt cho dự án tiếp theo.</p>",
            "https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=800",
            author.Id, see: 300, like: 44, heart: 27, now.AddDays(-2));

        var a11 = MakeArticle(
            "Ánh sáng và bóng tối: Căn bản nhiếp ảnh chân dung",
            "Hiểu cách ánh sáng đổ bóng lên khuôn mặt người là kỹ năng quan trọng nhất trong nhiếp ảnh chân dung — dù bạn ở trình độ nào.",
            "<p>Dù bạn chụp với ánh sáng cửa sổ tự nhiên hay đèn studio, những mẫu ánh sáng cổ điển — Rembrandt, loop, butterfly và split — đều áp dụng như nhau. Mỗi kiểu tạo ra tâm trạng khác nhau và tôn nét đẹp khác nhau tùy hình dạng khuôn mặt.</p><p>Hướng dẫn này phân tích từng mẫu ánh sáng với sơ đồ cài đặt thực tế và ví dụ hình ảnh cụ thể, để bạn có thể áp dụng với bất kỳ nguồn sáng nào có sẵn trong điều kiện thực tế.</p>",
            "https://images.unsplash.com/photo-1531746020798-e6953c6e8e04?w=800",
            author.Id, see: 175, like: 26, heart: 16, now.AddDays(-1));

        db.Articles.AddRange(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);

        // ── Tag / Category junctions ──────────────────────────────────────────
        AddJunctions(db, a0,  tags: [tagAI, tagLapTrinh],         cats: [catCongNghe]);
        AddJunctions(db, a1,  tags: [tagDuLich, tagNhiepAnh],     cats: [catDuLich]);
        AddJunctions(db, a2,  tags: [tagNhiepAnh, tagPhongCach],  cats: [catNhiepAnh]);
        AddJunctions(db, a3,  tags: [tagPhongCach],               cats: [catDoiSong]);
        AddJunctions(db, a4,  tags: [tagLapTrinh],                cats: [catCongNghe]);
        AddJunctions(db, a5,  tags: [tagDuLich, tagNhiepAnh],     cats: [catDuLich]);
        AddJunctions(db, a6,  tags: [tagLapTrinh],                cats: [catCongNghe]);
        AddJunctions(db, a7,  tags: [tagNhiepAnh],                cats: [catNhiepAnh]);
        AddJunctions(db, a8,  tags: [tagPhongCach],               cats: [catDoiSong]);
        AddJunctions(db, a9,  tags: [tagDuLich],                  cats: [catDuLich]);
        AddJunctions(db, a10, tags: [tagLapTrinh],                cats: [catCongNghe]);
        AddJunctions(db, a11, tags: [tagNhiepAnh],                cats: [catNhiepAnh]);

        tagLapTrinh.CountRef  = 4; tagAI.CountRef       = 1;
        tagDuLich.CountRef    = 3; tagNhiepAnh.CountRef = 5;
        tagPhongCach.CountRef = 3; tagThoiTrang.CountRef = 0;

        catCongNghe.CountRef = 4; catDuLich.CountRef  = 3;
        catNhiepAnh.CountRef = 3; catDoiSong.CountRef = 2;

        // ── Comments ──────────────────────────────────────────────────────────
        foreach (var article in new[] { a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11 })
        {
            var c1 = MakeComment(article.Id, author.Id,
                "Bài viết rất hay và bổ ích! Tôi đã theo dõi chủ đề này từ lâu và bài viết đã nêu bật tất cả các điểm quan trọng.",
                null, now.AddHours(-20));
            var c2 = MakeComment(article.Id, author.Id,
                "Phân tích rất sâu sắc. Mong được đọc phần tiếp theo tập trung vào cách triển khai thực tế hơn.",
                null, now.AddHours(-16));
            var c3 = MakeComment(article.Id, author.Id,
                "Đúng thứ tôi đang tìm kiếm. Đã lưu lại và chia sẻ với cả nhóm ngay rồi.",
                null, now.AddHours(-10));
            var r1 = MakeComment(article.Id, author.Id,
                "Cảm ơn bạn! Phần tiếp theo đang trong quá trình viết, theo dõi nhé.",
                c1.Id, now.AddHours(-8));
            var r2 = MakeComment(article.Id, author.Id,
                "Rất vui vì bài viết có ích. Sẽ có thêm nội dung thực tế hơn trong các bài sau.",
                c2.Id, now.AddHours(-4));

            db.Comments.AddRange(c1, c2, c3, r1, r2);
            article.CountCommentRef = 5;
        }

        await db.SaveChangesAsync();
    }

    // ── Factories ─────────────────────────────────────────────────────────────

    private static Article MakeArticle(
        string title, string description, string content, string image, string authorId,
        int see, int like, int heart, DateTimeOffset createdDate) => new()
    {
        Title         = title,
        Description   = description,
        Content       = content,
        Image         = image,
        Slug          = title.ToSlug(),
        AuthorId      = authorId,
        CountSee      = see,
        CountLikeRef  = like,
        CountHeartRef = heart,
        CreatedDate   = createdDate,
        UpdatedDate   = createdDate,
    };

    private static Tag MakeTag(string name, string title, string description, string image, DateTimeOffset now) => new()
    {
        Name        = name,
        Title       = title,
        Description = description,
        Image       = image,
        Slug        = name.ToSlug(),
        CreatedDate = now,
        UpdatedDate = now,
    };

    private static Category MakeCategory(string name, string title, string description, string image, DateTimeOffset now) => new()
    {
        Name        = name,
        Title       = title,
        Description = description,
        Image       = image,
        Slug        = name.ToSlug(),
        CreatedDate = now,
        UpdatedDate = now,
    };

    private static Comment MakeComment(string articleId, string authorId, string text, string? parentId, DateTimeOffset createdDate) => new()
    {
        ArticleId   = articleId,
        AuthorId    = authorId,
        Text        = text,
        ParentId    = parentId,
        CreatedDate = createdDate,
    };

    private static void AddJunctions(ApplicationDbContext db, Article article, Tag[] tags, Category[] cats)
    {
        foreach (var tag in tags)
            db.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
        foreach (var cat in cats)
            db.ArticleCategories.Add(new ArticleCategory { ArticleId = article.Id, CategoryId = cat.Id });
    }
}
