using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace uStoreAPI.Services
{
    public class EmailService
    {
        private readonly string apiKey;
        private string templateId;
        private readonly EmailAddress fromAddress;
        private readonly SendGridClient client;
        private IConfiguration configuration;
        public EmailService(IConfiguration _configuration)
        {
            configuration = _configuration;
            client = new SendGridClient(configuration["SENDGRID_API_KEY"]!);
            fromAddress = new EmailAddress("ustoreCEO@gmail.com", "uStore");
        }

        public async Task<bool> SendEmailConfirmacionCuentaUser(string toEmail, string subject, Dictionary<string, string> templateData)
        {
            templateId = "d-ce8a5aaf1c3a47608a27066b7235b966";
            var to = new EmailAddress(toEmail);
            var msg = new SendGridMessage
            {
                From = fromAddress,
                TemplateId = templateId,
                Subject = subject
            };

            msg.AddTo(to);
            msg.SetTemplateData(templateData);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SendEmailRecoverCuentaUser(string toEmail, string subject, Dictionary<string, string> templateData)
        {
            templateId = "d-018214066b7d401f965a271dd1dd520b";
            var to = new EmailAddress(toEmail);
            var msg = new SendGridMessage
            {
                From = fromAddress,
                TemplateId = templateId,
                Subject = subject
            };

            msg.AddTo(to);
            msg.SetTemplateData(templateData);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
