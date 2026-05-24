using Logic.IHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logic.Services
{
    public class EvaluationReminderJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EvaluationReminderJob> _logger;

        public EvaluationReminderJob(IServiceScopeFactory scopeFactory, ILogger<EvaluationReminderJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task SendWeeklyRemindersAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IEvaluationReminderService>();
                await reminderService.SendWeeklyEvaluationRemindersAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send weekly evaluation reminder emails.");
                throw;
            }
        }
    }
}
