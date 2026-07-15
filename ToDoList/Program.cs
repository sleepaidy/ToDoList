using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Data.Repository;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Hubs;
using ToDoList.Hubs.Interfaces;
using ToDoList.Interfaces;
using ToDoList.MiddlewareServices;
using ToDoList.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultDbConnection");
builder.Services.AddDbContext<WebContext>(op => op.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IToDoListService, ToDoListServices>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddAuthentication(AuthService.AUTH_KEY)
    .AddCookie(AuthService.AUTH_KEY, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddHostedService<DeadlineReminderBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<MyLocalizationMiddleware>();
app.MapHub<ToDoHub>("/my-hub/todo");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.Run();
