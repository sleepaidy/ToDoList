using System.Globalization;
using ToDoList.Data.Enums;
using ToDoList.Interfaces;

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
                : new CultureInfo("ru");

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await _next(context);
        }
    }
}
