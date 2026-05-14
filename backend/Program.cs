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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
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
    await AlbumSeeder.SeedAsync(dbContext);
    await ArticleSeeder.SeedAsync(dbContext);
    await SysConfigSeeder.SeedAsync(dbContext);
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

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ExeceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
