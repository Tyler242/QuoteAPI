using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuoteAPI;
using QuoteAPI.Helpers;
using QuoteAPI.Middleware;
using QuoteAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<QuoteDBSettings>(
    builder.Configuration.GetSection("QuoteDatabase"));

builder.Services.Configure<QuoteDBCredentials>(
    builder.Configuration.GetSection("DBCredentials"));

builder.Services.Configure<JWTAuthSettings>(
    builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.Get<JWTAuthSettings>().Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"),
            ValidAudience = builder.Configuration.Get<JWTAuthSettings>().Issuer ?? Environment.GetEnvironmentVariable("JWTIssuer"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                (builder.Configuration.Get<JWTAuthSettings>().Key ?? Environment.GetEnvironmentVariable("JWTKey"))!))
        };
    });

builder.Services.AddSingleton<QuoteModelFactory>();
builder.Services.AddSingleton<QuoteService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerAuthorizationHeader>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuth();

app.UseAuthorization();
app.MapControllers();

app.Run();
