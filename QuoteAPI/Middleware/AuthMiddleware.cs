using QuoteAPI.Services;

namespace QuoteAPI.Middleware
{
    public class AuthMiddleware
    {
        private readonly ITokenService _tokenService;
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next, ITokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains("/api/quotes"))
            {
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    string token = context.Request.Headers.Authorization.ToString();

                    if (string.IsNullOrEmpty(token))
                        throw new Exception("401 Unauthorized");
                    if (_tokenService.IsTokenValid(token))
                        await _next(context);
                    else
                        throw new Exception("401 Unauthorized");
                }
                else
                    throw new Exception("401 Unauthorized");
            }
            else
                await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
