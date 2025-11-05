using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace PointOfSale.Utilities.EmailSender
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get; set; }
		private readonly string _sendGridSecret;

        public EmailSender(IConfiguration _config)
        {

			_sendGridSecret = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? string.Empty;
        }
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			try
			{
				var client = new SendGridClient(_sendGridSecret);
				var from = new EmailAddress("montemorjeraldd@gmail.com", "Password Recovery");
				var to = new EmailAddress(email);
				var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

				var response = await client.SendEmailAsync(message);

				if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
				{
					var errorBody = await response.Body.ReadAsStringAsync();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
				throw;
			}
		}

	}
}
