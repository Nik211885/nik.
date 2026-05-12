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
        ("admin.label.key",         "Key",                                      "Khóa"),
        ("admin.label.value",       "Value",                                    "Giá trị"),
        ("admin.label.language",    "Language",                                 "Ngôn ngữ"),

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
        ("admin.albums.form.parent",         "Parent Album",        "Album cha"),
        ("admin.albums.form.no-parent",      "— No parent —",       "— Không có album cha —"),

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
    ];
}
