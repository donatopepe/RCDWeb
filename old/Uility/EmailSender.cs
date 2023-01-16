using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using RCDWeb.Areas.RCD.Models;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true);
    Task SendEmailTextAsync(string email, string testosms, string content);
}

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;


    public EmailSender(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true)
    {
        //_logger.LogInformation(">>>>>>>>>>>>>>>>" + email + subject + message);
        try
        {
            // Credentials
            //Console.WriteLine(">>>>>>>>>>>>>>>>" + _emailSettings.SmtpServer + _emailSettings.SmtpUsername);
            var credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            //_logger.LogInformation(">>>>>>>>>>>>>>>>SmtpUsername:" + _emailSettings.SmtpUsername + " SmtpPassword:" + _emailSettings.SmtpPassword);
            // Mail message
            var mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.SmtpUsername, _emailSettings.SenderName),
                //From = new MailAddress("noreply@"+ _emailSettings.SenderName+".com", _emailSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = isBodyHtml
            };
            //_logger.LogInformation(">>>>>>>>>>>>>>>>SmtpUsername:" + _emailSettings.SmtpUsername + " SenderName:" + _emailSettings.SenderName);
            mail.To.Add(new MailAddress(email));
            //_logger.LogInformation(">>>>>>>>>>>>>>>> creo mail SmtpPort:" + _emailSettings.SmtpPort + " SmtpServer:" + _emailSettings.SmtpServer);
            // Smtp client
            var client = new SmtpClient()
            {
                Port = (int)_emailSettings.SmtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = _emailSettings.SmtpServer,
                EnableSsl = (bool)_emailSettings.SmtpSSL,
                Credentials = credentials
            };
            //_logger.LogInformation(">>>>>>>>>>>>>>>>Port: " + client.Port + " EnableSsl:" + client.EnableSsl);
            // Send it...         
            client.Send(mail);
        }
        catch (Exception ex)
        {
            // TODO: handle exception
            //throw new InvalidOperationException(ex.Message);
            //_logger.LogError(ex.Message);
            return Task.FromResult(new { result = false , Message = ex.Message });
        }

        return Task.FromResult(new { result=true, Message = "" });
    }
    public Task SendEmailTextAsync(string email, string subject, string message)
    {
        return SendEmailAsync(email, subject, message, false);
    }
}

