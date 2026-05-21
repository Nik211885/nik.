using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>Seeds work experience, skill, and project entries. Idempotent — skips if data already exists.</summary>
public static class CareerSeeder
{
    /// <summary>Seeds work experiences, skills, and projects if the tables are empty.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await SeedWorkExperiencesAsync(db);
        await SeedSkillsAsync(db);
        await SeedProjectsAsync(db);
    }

    private static async Task SeedWorkExperiencesAsync(ApplicationDbContext db)
    {
        if (await db.WorkExperiences.AnyAsync()) return;

        // Base content is Vietnamese
        var entries = new List<(WorkExperience Entity, string EnRole, string EnCompany, string EnDesc)>
        {
            (
                new WorkExperience
                {
                    Company     = "Freelance / Tự do",
                    Role        = "Lập trình viên Full-stack",
                    StartDate   = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                    EndDate     = null,
                    Description = "Xây dựng các ứng dụng web cá nhân và dự án freelance sử dụng Angular và .NET Core. Thiết kế và triển khai giải pháp từ giao diện người dùng đến hệ thống backend.",
                    TechTags    = "Angular,.NET,PostgreSQL,Docker,Cloudflare",
                    Order       = 0,
                    IsPublished = true,
                },
                "Full-stack Developer",
                "Freelance / Self-employed",
                "Building personal web applications and freelance projects using Angular and .NET Core. Designing and deploying end-to-end solutions from UI to backend."
            ),
            (
                new WorkExperience
                {
                    Company     = "Dự án cá nhân",
                    Role        = "Lập trình viên",
                    StartDate   = new DateTimeOffset(2022, 6, 1, 0, 0, 0, TimeSpan.Zero),
                    EndDate     = new DateTimeOffset(2023, 12, 31, 0, 0, 0, TimeSpan.Zero),
                    Description = "Phát triển và duy trì nhiều dự án web cá nhân. Tích lũy kinh nghiệm thực tế với các công nghệ frontend và backend hiện đại.",
                    TechTags    = "Angular,TypeScript,ASP.NET Core,EF Core",
                    Order       = 1,
                    IsPublished = true,
                },
                "Software Developer",
                "Personal Projects",
                "Developed and maintained multiple personal web projects. Built hands-on experience with modern frontend and backend technologies."
            ),
            (
                new WorkExperience
                {
                    Company     = "Trường đại học",
                    Role        = "Sinh viên Khoa học Máy tính",
                    StartDate   = new DateTimeOffset(2020, 9, 1, 0, 0, 0, TimeSpan.Zero),
                    EndDate     = new DateTimeOffset(2024, 6, 30, 0, 0, 0, TimeSpan.Zero),
                    Description = "Học kỹ thuật phần mềm, thuật toán, cấu trúc dữ liệu và mạng máy tính.",
                    TechTags    = "C,C++,Java,Python",
                    Order       = 2,
                    IsPublished = true,
                },
                "Computer Science Student",
                "University",
                "Studied software engineering, algorithms, data structures, and computer networks."
            ),
        };

        var translations = new List<ContentTranslation>();
        foreach (var (entity, enRole, enCompany, enDesc) in entries)
        {
            db.WorkExperiences.Add(entity);
            translations.Add(CT(entity.Id, "role",        enRole));
            translations.Add(CT(entity.Id, "company",     enCompany));
            translations.Add(CT(entity.Id, "description", enDesc));
        }

        await db.SaveChangesAsync();

        db.ContentTranslations.AddRange(translations);
        await db.SaveChangesAsync();
    }

    private static async Task SeedSkillsAsync(ApplicationDbContext db)
    {
        if (await db.Skills.AnyAsync()) return;

        var entries = new List<Skill>
        {
            new() { Name = "Angular",      Category = "Frontend", Order = 0, IsPublished = true },
            new() { Name = "TypeScript",   Category = "Frontend", Order = 1, IsPublished = true },
            new() { Name = "RxJS",         Category = "Frontend", Order = 2, IsPublished = true },
            new() { Name = "Bootstrap",    Category = "Frontend", Order = 3, IsPublished = true },
            new() { Name = "HTML / CSS",   Category = "Frontend", Order = 4, IsPublished = true },

            new() { Name = ".NET",         Category = "Backend", Order = 0, IsPublished = true },
            new() { Name = "C#",           Category = "Backend", Order = 1, IsPublished = true },
            new() { Name = "ASP.NET Core", Category = "Backend", Order = 2, IsPublished = true },
            new() { Name = "EF Core",      Category = "Backend", Order = 3, IsPublished = true },
            new() { Name = "PostgreSQL",   Category = "Backend", Order = 4, IsPublished = true },

            new() { Name = "Docker",       Category = "Tools", Order = 0, IsPublished = true },
            new() { Name = "Git",          Category = "Tools", Order = 1, IsPublished = true },
            new() { Name = "Linux",        Category = "Tools", Order = 2, IsPublished = true },
            new() { Name = "Cloudinary",   Category = "Tools", Order = 3, IsPublished = true },
            new() { Name = "Cloudflare",   Category = "Tools", Order = 4, IsPublished = true },
        };

        db.Skills.AddRange(entries);
        await db.SaveChangesAsync();
    }

    private static async Task SeedProjectsAsync(ApplicationDbContext db)
    {
        if (await db.Projects.AnyAsync()) return;

        var entries = new List<(Project Entity, string EnName, string EnDesc)>
        {
            (
                new Project
                {
                    Name        = "Nik.app — Portfolio cá nhân",
                    Description = "Trang web portfolio cá nhân xây dựng bằng Angular 21 và ASP.NET Core 10. Gồm blog, thư viện ảnh, trang sự nghiệp và bảng quản trị đầy đủ tính năng.",
                    TechTags    = "Angular,.NET,PostgreSQL,Docker,Cloudflare",
                    DemoUrl     = null,
                    RepoUrl     = null,
                    Order       = 0,
                    IsPublished = true,
                },
                "Nik.app — Personal Portfolio",
                "Personal portfolio website built with Angular 21 and ASP.NET Core 10. Includes a blog, photo gallery, careers section, and a full-featured admin panel."
            ),
            (
                new Project
                {
                    Name        = "Hệ thống quản lý nội dung",
                    Description = "Ứng dụng web quản lý bài viết, album ảnh, danh mục và thẻ. Hỗ trợ đa ngôn ngữ thông qua hệ thống ContentTranslation.",
                    TechTags    = "Angular,TypeScript,ASP.NET Core,EF Core,PostgreSQL",
                    DemoUrl     = null,
                    RepoUrl     = null,
                    Order       = 1,
                    IsPublished = true,
                },
                "Content Management System",
                "Web application for managing articles, photo albums, categories, and tags. Supports multilingual content through a ContentTranslation system."
            ),
            (
                new Project
                {
                    Name        = "Ứng dụng theo dõi chi tiêu",
                    Description = "Ứng dụng quản lý tài chính cá nhân cho phép theo dõi thu chi, phân loại giao dịch và xem thống kê theo tháng.",
                    TechTags    = "Angular,Chart.js,ASP.NET Core,PostgreSQL",
                    DemoUrl     = null,
                    RepoUrl     = null,
                    Order       = 2,
                    IsPublished = true,
                },
                "Expense Tracker App",
                "Personal finance management app for tracking income and expenses, categorising transactions, and viewing monthly statistics."
            ),
        };

        var translations = new List<ContentTranslation>();
        foreach (var (entity, enName, enDesc) in entries)
        {
            db.Projects.Add(entity);
            translations.Add(CTProject(entity.Id, "name",        enName));
            translations.Add(CTProject(entity.Id, "description", enDesc));
        }

        await db.SaveChangesAsync();

        db.ContentTranslations.AddRange(translations);
        await db.SaveChangesAsync();
    }

    private static ContentTranslation CT(string entityId, string field, string value) => new()
    {
        EntityType = EntityType.WorkExperience,
        EntityId   = entityId,
        Field      = field,
        LangCode   = "en",
        Value      = value,
    };

    private static ContentTranslation CTProject(string entityId, string field, string value) => new()
    {
        EntityType = EntityType.Project,
        EntityId   = entityId,
        Field      = field,
        LangCode   = "en",
        Value      = value,
    };
}
