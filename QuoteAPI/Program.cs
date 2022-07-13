using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuoteAPI.Helpers;
using QuoteAPI.Middleware;
using QuoteAPI.Services;
using System.Diagnostics;
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
            ValidIssuer = builder.Configuration.Get<JWTAuthSettings>().Issuer,
            ValidAudience = builder.Configuration.Get<JWTAuthSettings>().Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.Get<JWTAuthSettings>().Key!))
        };
    });

builder.Services.AddSingleton<QuoteModelFactory>();
builder.Services.AddSingleton<QuoteService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddMvc()
//        .AddSessionStateTempDataProvider();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30); //We set Time here 
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseSession();
app.UseAuth();
//    .Use(async (context, next) =>
//{
//    var token = context.Session.GetString("Token");

//    if (!string.IsNullOrEmpty(token))
//        context.Request.Headers.Add("Authorization", "Bearer " + token);
    
//    await next();
//});

app.UseAuthorization();
app.MapControllers();

app.Run();
