using System.Text;
using DemoApi.Data;
using DemoApi.Extensions;
using DemoApi.Repositories;
using DemoApi.Repositories.Impl;
using DemoApi.Services;
using DemoApi.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 註冊 MVC 控制器服務
builder.Services.AddControllers();

// 加入 Swagger 產生器
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI 註冊 Service
builder.Services.AddScoped<IMemberService, MemberServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();
builder.Services.AddScoped<IJwtService, JwtServiceImpl>();
builder.Services.AddScoped<IPasswordService, PasswordServiceImpl>();
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistServiceImpl>();

// DI 註冊 Repository
builder.Services.AddScoped<IMemberRepository, MemberRepositoryImpl>();

// 設定 JWT 認證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // token 驗證設定
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,  // 驗證發行者
            ValidateAudience = true,  // 驗證受眾
            ValidateLifetime = true,  // 驗證過期時間
            ValidateIssuerSigningKey = true,  // 驗證簽名
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // 設定發行者
            ValidAudience = builder.Configuration["Jwt:Audience"],  // 設定受眾
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))  // 設定密鑰
        };

        // redis token 黑名單檢查設定
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var accessToken = context.Request.Headers["Authorization"]
                    .ToString().Replace("Bearer ", "");

                var redis = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();

                if (await redis.IsTokenBlacklisted(accessToken))
                {
                    context.Fail("This token has been revoked.");
                }

            }
        };
    });

// DI 註冊 AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// MSSQL 資料庫連線字串
// 指定 Migration 專案名稱(確保執行階段，提供 DbContext 實例給服務使用)
var connectionString = builder.Configuration.GetConnectionString("MSSQL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    sqlOptions.MigrationsAssembly("DemoApi")));

// redis 連線註冊
var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // 啟用 Swagger Middleware（開發環境用）
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order System API V1");
    });
}

// 全域例外管理
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

// 註冊身份驗證和授權(順序要先 Auth 再 Authorize)
app.UseAuthentication();
app.UseAuthorization();

// 註冊 MVC 控制器(對應 attribute routing)
app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
