using SendGrid;
using SendGrid.Helpers.Mail;

namespace uStoreAPI.Services
{
    public class EmailService
    {
        private string? templateId;
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

        public async Task<bool> SendEmailNotificacionApp(string toEmail, string subject, Dictionary<string, string> templateData, bool IsImagenNull = false)
        {
            if (IsImagenNull)
            {
                templateId = "d-0d2cb2a09843449ba630e3955f26e504";
            }
            else
            {
                templateId = "d-577b4ba4168046f89ac520afd7f694c0";
            }
            
            var to = new EmailAddress(toEmail);
            var msg = new SendGridMessage
            {
                From = fromAddress,
                TemplateId = templateId,
                Subject = subject
            };

            templateData["subject"] = subject;

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

        public async Task<bool> SendEmailNotificacionApartado(string? toEmail, string subject, Dictionary<string, string> templateData)
        {
            templateId = "d-0ae666b883204563bbd1c7690b5f2466";
            var to = new EmailAddress(toEmail);
            var msg = new SendGridMessage
            {
                From = fromAddress,
                TemplateId = templateId,
                Subject = subject
            };

            templateData["subject"] = subject;

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

        public async Task<bool> SendEmailNotificacionSolicitud(string? toEmail, string subject, Dictionary<string, string> templateData, string tipoNotificacionSolicitud)
        {
            switch (tipoNotificacionSolicitud)
            {
                case "aceptada": templateId = "d-081db4e63d3d4770a8bd342ed3cdd1f8";
                    break;
                case "vencida": templateId = "d-3f09c5a63459453b9ea88b7a35c399af";
                    break;
                case "rechazada": templateId = "d-d778c8c64aac4f64a9eb0a22f63cb09f";
                    break;
                case "cancelada": templateId = "d-a40b0675552a40268004442948310d40";
                    break;
            }

            
            var to = new EmailAddress(toEmail);
            var msg = new SendGridMessage
            {
                From = fromAddress,
                TemplateId = templateId,
                Subject = subject
            };

            templateData["subject"] = subject;

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
