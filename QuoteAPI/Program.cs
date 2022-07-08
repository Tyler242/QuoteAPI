using QuoteAPI.Models;
using QuoteAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<QuoteDBSettings>(
    builder.Configuration.GetSection("QuoteDatabase"));

builder.Services.Configure<QuoteDBCredentials>(
    builder.Configuration.GetSection("DBCredentials"));

builder.Services.AddSingleton<QuoteModelFactory>();
builder.Services.AddSingleton<QuoteService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
