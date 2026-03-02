using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            using var smtp = new SmtpClient
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["Smtp:User"],
                    _configuration["Smtp:Pass"]
                )
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:User"], "Pagina Proyecto"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);
            await smtp.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ ERROR SMTP:");
            Console.WriteLine(ex.Message);
            throw;
        }
    }

}
