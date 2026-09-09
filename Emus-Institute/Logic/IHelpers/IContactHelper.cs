namespace Logic.IHelpers
{
    public interface IContactHelper
    {
        (bool Success, string Message) SubmitContactMessage(string name, string email, string subject, string message);
    }
}
