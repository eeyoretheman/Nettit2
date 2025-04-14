using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace Nettit.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        SmtpClient mySmtpClient = new SmtpClient("smtp.abv.bg");
        mySmtpClient.UseDefaultCredentials = false;

        mySmtpClient.Port = 465;

        System.Net.NetworkCredential basicAuthenticationInfo = new
           System.Net.NetworkCredential("nettit@abv.bg", "sax%.Jd-}iv9+jN");

        mySmtpClient.Credentials = basicAuthenticationInfo;

        MailAddress from = new MailAddress("nettit@abv.bg", "Nettit");
        MailAddress to = new MailAddress(toEmail, "Nettit user");
        MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

        myMail.Subject = subject;
        myMail.SubjectEncoding = System.Text.Encoding.UTF8;

        myMail.Body = message;
        myMail.BodyEncoding = System.Text.Encoding.UTF8;

        myMail.IsBodyHtml = true;

        mySmtpClient.Send(myMail);
    }
}