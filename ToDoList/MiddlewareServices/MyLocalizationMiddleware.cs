using System.Globalization;
using ToDoList.Data.Enums;
using ToDoList.Services.Interfaces;

namespace ToDoList.MiddlewareServices
{
    public class MyLocalizationMiddleware
    {

        private readonly RequestDelegate _next;

        public MyLocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var authService = context.RequestServices.GetRequiredService<IAuthService>();

            var culture = authService.IsAuthenticated()
                ? authService.GetLanguage() switch
                {
                    Language.Russian => new CultureInfo("ru"),
                    Language.English => new CultureInfo("en"),
                    _ => new CultureInfo("ru")
                }
                : GetGuestCulture(context);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }

        private static CultureInfo GetGuestCulture(HttpContext context)
        {
            var acceptLanguage = context.Request.Headers.AcceptLanguage.ToString();
            if (acceptLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                return new CultureInfo("en");
            }

            return new CultureInfo("ru");
        }
    }
}
