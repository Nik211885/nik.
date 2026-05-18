using System.Text;
using backend.Data;
using backend.Entities;
using backend.Pipes.Middlewares;
using backend.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// CORS_ORIGINS env var (semicolon-separated) takes priority over appsettings
var envOrigins = (Environment.GetEnvironmentVariable("CORS_ORIGINS") ?? "")
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var allowedOrigins = envOrigins.Length > 0
    ? envOrigins
    : builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddServicesDefault();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ExeceptionHandlingMiddleware>();
builder.Services.AddScoped<TranslatedHandlingMiddleware>();

builder.Services.AddDbContext<ApplicationDbContext>(sp =>
{
    sp.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
    ArgumentNullException.ThrowIfNull(dbContext);
    await dbContext.Database.MigrateAsync();
    await LanguageSeeder.SeedAsync(dbContext);
    await ArticleSeeder.SeedAsync(dbContext);
    await AlbumSeeder.SeedAsync(dbContext);
    await HeroSlideSeeder.SeedAsync(dbContext);
    await SysConfigSeeder.SeedAsync(dbContext);
    await ContentTranslationSeeder.SeedAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "nik.app");
        c.RoutePrefix = "";
    });
}

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
    app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ExeceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
