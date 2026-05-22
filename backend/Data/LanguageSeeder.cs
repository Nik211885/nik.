using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// Seeds default languages, translation keys, and UI translations on startup.
/// Existing translation values are updated to match the seeder — it is the source of truth.
/// </summary>
public static class LanguageSeeder
{
    /// <summary>
    /// Ensures all translation keys and their English/Vietnamese values exist and are up to date.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var enLang = await EnsureLanguageAsync(db, "en", "English");
        var viLang = await EnsureLanguageAsync(db, "vi", "Tiếng Việt");
        await db.SaveChangesAsync();

        var existingCodes = await db.CodeLanguages.AsNoTracking()
            .ToDictionaryAsync(x => x.Code, x => x.Id);

        // Load with tracking so value updates are persisted on SaveChangesAsync
        var existingTranslates = await db.Translates
            .ToDictionaryAsync(x => $"{x.CodeId}:{x.LanguageId}", x => x);

        foreach (var (key, en, vi) in GetEntries())
        {
            if (!existingCodes.TryGetValue(key, out var codeId))
            {
                var entity = new CodeLanguage { Code = key };
                db.CodeLanguages.Add(entity);
                codeId = entity.Id;
                existingCodes[key] = codeId;
            }

            Upsert(db, existingTranslates, codeId, enLang.Id, en);
            Upsert(db, existingTranslates, codeId, viLang.Id, vi);
        }

        await db.SaveChangesAsync();
    }

    private static void Upsert(
        ApplicationDbContext db,
        Dictionary<string, Translate> cache,
        string codeId, string langId, string value)
    {
        var k = $"{codeId}:{langId}";
        if (cache.TryGetValue(k, out var existing))
        {
            existing.Value = value;
        }
        else
        {
            var t = new Translate { CodeId = codeId, LanguageId = langId, Value = value };
            db.Translates.Add(t);
            cache[k] = t;
        }
    }

    private static async Task<Language> EnsureLanguageAsync(ApplicationDbContext db, string code, string name)
    {
        var existing = await db.Languages.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null) return existing;
        var lang = new Language { Code = code, Name = name };
        db.Languages.Add(lang);
        return lang;
    }

    /// <summary>Returns all translation entries as (key, english, vietnamese) tuples.</summary>
    private static (string Key, string En, string Vi)[] GetEntries() =>
    [
        // ─── Shared ───────────────────────────────────────────────────────────
        ("admin.save",              "Save",                                 "Lưu"),
        ("admin.saving",            "Saving...",                            "Đang lưu..."),
        ("admin.cancel",            "Cancel",                               "Hủy"),
        ("admin.error.generic",          "An error occurred, please try again.", "Có lỗi xảy ra, vui lòng thử lại."),
        ("admin.toast.delete-success",   "Deleted successfully.",               "Xóa thành công."),
        ("admin.toast.delete-error",     "Delete failed, please try again.",    "Xóa thất bại, vui lòng thử lại."),
        ("admin.toast.save-success",     "Saved successfully.",                 "Lưu thành công."),
        ("admin.toast.save-error",       "Save failed, please try again.",      "Lưu thất bại, vui lòng thử lại."),
        ("admin.loading",           "Loading...",                           "Đang tải..."),
        ("admin.label.name",        "Name",                                 "Tên"),
        ("admin.label.title",       "Title",                                "Tiêu đề"),
        ("admin.label.description", "Description",                          "Mô tả"),
        ("admin.label.image",       "Image",                                "Hình ảnh"),
        ("admin.label.order",       "Order",                                "Thứ tự"),
        ("admin.label.key",           "Key",              "Khóa"),
        ("admin.label.value",         "Value",            "Giá trị"),
        ("admin.label.language",      "Language",         "Ngôn ngữ"),
        ("admin.label.slug",          "Slug",             "Slug"),
        ("admin.label.views",         "Views",            "Lượt xem"),
        ("admin.label.like",          "Likes",            "Lượt thích"),
        ("admin.label.comment-count", "Comments",         "Bình luận"),
        ("admin.label.created-date",  "Created Date",     "Ngày tạo"),
        ("admin.label.image-count",   "Images",           "Số ảnh"),
        ("admin.label.article-count", "Articles",         "Bài viết"),
        ("admin.label.content",       "Content",          "Nội dung"),
        ("admin.label.article-id",    "Article ID",       "ID bài viết"),
        ("admin.label.author",        "Author",           "Tác giả"),
        ("admin.label.reply-id",      "Reply ID",         "ID trả lời"),
        ("admin.label.lang-code",     "Language Code",    "Mã ngôn ngữ"),
        ("admin.label.username",      "Username",         "Tên đăng nhập"),
        ("admin.label.email",         "Email",            "Email"),
        ("admin.label.phone",         "Phone",            "Điện thoại"),
        ("admin.label.bio",           "Bio",              "Giới thiệu"),

        // ─── Dashboard ────────────────────────────────────────────────────────
        ("admin.dashboard.title",            "Dashboard",          "Bảng điều khiển"),
        ("admin.dashboard.subtitle",         "System overview",    "Tổng quan hệ thống"),
        ("admin.dashboard.welcome",          "Welcome back",       "Chào mừng trở lại"),
        ("admin.dashboard.quick",            "Quick Actions",      "Thao tác nhanh"),
        ("admin.dashboard.new-article",      "New Article",        "Viết bài mới"),
        ("admin.dashboard.new-album",        "New Album",          "Tạo album"),
        ("admin.dashboard.new-category",     "New Category",       "Tạo danh mục"),
        ("admin.dashboard.new-tag",          "New Tag",            "Tạo thẻ tag"),
        ("admin.dashboard.settings",         "Settings",           "Cài đặt"),
        ("admin.dashboard.stat.articles",    "Articles",           "Bài viết"),
        ("admin.dashboard.stat.albums",      "Albums",             "Album"),
        ("admin.dashboard.stat.categories",  "Categories",         "Danh mục"),
        ("admin.dashboard.stat.tags",        "Tags",               "Thẻ tag"),
        ("admin.dashboard.stat.files",       "Files",              "Tệp"),
        ("admin.dashboard.stat.users",       "Users",              "Người dùng"),
        ("admin.dashboard.stat.contacts",    "Contacts",           "Liên hệ"),
        ("admin.dashboard.session.title",    "Session Information","Thông tin phiên làm việc"),
        ("admin.dashboard.session.browser",  "Browser",            "Trình duyệt"),
        ("admin.dashboard.session.timezone", "Timezone",           "Múi giờ"),
        ("admin.dashboard.session.lang",     "Language",           "Ngôn ngữ"),
        ("admin.dashboard.recent-messages",  "Recent Messages",    "Tin nhắn mới"),
        ("admin.dashboard.no-unread",        "No unread messages", "Không có tin nhắn chưa đọc"),
        ("admin.dashboard.view-all",         "View all",           "Xem tất cả"),

        // ─── Articles ─────────────────────────────────────────────────────────
        ("admin.articles.title",           "Articles",          "Bài viết"),
        ("admin.articles.create",          "New Article",       "Viết bài mới"),
        ("admin.articles.modal.create",    "Create Article",    "Tạo bài viết"),
        ("admin.articles.modal.edit",      "Edit Article",      "Chỉnh sửa bài viết"),
        ("admin.articles.form.title",      "Title",             "Tiêu đề"),
        ("admin.articles.form.desc",       "Description",       "Mô tả"),
        ("admin.articles.form.content",    "Content",           "Nội dung"),
        ("admin.articles.form.cover",      "Cover Image",       "Ảnh bìa"),
        ("admin.articles.form.categories", "Categories",        "Danh mục"),
        ("admin.articles.form.tags",       "Tags",              "Thẻ tag"),
        ("admin.articles.save",            "Save Article",      "Lưu bài viết"),

        // ─── Albums ───────────────────────────────────────────────────────────
        ("admin.albums.title",           "Albums",                       "Album"),
        ("admin.albums.subtitle",        "Manage photo albums",          "Quản lý album ảnh"),
        ("admin.albums.create",          "New Album",                    "Tạo album"),
        ("admin.albums.modal.create",    "Create Album",                 "Tạo album"),
        ("admin.albums.modal.edit",      "Edit Album",                   "Chỉnh sửa album"),
        ("admin.albums.form.parent",     "Parent Album",                 "Album cha"),
        ("admin.albums.form.no-parent",  "— No parent —",               "— Không có album cha —"),
        ("admin.albums.view.list",       "List",                         "Danh sách"),
        ("admin.albums.view.tree",       "Tree",                         "Cây thư mục"),
        ("admin.albums.view.explorer",   "Explorer",                     "Trình duyệt"),
        ("admin.albums.empty-children",  "No sub-albums",                "Không có album con"),
        ("admin.albums.files.title",     "Files",                        "Tệp"),
        ("admin.albums.files.empty",     "No files in this album.",      "Album này chưa có tệp nào."),
        ("admin.albums.files.add",       "Add Files",                    "Thêm tệp"),
        ("admin.albums.files.remove",    "Remove",                       "Xóa khỏi album"),
        ("admin.albums.root",            "Root",                         "Gốc"),
        ("admin.albums.expand-all",      "Expand all",                   "Mở rộng tất cả"),
        ("admin.albums.collapse-all",    "Collapse all",                 "Thu gọn tất cả"),
        ("admin.albums.copy-url",        "Copy URL",                     "Sao chép URL"),
        ("admin.albums.set-cover",       "Set as cover",                 "Đặt làm ảnh bìa"),
        ("admin.albums.is-cover",        "Current cover",                "Ảnh bìa hiện tại"),
        ("admin.albums.cover-for",       "Cover for",                    "Ảnh bìa cho"),
        ("admin.albums.count-suffix",    "album",                        "album"),

        // ─── Comments ─────────────────────────────────────────────────────────
        ("admin.comments.title",              "Comments",                                  "Bình luận"),
        ("admin.comments.subtitle",           "Find and delete comments by article",       "Tìm và xóa bình luận theo bài viết"),
        ("admin.comments.search-placeholder", "Enter article ID...",                       "Nhập ID bài viết..."),
        ("admin.comments.search",             "Search",                                    "Tìm kiếm"),

        // ─── Tags ─────────────────────────────────────────────────────────────
        ("admin.tags.title",          "Tags",        "Thẻ tag"),
        ("admin.tags.subtitle",       "Manage tags", "Quản lý thẻ tag"),
        ("admin.tags.create",         "New Tag",     "Tạo thẻ tag"),
        ("admin.tags.modal.create",   "Create Tag",  "Tạo thẻ tag"),
        ("admin.tags.modal.edit",     "Edit Tag",    "Chỉnh sửa thẻ tag"),

        // ─── Categories ───────────────────────────────────────────────────────
        ("admin.categories.title",          "Categories",          "Danh mục"),
        ("admin.categories.subtitle",       "Manage categories",   "Quản lý danh mục"),
        ("admin.categories.create",         "New Category",        "Tạo danh mục"),
        ("admin.categories.modal.create",   "Create Category",     "Tạo danh mục"),
        ("admin.categories.modal.edit",     "Edit Category",       "Chỉnh sửa danh mục"),

        // ─── Users ────────────────────────────────────────────────────────────
        ("admin.users.title",           "Users",                "Người dùng"),
        ("admin.users.subtitle",        "Manage user accounts", "Quản lý tài khoản người dùng"),
        ("admin.users.modal.edit",      "Edit User",            "Chỉnh sửa người dùng"),
        ("admin.users.form.username",   "Username",             "Tên đăng nhập"),
        ("admin.users.form.email",      "Email",                "Email"),
        ("admin.users.form.phone",      "Phone",                "Điện thoại"),
        ("admin.users.form.bio",        "Bio",                  "Giới thiệu"),

        // ─── Languages ────────────────────────────────────────────────────────
        ("admin.languages.title",           "Languages",                     "Ngôn ngữ"),
        ("admin.languages.subtitle",        "Manage interface languages",    "Quản lý ngôn ngữ giao diện"),
        ("admin.languages.create",          "New Language",                  "Thêm ngôn ngữ"),
        ("admin.languages.modal.create",    "Create Language",               "Tạo ngôn ngữ"),
        ("admin.languages.modal.edit",      "Edit Language",                 "Chỉnh sửa ngôn ngữ"),
        ("admin.languages.form.code",       "Language Code",                 "Mã ngôn ngữ"),

        // ─── Translations ─────────────────────────────────────────────────────
        ("admin.translations.all-langs",        "All languages",           "Tất cả ngôn ngữ"),
        ("admin.translations.title",            "Translations",            "Bản dịch"),
        ("admin.translations.subtitle",         "Manage translation keys", "Quản lý khóa dịch"),
        ("admin.translations.add-key",          "Add Key",                 "Thêm khóa"),
        ("admin.translations.create",           "New Translation",         "Thêm bản dịch"),
        ("admin.translations.modal.create",     "Create Translation",      "Tạo bản dịch"),
        ("admin.translations.modal.edit",       "Edit Translation",        "Chỉnh sửa bản dịch"),
        ("admin.translations.modal.key-title",  "Create Translation Key",  "Tạo khóa dịch"),
        ("admin.translations.form.value",       "Value",                   "Giá trị"),
        ("admin.translations.form.choose-key",  "Choose key",              "Chọn khóa"),
        ("admin.translations.form.choose-lang", "Choose language",         "Chọn ngôn ngữ"),
        ("admin.translations.save-key",         "Save Key",                "Lưu khóa"),

        // ─── Sys Config ───────────────────────────────────────────────────────
        ("admin.sys-config.title",            "System Config",                    "Cấu hình hệ thống"),
        ("admin.sys-config.subtitle",         "Manage system configuration",      "Quản lý cấu hình hệ thống"),
        ("admin.sys-config.create",           "New Config",                       "Thêm cấu hình"),
        ("admin.sys-config.modal.create",     "Create Config",                    "Tạo cấu hình"),
        ("admin.sys-config.modal.edit",       "Edit Config",                      "Chỉnh sửa cấu hình"),
        ("admin.sys-config.form.value-label", "Value (JSON or text)",             "Giá trị (JSON hoặc văn bản)"),
        ("admin.sys-config.form.hint",        "JSON objects are parsed automatically.", "Đối tượng JSON sẽ được tự động phân tích theo ngôn ngữ."),
        ("admin.sys-config.editor.back",      "Back to configs",        "Quay lại"),
        ("admin.sys-config.form.vi",          "Vietnamese",             "Tiếng Việt"),
        ("admin.sys-config.form.en",          "English",                "Tiếng Anh"),
        ("admin.sys-config.info.name",        "Full name",              "Họ tên"),
        ("admin.sys-config.info.email",       "Email",                  "Email"),
        ("admin.sys-config.info.phone",       "Phone",                  "Số điện thoại"),
        ("admin.sys-config.info.address",     "Address",                "Địa chỉ"),
        ("admin.sys-config.info.website",     "Website",                "Website"),
        ("admin.sys-config.info.avatar",      "Avatar URL",             "URL ảnh đại diện"),
        ("admin.sys-config.info.bio",         "Short bio",              "Giới thiệu ngắn"),
        ("admin.sys-config.info.intro",       "Introduction",           "Lời giới thiệu"),
        ("admin.sys-config.social.url",       "Profile URL",            "URL trang cá nhân"),
        ("admin.sys-config.nav.key",          "Translation key",        "Khóa dịch"),
        ("admin.sys-config.nav.route",        "Route",                  "Đường dẫn"),
        ("admin.sys-config.add-item",         "Add item",               "Thêm dòng"),

        // ─── Files ────────────────────────────────────────────────────────────
        ("admin.files.title",           "Files",                          "Tệp"),
        ("admin.files.create",          "Upload",                         "Tải lên"),
        ("admin.files.modal.create",    "Upload File",                    "Tải tệp lên"),
        ("admin.files.modal.edit",      "Edit File",                      "Chỉnh sửa tệp"),
        ("admin.files.form.media",      "Media",                          "Tệp phương tiện"),
        ("admin.files.empty",           "No files uploaded yet.",         "Chưa có tệp nào được tải lên."),
        ("admin.files.drop",            "Drop files here to upload",      "Thả tệp vào đây để tải lên"),
        ("admin.files.batch",           "Uploading",                      "Đang tải lên"),
        ("admin.files.upload-required", "Please upload a file first.",    "Vui lòng tải tệp lên trước."),
        ("admin.files.save-failed",     "Save failed.",                   "Lưu thất bại."),

        // ─── Contacts ─────────────────────────────────────────────────────────
        ("admin.contacts.title",          "Contacts",                        "Liên hệ"),
        ("admin.contacts.subtitle",       "Manage contact form submissions", "Quản lý tin nhắn liên hệ"),
        ("admin.contacts.label.name",     "Name",                            "Họ tên"),
        ("admin.contacts.label.email",    "Email",                           "Email"),
        ("admin.contacts.label.subject",  "Subject",                         "Chủ đề"),
        ("admin.contacts.label.message",  "Message",                         "Tin nhắn"),
        ("admin.contacts.label.status",   "Status",                          "Trạng thái"),
        ("admin.contacts.status.read",    "Read",                            "Đã đọc"),
        ("admin.contacts.status.unread",  "Unread",                          "Chưa đọc"),
        ("admin.contacts.mark-read",      "Mark as read",                    "Đánh dấu đã đọc"),
        ("admin.contacts.detail.title",   "Contact Detail",                  "Chi tiết liên hệ"),
        ("admin.contacts.close",          "Close",                           "Đóng"),

        // ─── Hero Slides ──────────────────────────────────────────────────────
        ("admin.hero-slides.title",          "Hero Slides",                         "Slide trang chủ"),
        ("admin.hero-slides.subtitle",       "Manage homepage carousel slides",     "Quản lý slide carousel trang chủ"),
        ("admin.hero-slides.create",         "Add Slide",                           "Thêm slide"),
        ("admin.hero-slides.modal.create",   "New Slide",                           "Thêm slide mới"),
        ("admin.hero-slides.modal.edit",     "Edit Slide",                          "Chỉnh sửa slide"),
        ("admin.hero-slides.label.image",    "Background Image",                    "Ảnh nền"),
        ("admin.hero-slides.label.title",    "Title",                               "Tiêu đề"),
        ("admin.hero-slides.label.desc",     "Description",                         "Mô tả"),
        ("admin.hero-slides.label.order",    "Order",                               "Thứ tự"),
        ("admin.hero-slides.label.active",   "Active",                              "Hiển thị"),
        ("admin.hero-slides.empty",          "No slides yet",                       "Chưa có slide nào"),

        // ─── Content Translations ─────────────────────────────────────────────
        ("admin.content-trans.title",         "Content Translations",                           "Dịch nội dung"),
        ("admin.content-trans.subtitle",      "Translate articles, categories, tags and albums", "Dịch bài viết, danh mục, thẻ tag và album sang ngôn ngữ khác"),
        ("admin.content-trans.entity-type",   "Entity Type",                                    "Loại nội dung"),
        ("admin.content-trans.lang",          "Language",                                       "Ngôn ngữ"),
        ("admin.content-trans.status",        "Status",                                         "Trạng thái"),
        ("admin.content-trans.status.all",    "All",                                            "Tất cả"),
        ("admin.content-trans.status.done",   "Translated",                                     "Đã dịch"),
        ("admin.content-trans.status.todo",   "Not translated",                                 "Chưa dịch"),
        ("admin.content-trans.translate-btn", "Translate",                                      "Dịch"),
        ("admin.content-trans.modal.title",   "Translation Editor",                             "Chỉnh sửa bản dịch"),
        ("admin.content-trans.save",          "Save Translation",                               "Lưu bản dịch"),
        ("admin.content-trans.saving",        "Saving...",                                      "Đang lưu..."),
        ("admin.content-trans.original",      "Original (VI)",                                  "Nội dung gốc (VI)"),
        ("admin.content-trans.translation",   "Translation",                                    "Bản dịch"),
        ("admin.content-trans.back",          "Back to list",                                   "Quay lại danh sách"),

        // ─── Topbar ───────────────────────────────────────────────────────────
        ("admin.topbar.notifications",    "Notifications",         "Thông báo"),
        ("admin.topbar.no-notifications", "No new messages",       "Không có tin nhắn mới"),
        ("admin.topbar.view-all",         "View all contacts",     "Xem tất cả liên hệ"),

        // ─── Page Views ───────────────────────────────────────────────────────
        ("admin.page-views.title",         "Page Views",            "Lưu lượng truy cập"),
        ("admin.page-views.subtitle",      "Track visitors and traffic", "Theo dõi lượt xem và truy cập"),
        ("admin.page-views.export",        "Export Excel",          "Xuất Excel"),
        ("admin.page-views.tab.chart",     "Chart",                 "Biểu đồ"),
        ("admin.page-views.tab.table",     "Table",                 "Bảng dữ liệu"),
        ("admin.page-views.period.week",   "Week",                  "Tuần"),
        ("admin.page-views.period.month",  "Month",                 "Tháng"),
        ("admin.page-views.period.year",   "Year",                  "Năm"),
        ("admin.page-views.total-views",   "Total Views",           "Tổng lượt xem"),
        ("admin.page-views.unique-ips",    "Unique Visitors",       "Khách truy cập duy nhất"),
        ("admin.page-views.avg-day",       "Avg / Day",             "Trung bình / ngày"),
        ("admin.page-views.top-pages",     "Top Pages",             "Trang xem nhiều nhất"),
        ("admin.page-views.chart-title",   "Traffic Overview",      "Tổng quan lưu lượng"),
        ("admin.page-views.no-data",       "No data available",     "Không có dữ liệu"),
        ("admin.page-views.label.ip",      "IP Address",            "Địa chỉ IP"),
        ("admin.page-views.label.path",    "Path",                  "Đường dẫn"),
        ("admin.page-views.label.browser", "Browser",               "Trình duyệt"),
        ("admin.page-views.label.os",      "OS",                    "Hệ điều hành"),
        ("admin.page-views.label.referer", "Referer",               "Trang nguồn"),
        ("admin.page-views.browser-chart", "Browser Breakdown",     "Phân tích trình duyệt"),
        ("admin.page-views.os-chart",      "OS Breakdown",          "Phân tích hệ điều hành"),
        ("admin.page-views.hourly-chart",  "Peak Hours",            "Khung giờ cao điểm"),

        // ─── Login ────────────────────────────────────────────────────────────
        ("admin.login.title",                "Admin Login",                                "Đăng nhập quản trị"),
        ("admin.login.subtitle",             "Sign in to manage your site",                "Đăng nhập để quản lý trang web"),
        ("admin.login.email-label",          "Email or Username",                          "Email hoặc tên đăng nhập"),
        ("admin.login.email-placeholder",    "Enter email or username",                    "Nhập email hoặc tên đăng nhập"),
        ("admin.login.password-label",       "Password",                                   "Mật khẩu"),
        ("admin.login.password-placeholder", "Enter password",                             "Nhập mật khẩu"),
        ("admin.login.submit",               "Sign In",                                    "Đăng nhập"),
        ("admin.login.submitting",           "Signing in...",                              "Đang đăng nhập..."),
        ("admin.login.back",                 "Back to site",                               "Quay lại trang web"),
        ("admin.login.error",                "Invalid email/username or password.",        "Email/Tên đăng nhập hoặc mật khẩu không đúng."),

        // ─── Cloudinary Upload ────────────────────────────────────────────────
        ("admin.upload.change",     "Change",                     "Thay đổi"),
        ("admin.upload.drag-title", "Drop image or video here",   "Thả ảnh hoặc video vào đây"),
        ("admin.upload.drag-sub",   "or <u>click to browse</u>", "hoặc <u>nhấp để chọn</u>"),
        ("admin.upload.drag-hint",  "JPG, PNG, WEBP, MP4",       "JPG, PNG, WEBP, MP4"),
        ("admin.upload.uploading",  "Uploading",                  "Đang tải lên"),
        ("admin.upload.manual-url", "Or enter URL manually",      "Hoặc nhập URL thủ công"),
        ("admin.upload.error",      "Upload failed.",             "Tải lên thất bại."),

        // ─── Table ────────────────────────────────────────────────────────────
        ("admin.table.actions",       "Actions",       "Thao tác"),
        ("admin.table.loading",       "Loading...",    "Đang tải..."),
        ("admin.table.empty-data",    "No data found.","Không có dữ liệu."),
        ("admin.table.action.edit",   "Edit",          "Chỉnh sửa"),
        ("admin.table.action.delete", "Delete",        "Xóa"),
        ("admin.table.per-page",      "Per page",      "Mỗi trang"),
        ("admin.table.selected",      "selected",      "đã chọn"),
        ("admin.table.deselect",      "Deselect all",  "Bỏ chọn tất cả"),

        // ─── Dialog ───────────────────────────────────────────────────────────
        ("admin.diaglog.delete.confirm-title",    "Confirm Delete",               "Xác nhận xóa"),
        ("admin.diaglog.delete.confirm-label",    "Are you sure?",                "Bạn có chắc chắn không?"),
        ("admin.diaglog.delete.action.delete",    "Delete",                       "Xóa"),
        ("admin.diaglog.delete.action.cancel",    "Cancel",                       "Hủy"),
        ("admin.diaglog.delete.message",          "This action cannot be undone.","Hành động này không thể hoàn tác."),

        // ─── Sidebar ──────────────────────────────────────────────────────────
        ("admin.sidebar.home",                  "Home",                 "Trang chủ"),
        ("admin.sidebar.logout",                "Logout",               "Đăng xuất"),
        ("admin.sidebar.nav.dashboard",         "Dashboard",            "Bảng điều khiển"),
        ("admin.sidebar.nav.articles",          "Articles",             "Bài viết"),
        ("admin.sidebar.nav.albums",            "Albums",               "Album"),
        ("admin.sidebar.nav.categories",        "Categories",           "Danh mục"),
        ("admin.sidebar.nav.tags",              "Tags",                 "Thẻ tag"),
        ("admin.sidebar.nav.comments",          "Comments",             "Bình luận"),
        ("admin.sidebar.nav.files",             "Files",                "Tệp"),
        ("admin.sidebar.nav.users",             "Users",                "Người dùng"),
        ("admin.sidebar.nav.languages",         "Languages",            "Ngôn ngữ"),
        ("admin.sidebar.nav.translations",      "Translations",         "Bản dịch"),
        ("admin.sidebar.nav.settings",          "Settings",             "Cài đặt"),
        ("admin.sidebar.nav.contacts",          "Contacts",             "Liên hệ"),
        ("admin.sidebar.nav.page-views",        "Page Views",           "Lưu lượng"),
        ("admin.sidebar.nav.hero-slides",       "Hero Slides",          "Slide trang chủ"),
        ("admin.sidebar.nav.content-trans",     "Content Translations", "Dịch nội dung"),
        ("admin.sidebar.group.overview",        "Overview",             "Tổng quan"),
        ("admin.sidebar.group.content",         "Content",              "Nội dung"),
        ("admin.sidebar.group.community",       "Community",            "Cộng đồng"),
        ("admin.sidebar.group.i18n",            "Localization",         "Đa ngôn ngữ"),
        ("admin.sidebar.group.system",          "System",               "Hệ thống"),

        // ─── Backend error keys ───────────────────────────────────────────────
        ("exception.unauthorized", "Session expired. Please log in again.", "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại."),
        ("file.not.exists",  "File does not exist.",           "Tệp không tồn tại."),
        ("file.type.apply",  "File type is not allowed.",      "Loại tệp không được phép."),
        ("file.max.size",    "File exceeds the maximum size.", "Tệp vượt quá kích thước cho phép."),
        ("loading.process",  "Processing...",                  "Đang xử lý..."),

        // ─── Public navigation ────────────────────────────────────────────────
        ("nav.home",        "Home",        "Trang chủ"),
        ("nav.photography", "Photography", "Nhiếp ảnh"),
        ("nav.travel",      "Travel",      "Du lịch"),
        ("nav.fashion",     "Fashion",     "Thời trang"),
        ("nav.about",       "About",       "Giới thiệu"),
        ("nav.contact",     "Contact",     "Liên hệ"),
        ("nav.wall",        "Wall",        "Nhắn gửi"),
        ("nav.sponsor",     "Sponsor",     "Ủng hộ"),

        // ─── Sponsor page ─────────────────────────────────────────────────────
        ("sponsor.title",          "Support My Work",                                                           "Ủng hộ tôi"),
        ("sponsor.subtitle",       "If you enjoy my content, consider buying me a coffee!",                     "Nếu bạn thích nội dung của tôi, hãy mời tôi một ly cà phê!"),
        // ─── Wall page ───────────────────────────────────────────────────────────
        ("wall.title",     "Wall",                                    "Nhắn Gửi"),
        ("wall.subtitle",  "Leave a short note — I read every one.", "Để lại một vài dòng — tôi đọc hết đấy."),
        ("wall.name",      "Your name",                               "Tên của bạn"),
        ("wall.message",   "Say something...",                        "Nói gì đó..."),
        ("wall.submit",    "Post",                                     "Gửi"),
        ("wall.submitting","Posting...",                               "Đang gửi..."),
        ("wall.pending",   "Your message is under review. It'll appear soon!", "Tin nhắn của bạn đang được xem xét. Sẽ hiện sớm thôi!"),
        ("wall.approved",  "Message posted!",                         "Đã đăng tin nhắn!"),
        ("wall.empty",     "Be the first to leave a message!",        "Hãy là người đầu tiên để lại tin nhắn!"),
        ("wall.char-left", "characters left",                         "ký tự còn lại"),
        ("wall.source",    "Source / Author (optional)",              "Nguồn / Tác giả (không bắt buộc)"),
        ("wall.react",     "Like",                                    "Thích"),

        // ─── Admin wall messages ──────────────────────────────────────────────
        ("admin.wall-messages.title",             "Wall Messages",          "Tin nhắn tường"),
        ("admin.wall-messages.subtitle",          "Manage visitor messages","Quản lý tin nhắn khách"),
        ("admin.wall-messages.approve",           "Approve",                "Duyệt"),
        ("admin.wall-messages.reject",            "Reject",                 "Từ chối"),
        ("admin.wall-messages.status.all",        "All",                    "Tất cả"),
        ("admin.wall-messages.status.pending",    "Pending",                "Chờ duyệt"),
        ("admin.wall-messages.status.approved",   "Approved",               "Đã duyệt"),
        ("admin.wall-messages.status.rejected",   "Rejected",               "Từ chối"),
        ("admin.wall-messages.selected",          "selected",               "đã chọn"),
        ("admin.wall-messages.select-all",        "Select all",             "Chọn tất cả"),
        ("admin.sidebar.nav.wall-messages",       "Wall Messages",          "Tin nhắn tường"),
        ("admin.sidebar.nav.careers",             "Careers",                "Sự nghiệp"),

        ("admin.careers.title",                   "Careers",                "Sự nghiệp"),
        ("admin.careers.subtitle",                "Manage work experience and skills", "Quản lý kinh nghiệm và kỹ năng"),
        ("admin.careers.tab.experience",          "Experience",             "Kinh nghiệm"),
        ("admin.careers.tab.skills",              "Skills",                 "Kỹ năng"),
        ("admin.careers.add-experience",          "Add Experience",         "Thêm kinh nghiệm"),
        ("admin.careers.edit-experience",         "Edit Experience",        "Sửa kinh nghiệm"),
        ("admin.careers.add-skill",               "Add Skill",              "Thêm kỹ năng"),
        ("admin.careers.edit-skill",              "Edit Skill",             "Sửa kỹ năng"),
        ("admin.careers.label.company",           "Company",                "Công ty"),
        ("admin.careers.label.role",              "Role",                   "Vị trí"),
        ("admin.careers.label.start-date",        "Start Date",             "Ngày bắt đầu"),
        ("admin.careers.label.end-date",          "End Date (leave blank if current)", "Ngày kết thúc (để trống nếu đang làm)"),
        ("admin.careers.label.tags",              "Tech Tags",              "Công nghệ"),
        ("admin.careers.label.category",          "Category",               "Danh mục"),
        ("admin.careers.tags-hint",               "Angular, .NET, Docker…", "Angular, .NET, Docker…"),
        ("admin.careers.category-hint",           "Frontend, Backend, Tools…", "Frontend, Backend, Tools…"),
        ("admin.careers.present",                 "Present",                "Hiện tại"),
        ("admin.careers.current",                 "Current",                "Đang làm"),
        ("admin.careers.hidden",                  "Hidden",                 "Ẩn"),
        ("admin.careers.published",               "Published",              "Hiển thị"),

        ("about.cv.experience",                   "Experience",             "Kinh nghiệm"),
        ("about.cv.skills",                       "Skills",                 "Kỹ năng"),
        ("about.cv.present",                      "Present",                "Hiện tại"),
        ("about.cv.current",                      "Current",                "Đang làm"),
        ("about.cv.projects",                     "Projects",               "Dự án"),
        ("about.cv.demo",                         "Demo",                   "Demo"),
        ("about.cv.repo",                         "Repository",             "Source code"),

        ("admin.careers.tab.projects",            "Projects",               "Dự án"),
        ("admin.careers.add-project",             "Add Project",            "Thêm dự án"),
        ("admin.careers.edit-project",            "Edit Project",           "Sửa dự án"),
        ("admin.careers.label.demo-url",          "Demo URL",               "URL Demo"),
        ("admin.careers.label.repo-url",          "Repository URL",         "URL Repository"),

        ("sponsor.page-title",     "Buy me a coffee",                                                           "Mua cho tôi một ly cà phê"),
        ("sponsor.page-desc",      "If my content has ever made you smile, learn something new, or simply kill some time — consider buying me a coffee. It means more than you know.",  "Nếu nội dung của tôi từng khiến bạn mỉm cười, học được điều gì mới, hay chỉ đơn giản là giết thời gian — hãy mua cho tôi một ly cà phê nhé. Điều đó có ý nghĩa hơn bạn nghĩ đấy."),
        ("sponsor.bank",           "Bank",                                                                      "Ngân hàng"),
        ("sponsor.account-name",   "Account Name",                                                              "Tên tài khoản"),
        ("sponsor.account-number", "Account Number",                                                            "Số tài khoản"),
        ("sponsor.copy",           "Copy",                                                                      "Sao chép"),
        ("sponsor.copied",         "Copied!",                                                                   "Đã sao chép!"),
        ("sponsor.scan",           "Scan QR code to transfer",                                                  "Quét mã QR để chuyển khoản"),
        ("sponsor.thank-you",      "Thank you for your support!",                                               "Cảm ơn bạn đã ủng hộ!"),
        ("sponsor.note",           "Every contribution, big or small, means the world to me.",                  "Mọi đóng góp, dù lớn hay nhỏ, đều có ý nghĩa với tôi."),
        ("sponsor.personal-message", "Your support motivates me to keep creating and sharing. Thank you for being here!", "Sự ủng hộ của bạn là động lực để tôi tiếp tục sáng tạo và chia sẻ. Cảm ơn bạn đã ở đây!"),
        ("sponsor.open-app",       "Open payment app",                                                          "Mở app thanh toán"),
        ("sponsor.or-scan",        "or scan QR code above",                                                     "hoặc quét mã QR phía trên"),

        // ─── Public site strings ──────────────────────────────────────────────
        ("category",                 "Category",                          "Danh mục"),
        ("archives",                 "Archives",                          "Lưu trữ"),
        ("have-questions?",          "Have a question?",                  "Có câu hỏi?"),
        ("write-by",                 "Written by",                        "Viết bởi"),
        ("continuing-reading",       "Continue reading",                  "Đọc tiếp"),
        ("news-letter.title",        "Newsletter",                        "Bản tin"),
        ("news-letter.content",      "Subscribe to get the latest posts delivered to your inbox.", "Đăng ký để nhận các bài viết mới nhất trong hộp thư của bạn."),
        ("email-address",            "Email address",                     "Địa chỉ email"),
        ("subscribe",                "Subscribe",                         "Đăng ký"),
        ("contact-information",      "Contact Information",               "Thông tin liên hệ"),
        ("address",                  "Address",                           "Địa chỉ"),
        ("phone",                    "Phone",                             "Điện thoại"),
        ("email",                    "Email",                             "Email"),
        ("website",                  "Website",                           "Trang web"),
        ("your-name",                "Your Name",                         "Họ và tên"),
        ("your-email",               "Your Email",                        "Email của bạn"),
        ("subject",                  "Subject",                           "Chủ đề"),
        ("message",                  "Message",                           "Tin nhắn"),
        ("send-message",             "Send Message",                      "Gửi tin nhắn"),
        ("hello-i-am",               "Hello, I am",                       "Xin chào, tôi là"),
        ("more-about-me",            "More about me",                     "Thêm về tôi"),
        ("articles",                 "Articles",                          "Bài viết"),
        ("articles-description",     "Thoughts, stories and ideas.",      "Suy nghĩ, câu chuyện và ý tưởng."),
        ("description-search-box",   "Search articles...",                "Tìm kiếm bài viết..."),
        ("popular-articles",         "Popular Articles",                  "Bài viết nổi bật"),
        ("tag-cloud",                "Tag Cloud",                         "Đám mây thẻ"),
        ("our-story",                "Our Story",                         "Câu chuyện của chúng tôi"),
        ("photo",                    "Photo",                             "Ảnh"),
        ("back",                     "Back",                              "Quay lại"),
        ("photography",              "Photography",                       "Nhiếp ảnh"),
        ("photography.album-count",  "albums",                            "album"),
        ("photography.album-sub-count", "sub-albums",                     "album con"),
        ("photography.album-empty",  "No photos in this album",           "Album này chưa có ảnh nào"),
        ("comment",                  "Comment",                           "Bình luận"),
        ("reply",                    "Reply",                             "Trả lời"),
        ("leave-a-comment",          "Leave a comment",                   "Để lại bình luận"),
        ("post-comment",             "Post Comment",                      "Đăng bình luận"),
        ("contact.send-success",     "Your message has been sent successfully. We will get back to you soon!", "Tin nhắn của bạn đã được gửi thành công. Chúng tôi sẽ liên hệ lại sớm!"),
        ("contact.send-error",       "Failed to send message. Please try again.", "Gửi tin nhắn thất bại. Vui lòng thử lại."),
        ("auto.translated",          "Translated",                                "Đã dịch"),
    ];
}
