using System.Net.Mail;
using System.Net;

namespace OnlinePropertyBookingPlatform.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
        string mail = "del40ismost@gmail.com";
        string pw = "Test12345678!";
        SmtpClient client = new SmtpClient("smtp.office365.com", 587)
        {EnableSsl = true, Credentials = new NetworkCredential(mail,pw)};
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

    }
}
