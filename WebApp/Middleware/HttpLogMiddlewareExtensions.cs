using Microsoft.AspNetCore.Builder;

namespace WebApp
{
    public static class HttpLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpLogMiddleware>();
        }
    }
}
