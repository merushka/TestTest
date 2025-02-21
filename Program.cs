using WebApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

//  подключения
var config = builder.Configuration;
var connectionString = config.GetConnectionString("OracleDb");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("? Строка подключения к Oracle не найдена! Проверьте appsettings.json");
}

builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

    try
    {
        Console.WriteLine("?? Seeding database...");
        DatabaseSeeder.SeedAll(db);
        Console.WriteLine("? Database seeding completed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Ошибка сидирования данных: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

Console.WriteLine("?? API запущен!");
app.Run();
