using Microsoft.AspNetCore.Identity.UI.Services;

namespace PointOfSale.Utilities.EmailSender
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
