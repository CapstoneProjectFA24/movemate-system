using MoveMate.API.Middleware;

namespace MoveMate.API.Extensions
{
    public static class ExceptionMiddlewareExtention
    {
        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            // bibi
        }
    }
}