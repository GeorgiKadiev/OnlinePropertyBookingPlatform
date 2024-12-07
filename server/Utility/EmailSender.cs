using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using DotNetEnv;

namespace OnlinePropertyBookingPlatform.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get;set; }
        public EmailSender(IConfiguration _config) 
        {
            Env.Load();
            SendGridSecret = Environment.GetEnvironmentVariable("SENDGRIDAPIKEY");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var client = new SendGrid.SendGridClient(SendGridSecret);
            var frmo = new EmailAddress("delyanboyanov@hotmail.com", "BookingPlatform");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(frmo, to, subject, "", htmlMessage);
            return client.SendEmailAsync(message);
            

        }

    }
}
