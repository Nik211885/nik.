using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds default languages, translation keys, and admin-UI translations idempotently on startup.
/// </summary>
public static class LanguageSeeder
{
    /// <summary>
    /// Ensures all admin UI translation keys and their English/Vietnamese values exist in the database.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var enLang = await EnsureLanguageAsync(db, "en", "English");
        var viLang = await EnsureLanguageAsync(db, "vi", "Tiếng Việt");
        await db.SaveChangesAsync();

        var existingCodes = await db.CodeLanguages.AsNoTracking()
            .ToDictionaryAsync(x => x.Code, x => x.Id);

        var existingTranslates = await db.Translates.AsNoTracking()
            .Select(x => new { x.CodeId, x.LanguageId })
            .ToListAsync();
        var translateSet = existingTranslates
            .Select(x => $"{x.CodeId}:{x.LanguageId}")
            .ToHashSet();

        foreach (var (key, en, vi) in GetEntries())
        {
            if (!existingCodes.TryGetValue(key, out var codeId))
            {
                var entity = new CodeLanguage { Code = key };
                db.CodeLanguages.Add(entity);
                codeId = entity.Id;
                existingCodes[key] = codeId;
            }

            var enKey = $"{codeId}:{enLang.Id}";
            if (!translateSet.Contains(enKey))
            {
                db.Translates.Add(new Translate { CodeId = codeId, LanguageId = enLang.Id, Value = en });
                translateSet.Add(enKey);
            }

            var viKey = $"{codeId}:{viLang.Id}";
            if (!translateSet.Contains(viKey))
            {
                db.Translates.Add(new Translate { CodeId = codeId, LanguageId = viLang.Id, Value = vi });
                translateSet.Add(viKey);
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task<Language> EnsureLanguageAsync(ApplicationDbContext db, string code, string name)
    {
        var existing = await db.Languages.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null) return existing;
        var lang = new Language { Code = code, Name = name };
        db.Languages.Add(lang);
        return lang;
    }

    /// <summary>Returns all admin-UI translation entries as (key, english, vietnamese) tuples.</summary>
    private static (string Key, string En, string Vi)[] GetEntries() =>
    [
        // ─── Shared ───────────────────────────────────────────────────────────
        ("admin.save",              "Save",                                     "Lưu"),
        ("admin.saving",            "Saving...",                                "Đang lưu..."),
        ("admin.cancel",            "Cancel",                                   "Hủy"),
        ("admin.error.generic",     "An error occurred, please try again.",     "Có lỗi xảy ra, vui lòng thử lại."),
        ("admin.loading",           "Loading...",                               "Đang tải..."),
        ("admin.label.name",        "Name",                                     "Tên"),
        ("admin.label.title",       "Title",                                    "Tiêu đề"),
        ("admin.label.description", "Description",                              "Mô tả"),
        ("admin.label.image",       "Image",                                    "Hình ảnh"),
        ("admin.label.order",       "Order",                                    "Thứ tự"),
        ("admin.label.key",           "Key",                "Khóa"),
        ("admin.label.value",         "Value",              "Giá trị"),
        ("admin.label.language",      "Language",           "Ngôn ngữ"),
        ("admin.label.slug",          "Slug",               "Slug"),
        ("admin.label.views",         "Views",              "Lượt xem"),
        ("admin.label.like",          "Likes",              "Lượt thích"),
        ("admin.label.comment-count", "Comments",           "Bình luận"),
        ("admin.label.created-date",  "Created Date",       "Ngày tạo"),
        ("admin.label.image-count",   "Images",             "Số ảnh"),
        ("admin.label.article-count", "Articles",           "Số bài viết"),
        ("admin.label.content",       "Content",            "Nội dung"),
        ("admin.label.article-id",    "Article ID",         "ID bài viết"),
        ("admin.label.author",        "Author",             "Tác giả"),
        ("admin.label.reply-id",      "Reply ID",           "ID trả lời"),
        ("admin.label.lang-code",     "Language Code",      "Mã ngôn ngữ"),
        ("admin.label.username",      "Username",           "Tên đăng nhập"),
        ("admin.label.email",         "Email",              "Email"),
        ("admin.label.phone",         "Phone",              "Điện thoại"),
        ("admin.label.bio",           "Bio",                "Giới thiệu"),

        // ─── Dashboard ────────────────────────────────────────────────────────
        ("admin.dashboard.title",            "Dashboard",           "Bảng điều khiển"),
        ("admin.dashboard.subtitle",         "System overview",     "Tổng quan hệ thống"),
        ("admin.dashboard.quick",            "Quick Actions",       "Thao tác nhanh"),
        ("admin.dashboard.new-article",      "New Article",         "Viết bài mới"),
        ("admin.dashboard.new-album",        "New Album",           "Tạo album"),
        ("admin.dashboard.new-category",     "New Category",        "Tạo danh mục"),
        ("admin.dashboard.new-tag",          "New Tag",             "Tạo thẻ tag"),
        ("admin.dashboard.settings",         "Settings",            "Cấu hình"),
        ("admin.dashboard.stat.articles",    "Articles",            "Bài viết"),
        ("admin.dashboard.stat.albums",      "Albums",              "Album"),
        ("admin.dashboard.stat.categories",  "Categories",          "Danh mục"),
        ("admin.dashboard.stat.tags",        "Tags",                "Thẻ tag"),
        ("admin.dashboard.stat.files",       "Files",               "Tệp"),
        ("admin.dashboard.stat.users",       "Users",               "Người dùng"),

        // ─── Articles ─────────────────────────────────────────────────────────
        ("admin.articles.title",             "Articles",            "Bài viết"),
        ("admin.articles.create",            "New Article",         "Viết bài mới"),
        ("admin.articles.modal.create",      "Create Article",      "Tạo bài viết"),
        ("admin.articles.modal.edit",        "Edit Article",        "Chỉnh sửa bài viết"),
        ("admin.articles.form.title",        "Title",               "Tiêu đề"),
        ("admin.articles.form.desc",         "Description",         "Mô tả"),
        ("admin.articles.form.content",      "Content",             "Nội dung"),
        ("admin.articles.form.cover",        "Cover Image",         "Ảnh bìa"),
        ("admin.articles.form.categories",   "Categories",          "Danh mục"),
        ("admin.articles.form.tags",         "Tags",                "Thẻ tag"),
        ("admin.articles.save",              "Save Article",        "Lưu bài viết"),

        // ─── Albums ───────────────────────────────────────────────────────────
        ("admin.albums.title",               "Albums",              "Album"),
        ("admin.albums.subtitle",            "Manage photo albums", "Quản lý album ảnh"),
        ("admin.albums.create",              "New Album",           "Tạo album"),
        ("admin.albums.modal.create",        "Create Album",        "Tạo album"),
        ("admin.albums.modal.edit",          "Edit Album",          "Chỉnh sửa album"),
        ("admin.albums.form.parent",         "Parent Album",              "Album cha"),
        ("admin.albums.form.no-parent",      "— No parent —",             "— Không có album cha —"),
        ("admin.albums.view.list",           "List",                      "Danh sách"),
        ("admin.albums.view.tree",           "Tree",                      "Cây"),
        ("admin.albums.view.explorer",       "Explorer",                  "Khám phá"),
        ("admin.albums.empty-children",      "No sub-albums",             "Không có album con"),
        ("admin.albums.files.title",         "Files",                     "Tệp"),
        ("admin.albums.files.empty",         "No files in this album.",   "Album này chưa có tệp nào."),
        ("admin.albums.files.add",           "Add Files",                 "Thêm tệp"),
        ("admin.albums.files.remove",        "Remove",                    "Xóa khỏi album"),
        ("admin.albums.root",                "Root",                      "Gốc"),
        ("admin.albums.expand-all",          "Expand all",                "Mở rộng tất cả"),
        ("admin.albums.collapse-all",        "Collapse",                  "Thu gọn"),
        ("admin.albums.copy-url",            "Copy URL",                  "Sao chép URL"),
        ("admin.albums.count-suffix",        "album",                     "album"),

        // ─── Comments ─────────────────────────────────────────────────────────
        ("admin.comments.title",              "Comments",                             "Bình luận"),
        ("admin.comments.subtitle",           "Find and delete comments by article",  "Tìm và xóa bình luận theo bài viết"),
        ("admin.comments.search-placeholder", "Enter article ID...",                  "Nhập ID bài viết..."),
        ("admin.comments.search",             "Search",                               "Tìm kiếm"),

        // ─── Tags ─────────────────────────────────────────────────────────────
        ("admin.tags.title",                 "Tags",                "Thẻ tag"),
        ("admin.tags.subtitle",              "Manage tags",         "Quản lý thẻ tag"),
        ("admin.tags.create",                "New Tag",             "Tạo thẻ tag"),
        ("admin.tags.modal.create",          "Create Tag",          "Tạo thẻ tag"),
        ("admin.tags.modal.edit",            "Edit Tag",            "Chỉnh sửa thẻ tag"),

        // ─── Categories ───────────────────────────────────────────────────────
        ("admin.categories.title",           "Categories",          "Danh mục"),
        ("admin.categories.subtitle",        "Manage categories",   "Quản lý danh mục"),
        ("admin.categories.create",          "New Category",        "Tạo danh mục"),
        ("admin.categories.modal.create",    "Create Category",     "Tạo danh mục"),
        ("admin.categories.modal.edit",      "Edit Category",       "Chỉnh sửa danh mục"),

        // ─── Users ────────────────────────────────────────────────────────────
        ("admin.users.title",                "Users",               "Người dùng"),
        ("admin.users.subtitle",             "Manage user accounts","Quản lý tài khoản"),
        ("admin.users.modal.edit",           "Edit User",           "Chỉnh sửa người dùng"),
        ("admin.users.form.username",        "Username",            "Tên đăng nhập"),
        ("admin.users.form.email",           "Email",               "Email"),
        ("admin.users.form.phone",           "Phone",               "Điện thoại"),
        ("admin.users.form.bio",             "Bio",                 "Giới thiệu"),

        // ─── Languages ────────────────────────────────────────────────────────
        ("admin.languages.title",            "Languages",                        "Ngôn ngữ"),
        ("admin.languages.subtitle",         "Manage interface languages",       "Quản lý ngôn ngữ giao diện"),
        ("admin.languages.create",           "New Language",                     "Thêm ngôn ngữ"),
        ("admin.languages.modal.create",     "Create Language",                  "Tạo ngôn ngữ"),
        ("admin.languages.modal.edit",       "Edit Language",                    "Chỉnh sửa ngôn ngữ"),
        ("admin.languages.form.code",        "Language Code",                    "Mã ngôn ngữ"),

        // ─── Translations ─────────────────────────────────────────────────────
        ("admin.translations.all-langs",          "All languages",            "Tất cả ngôn ngữ"),
        ("admin.translations.title",              "Translations",             "Bản dịch"),
        ("admin.translations.subtitle",           "Manage translation keys",  "Quản lý khóa dịch"),
        ("admin.translations.add-key",            "Add Key",                  "Thêm khóa"),
        ("admin.translations.create",             "New Translation",          "Thêm bản dịch"),
        ("admin.translations.modal.create",       "Create Translation",       "Tạo bản dịch"),
        ("admin.translations.modal.edit",         "Edit Translation",         "Chỉnh sửa bản dịch"),
        ("admin.translations.modal.key-title",    "Create Translation Key",   "Tạo khóa dịch"),
        ("admin.translations.form.value",         "Value",                    "Giá trị"),
        ("admin.translations.form.choose-key",    "Choose key",               "Chọn khóa"),
        ("admin.translations.form.choose-lang",   "Choose language",          "Chọn ngôn ngữ"),
        ("admin.translations.save-key",           "Save Key",                 "Lưu khóa"),

        // ─── Sys Config ───────────────────────────────────────────────────────
        ("admin.sys-config.title",           "System Config",                    "Cấu hình hệ thống"),
        ("admin.sys-config.subtitle",        "Manage system configuration",      "Quản lý cấu hình hệ thống"),
        ("admin.sys-config.create",          "New Config",                       "Thêm cấu hình"),
        ("admin.sys-config.modal.create",    "Create Config",                    "Tạo cấu hình"),
        ("admin.sys-config.modal.edit",      "Edit Config",                      "Chỉnh sửa cấu hình"),
        ("admin.sys-config.form.value-label","Value (JSON or text)",             "Giá trị (JSON hoặc văn bản)"),
        ("admin.sys-config.form.hint",       "JSON objects are parsed automatically.", "JSON sẽ được tự động phân tích cú pháp."),

        // ─── Files ────────────────────────────────────────────────────────────
        ("admin.files.title",                "Files",                            "Tệp"),
        ("admin.files.create",               "Upload",                           "Tải lên"),
        ("admin.files.modal.create",         "Upload File",                      "Tải lên tệp"),
        ("admin.files.modal.edit",           "Edit File",                        "Chỉnh sửa tệp"),
        ("admin.files.form.media",           "Media",                            "Tệp media"),
        ("admin.files.empty",                "No files uploaded yet.",           "Chưa có tệp nào được tải lên."),
        ("admin.files.drop",                 "Drop files here to upload",        "Thả tệp vào đây để tải lên"),
        ("admin.files.batch",                "Uploading",                        "Đang tải lên"),
        ("admin.files.upload-required",      "Please upload a file first.",      "Vui lòng tải tệp lên trước."),
        ("admin.files.save-failed",          "Save failed.",                     "Lưu thất bại."),

        // ─── Sidebar ──────────────────────────────────────────────────────────
        ("admin.sidebar.home",               "Home",                "Trang chủ"),
        ("admin.sidebar.logout",             "Logout",              "Đăng xuất"),
        ("admin.sidebar.nav.dashboard",      "Dashboard",           "Bảng điều khiển"),
        ("admin.sidebar.nav.articles",       "Articles",            "Bài viết"),
        ("admin.sidebar.nav.albums",         "Albums",              "Album"),
        ("admin.sidebar.nav.categories",     "Categories",          "Danh mục"),
        ("admin.sidebar.nav.tags",           "Tags",                "Thẻ tag"),
        ("admin.sidebar.nav.comments",       "Comments",            "Bình luận"),
        ("admin.sidebar.nav.files",          "Files",               "Tệp"),
        ("admin.sidebar.nav.users",          "Users",               "Người dùng"),
        ("admin.sidebar.nav.languages",      "Languages",           "Ngôn ngữ"),
        ("admin.sidebar.nav.translations",   "Translations",        "Bản dịch"),
        ("admin.sidebar.nav.settings",       "Settings",            "Cấu hình"),

        // ─── Login ────────────────────────────────────────────────────────────
        ("admin.login.title",                "Admin Login",                                  "Đăng nhập quản trị"),
        ("admin.login.subtitle",             "Sign in to manage your site",                  "Đăng nhập để quản lý trang web"),
        ("admin.login.email-label",          "Email or Username",                            "Email hoặc Tên đăng nhập"),
        ("admin.login.email-placeholder",    "Enter email or username",                      "Nhập email hoặc tên đăng nhập"),
        ("admin.login.password-label",       "Password",                                     "Mật khẩu"),
        ("admin.login.password-placeholder", "Enter password",                               "Nhập mật khẩu"),
        ("admin.login.submit",               "Sign In",                                      "Đăng nhập"),
        ("admin.login.submitting",           "Signing in...",                                "Đang đăng nhập..."),
        ("admin.login.back",                 "Back to site",                                 "Quay lại trang web"),
        ("admin.login.error",                "Invalid email/username or password.",          "Email/Tên đăng nhập hoặc mật khẩu không đúng."),

        // ─── Cloudinary Upload ────────────────────────────────────────────────
        ("admin.upload.change",              "Change",                                       "Thay đổi"),
        ("admin.upload.drag-title",          "Drop image or video here",                     "Thả ảnh hoặc video vào đây"),
        ("admin.upload.drag-sub",            "or <u>click to browse</u>",                   "hoặc <u>nhấp để chọn</u>"),
        ("admin.upload.drag-hint",           "JPG, PNG, WEBP, MP4",                         "JPG, PNG, WEBP, MP4"),
        ("admin.upload.uploading",           "Uploading",                                    "Đang tải lên"),
        ("admin.upload.manual-url",          "Or enter URL manually",                        "Hoặc nhập URL thủ công"),
        ("admin.upload.error",               "Upload failed.",                               "Tải lên thất bại."),

        // ─── Table ────────────────────────────────────────────────────────────
        ("admin.table.actions",              "Actions",             "Thao tác"),
        ("admin.table.loading",              "Loading...",          "Đang tải..."),
        ("admin.table.empty-data",           "No data found.",      "Không có dữ liệu."),
        ("admin.table.action.edit",          "Edit",                "Chỉnh sửa"),
        ("admin.table.action.delete",        "Delete",              "Xóa"),
        ("admin.table.per-page",             "Per page",            "Mỗi trang"),
        ("admin.table.selected",             "selected",            "đã chọn"),
        ("admin.table.deselect",             "Deselect all",        "Bỏ chọn"),

        // ─── Dialog ───────────────────────────────────────────────────────────
        ("admin.diaglog.delete.confirm-title",    "Confirm Delete",              "Xác nhận xóa"),
        ("admin.diaglog.delete.confirm-label",    "Are you sure?",               "Bạn có chắc không?"),
        ("admin.diaglog.delete.action.delete",    "Delete",                      "Xóa"),
        ("admin.diaglog.delete.action.cancel",    "Cancel",                      "Hủy"),
        ("admin.diaglog.delete.message",          "This action cannot be undone.", "Hành động này không thể hoàn tác."),

        // ─── ApplicationMessage (backend error keys) ─────────────────────────
        ("file.not.exists",    "File does not exist.",              "Tệp không tồn tại."),
        ("file.type.apply",    "File type is not allowed.",         "Loại tệp không được hỗ trợ."),
        ("file.max.size",      "File exceeds the maximum size.",    "Tệp vượt quá kích thước cho phép."),
        ("loading.process",    "Processing...",                     "Đang xử lý..."),

        // ─── Public navigation keys (used in config.sidebar) ─────────────────
        ("nav.home",                  "Home",                "Trang chủ"),
        ("nav.photography",           "Photography",         "Nhiếp ảnh"),
        ("nav.travel",                "Travel",              "Du lịch"),
        ("nav.fashion",               "Fashion",             "Thời trang"),
        ("nav.about",                 "About",               "Giới thiệu"),
        ("nav.contact",               "Contact",             "Liên hệ"),

        // ─── ApplicationTitle (public site keys) ─────────────────────────────
        ("category",                  "Category",                  "Danh mục"),
        ("archives",                  "Archives",                  "Lưu trữ"),
        ("have-questions?",           "Have a question?",          "Có câu hỏi?"),
        ("write-by",                  "Written by",                "Viết bởi"),
        ("continuing-reading",        "Continue reading",          "Đọc tiếp"),
        ("news-letter.title",         "Newsletter",                "Bản tin"),
        ("news-letter.content",       "Subscribe to get the latest posts delivered to your inbox.", "Đăng ký để nhận bài viết mới nhất."),
        ("email-address",             "Email address",             "Địa chỉ email"),
        ("subscribe",                 "Subscribe",                 "Đăng ký"),
        ("contact-information",       "Contact Information",       "Thông tin liên hệ"),
        ("address",                   "Address",                   "Địa chỉ"),
        ("phone",                     "Phone",                     "Điện thoại"),
        ("email",                     "Email",                     "Email"),
        ("website",                   "Website",                   "Trang web"),
        ("your-name",                 "Your Name",                 "Họ và tên"),
        ("your-email",                "Your Email",                "Email của bạn"),
        ("subject",                   "Subject",                   "Chủ đề"),
        ("message",                   "Message",                   "Tin nhắn"),
        ("send-message",              "Send Message",              "Gửi tin nhắn"),
        ("hello-i-am",                "Hello, I am",               "Xin chào, tôi là"),
        ("more-about-me",             "More about me",             "Thêm về tôi"),
        ("articles",                  "Articles",                  "Bài viết"),
        ("articles-description",      "Thoughts, stories and ideas.", "Suy nghĩ, câu chuyện và ý tưởng."),
        ("description-search-box",    "Search articles...",        "Tìm kiếm bài viết..."),
        ("popular-articles",          "Popular Articles",          "Bài viết nổi bật"),
        ("tag-cloud",                 "Tag Cloud",                 "Đám mây thẻ"),
        ("our-story",                 "Our Story",                 "Câu chuyện của chúng tôi"),
        ("photo",                     "Photo",                     "Ảnh"),
        ("back",                      "Back",                      "Quay lại"),
        ("photography",               "Photography",               "Nhiếp ảnh"),
        ("photography.album-count",   "albums",                    "album"),
        ("photography.album-sub-count","sub-albums",               "album con"),
        ("photography.album-empty",   "No photos in this album",   "Album này chưa có ảnh"),
        ("comment",                   "Comment",                   "Bình luận"),
        ("reply",                     "Reply",                     "Trả lời"),
        ("leave-a-comment",           "Leave a comment",           "Để lại bình luận"),
        ("post-comment",              "Post Comment",              "Đăng bình luận"),
        ("contact.send-success",      "Your message has been sent successfully. We will get back to you soon!", "Tin nhắn của bạn đã được gửi thành công. Chúng tôi sẽ phản hồi sớm!"),
        ("contact.send-error",        "Failed to send message. Please try again.", "Gửi tin nhắn thất bại. Vui lòng thử lại."),

        // ─── Admin contacts ───────────────────────────────────────────────────
        ("admin.contacts.title",          "Contacts",                     "Liên hệ"),
        ("admin.contacts.subtitle",       "Manage contact form submissions", "Quản lý tin nhắn liên hệ"),
        ("admin.contacts.label.name",     "Name",                         "Họ tên"),
        ("admin.contacts.label.email",    "Email",                        "Email"),
        ("admin.contacts.label.subject",  "Subject",                      "Chủ đề"),
        ("admin.contacts.label.message",  "Message",                      "Tin nhắn"),
        ("admin.contacts.label.status",   "Status",                       "Trạng thái"),
        ("admin.contacts.status.read",    "Read",                         "Đã đọc"),
        ("admin.contacts.status.unread",  "Unread",                       "Chưa đọc"),
        ("admin.contacts.mark-read",      "Mark as read",                 "Đánh dấu đã đọc"),
        ("admin.contacts.detail.title",   "Contact Detail",               "Chi tiết liên hệ"),
        ("admin.contacts.close",          "Close",                        "Đóng"),
        ("admin.sidebar.nav.contacts",    "Contacts",                     "Liên hệ"),

        // ─── Topbar ───────────────────────────────────────────────────────────
        ("admin.topbar.notifications",    "Notifications",               "Thông báo"),
        ("admin.topbar.no-notifications", "No new messages",             "Không có tin nhắn mới"),
        ("admin.topbar.view-all",         "View all contacts",           "Xem tất cả liên hệ"),

        // ─── Dashboard extended ───────────────────────────────────────────────
        ("admin.dashboard.welcome",            "Welcome back",            "Chào mừng trở lại"),
        ("admin.dashboard.session.title",      "Session Information",     "Thông tin phiên làm việc"),
        ("admin.dashboard.session.browser",    "Browser",                 "Trình duyệt"),
        ("admin.dashboard.session.timezone",   "Timezone",                "Múi giờ"),
        ("admin.dashboard.session.lang",       "Language",                "Ngôn ngữ"),
        ("admin.dashboard.recent-messages",    "Recent Messages",         "Tin nhắn mới"),
        ("admin.dashboard.no-unread",          "No unread messages",      "Không có tin nhắn chưa đọc"),
        ("admin.dashboard.view-all",           "View all",                "Xem tất cả"),
        ("admin.dashboard.stat.contacts",      "Contacts",                "Liên hệ"),

        // ─── Page views ───────────────────────────────────────────────────────
        ("admin.page-views.title",          "Page Views",              "Lưu lượng truy cập"),
        ("admin.page-views.subtitle",       "Track visitors and traffic", "Theo dõi lượt xem và truy cập"),
        ("admin.page-views.export",         "Export Excel",            "Xuất Excel"),
        ("admin.page-views.tab.chart",      "Chart",                   "Biểu đồ"),
        ("admin.page-views.tab.table",      "Table",                   "Bảng dữ liệu"),
        ("admin.page-views.period.week",    "Week",                    "Tuần"),
        ("admin.page-views.period.month",   "Month",                   "Tháng"),
        ("admin.page-views.period.year",    "Year",                    "Năm"),
        ("admin.page-views.total-views",    "Total Views",             "Tổng lượt xem"),
        ("admin.page-views.unique-ips",     "Unique Visitors",         "Khách truy cập duy nhất"),
        ("admin.page-views.avg-day",        "Avg / Day",               "Trung bình / ngày"),
        ("admin.page-views.top-pages",      "Top Pages",               "Trang nhiều lượt xem"),
        ("admin.page-views.chart-title",    "Traffic Overview",        "Tổng quan lưu lượng"),
        ("admin.page-views.no-data",        "No data available",       "Không có dữ liệu"),
        ("admin.page-views.label.ip",       "IP Address",              "Địa chỉ IP"),
        ("admin.page-views.label.path",     "Path",                    "Đường dẫn"),
        ("admin.page-views.label.browser",  "Browser",                 "Trình duyệt"),
        ("admin.page-views.label.os",       "OS",                      "Hệ điều hành"),
        ("admin.page-views.label.referer",  "Referer",                 "Trang nguồn"),
        ("admin.page-views.browser-chart",  "Browser Breakdown",       "Phân tích trình duyệt"),
        ("admin.page-views.os-chart",       "OS Breakdown",            "Phân tích hệ điều hành"),
        ("admin.page-views.hourly-chart",   "Peak Hours",              "Khung giờ cao điểm"),
        ("admin.sidebar.nav.page-views",    "Page Views",              "Lưu lượng"),

        // ─── Hero slides ──────────────────────────────────────────────────────
        ("admin.hero-slides.title",         "Hero Slides",             "Slide trang chủ"),
        ("admin.hero-slides.subtitle",      "Manage homepage carousel slides", "Quản lý slide carousel trang chủ"),
        ("admin.hero-slides.create",        "Add Slide",               "Thêm slide"),
        ("admin.hero-slides.modal.create",  "New Slide",               "Thêm slide mới"),
        ("admin.hero-slides.modal.edit",    "Edit Slide",              "Chỉnh sửa slide"),
        ("admin.hero-slides.label.image",   "Background Image",        "Ảnh nền"),
        ("admin.hero-slides.label.title",   "Title",                   "Tiêu đề"),
        ("admin.hero-slides.label.desc",    "Description",             "Mô tả"),
        ("admin.hero-slides.label.order",   "Order",                   "Thứ tự"),
        ("admin.hero-slides.label.active",  "Active",                  "Hiển thị"),
        ("admin.hero-slides.empty",         "No slides yet",           "Chưa có slide nào"),
        ("admin.sidebar.nav.hero-slides",   "Hero Slides",             "Slide trang chủ"),
    ];
}
