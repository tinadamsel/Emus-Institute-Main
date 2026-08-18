namespace Logic.IHelpers
{
    public interface IEvaluationReminderService
    {
        Task SendWeeklyEvaluationRemindersAsync();
        Task<bool> HasReachedEvaluationReminderLimitAsync(string email);
    }
}
