using Microsoft.AspNetCore.SignalR;
using ToDoList.Data.Repository.Interfaces;
using ToDoList.Hubs;
using ToDoList.Hubs.Interfaces;

namespace ToDoList.Services
{
    public class DeadlineReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ToDoHub, IToDoHub> _hubContext;
        private readonly ILogger<DeadlineReminderBackgroundService> _logger;
        public DeadlineReminderBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<ToDoHub, IToDoHub> hubContext, ILogger<DeadlineReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessRemindersAsync(stoppingToken);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error processing deadline reminders");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var now = DateTime.Now;

            var tasks24h = repository.GetTasksNeeding24HoursReminder(now);
            foreach (var task in tasks24h)
            {
                await SendReminderAsync(task, "24h", cancellationToken);
                repository.Mark24HoursReminderSent(task.Id);
            }

            var tasks1h = repository.GetTasksNeeding1HourReminder(now);
            foreach (var task in tasks1h)
            {
                await SendReminderAsync(task, "1h", cancellationToken);
                repository.Mark1HourReminderSent(task.Id);
            }

        }

        private async Task SendReminderAsync(ToDoList.Data.Models.TaskData task, string reminderType, CancellationToken cancellationToken)
        {
            var deadlineFormatted = task.DeadlineAt!.Value.ToString("dd.MM.yyyy, HH:mm");

            await _hubContext.Clients
                .User(task.UserId.ToString())
                .DeadlineApproaching(
                    task.Id,
                    task.Name,
                    deadlineFormatted,
                    reminderType);
        }

    }
}
