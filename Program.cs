using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RankingServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Redis ���� ����
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddScoped<RankingService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
