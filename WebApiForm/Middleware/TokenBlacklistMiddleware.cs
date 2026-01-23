namespace WebApiForm.Middleware
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

            if (TokenBlacklist.IsBlacklisted(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Token is blacklisted");
                return;
            }

            await _next(context);
        }
    }

    public static class TokenBlacklistExtensions
    {
        public static IApplicationBuilder UseTokenBlacklist(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenBlacklistMiddleware>();
        }
    }
}
