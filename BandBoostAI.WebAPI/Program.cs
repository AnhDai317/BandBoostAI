using BandBoostAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BandBoostAI.Application.Interfaces.Services;
using BandBoostAI.Application.Services;
using BandBoostAI.Application.Interfaces.Repositories;
using BandBoostAI.Infrastructure.Providers;
using BandBoostAI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>(); // Đăng ký Provider
builder.Services.AddScoped<IUserRepository, UserRepository>();    
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IExamAttemptRepository, ExamAttemptRepository>();
builder.Services.AddScoped<ILearningRepository, LearningRepository>();
builder.Services.AddScoped<ILearningService, LearningService>();
builder.Services.AddHttpClient(); // Cho phép ứng dụng tạo các cuộc gọi HTTP ra internet
builder.Services.AddHttpClient<IDictionaryLookupService, DictionaryLookupService>(client =>
{
    client.BaseAddress = new Uri("https://api.dictionaryapi.dev/");
    client.Timeout = TimeSpan.FromSeconds(6);
});
builder.Services.AddHttpClient<IWordDiscoveryService, WordDiscoveryService>(client =>
{
    client.BaseAddress = new Uri("https://api.datamuse.com/");
    client.Timeout = TimeSpan.FromSeconds(6);
});
builder.Services.AddHttpClient<ITranslationService, TranslationService>(client =>
{
    client.BaseAddress = new Uri("https://api.mymemory.translated.net/");
    client.Timeout = TimeSpan.FromSeconds(6);
});
builder.Services.AddScoped<IAiScoringService, AiScoringService>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keep JWT claim names such as "sub" and "email" unchanged.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseAuthentication(); // Bật kiểm tra Hộ chiếu (Token)
app.UseAuthorization();  // Bật kiểm tra Phân quyền (Role)
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

// Tự động Seed dữ liệu đề thi mẫu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context);
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
