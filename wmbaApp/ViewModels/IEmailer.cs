using System.Net.Mail;

namespace wmbaApp.ViewModels
{
    /// <summary>
    /// Interface for my own email service
    /// </summary>
    public interface IEmailer
    {
        Task SendOneAsync(string name, string email, string subject, string htmlMessage);
        Task SendToManyAsync(EmailMessage emailMessage);
    }
}
