using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using RCDWeb.Areas.RCD.Models;
using System.ComponentModel.DataAnnotations;
using Elmah;

namespace RCDWeb.Areas.RCD.Uility
{
    public class EmailSettings
    {
        [Key]
        public int id { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public string SmtpServer { get; set; }
        public int? SmtpPort { get; set; }
        public bool? SmtpSSL { get; set; }
        public string SenderName { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool isSimulated { get; set; }
        public string SimulatedEmail { get; set; } 
        //public string PopServer { get; set; }
        //public int? PopPort { get; set; }
        //public bool? PopSSL { get; set; }
        //public string PopUsername { get; set; }
        //public string PopPassword { get; set; }
    }

    public class EmailTipo
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IEmailSender
    {
        EmailTipo SendEmail(string email, string subject, string message, bool isBodyHtml = true);
        EmailTipo SendEmailText(string email, string testosms, string content);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;


        public EmailSender(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public EmailTipo SendEmail(string email, string subject, string message, bool isBodyHtml = true)
        {
            email = email.Replace(";", ",");
            if (_emailSettings.isSimulated)
            {
                subject += " > ("+email+")";
                email = _emailSettings.SimulatedEmail;
            }
            var result = new EmailTipo() { Email = email, Message = message, IsBodyHtml = isBodyHtml, Subject = subject };
            if (_emailSettings == null)
            {
                result.Result = false;
                result.ErrorMessage = "No Email Setting";
                return result;
            }

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
                var emailArray = email.Split(',');
                bool firstEmail = false;
                foreach (var item in emailArray)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        if (firstEmail)
                        {
                            mail.CC.Add(new MailAddress(item));
                        }
                        else
                        {
                            firstEmail = true;
                            mail.To.Add(new MailAddress(item));
                        }
                    }
                }
                //mail.To.Add(new MailAddress(email));
                //_logger.LogInformation(">>>>>>>>>>>>>>>> creo mail SmtpPort:" + _emailSettings.SmtpPort + " SmtpServer:" + _emailSettings.SmtpServer);
                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.SmtpPort == null ? 25 : (int)_emailSettings.SmtpPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.SmtpServer,
                    EnableSsl = _emailSettings.SmtpPort == null ? false : (bool)_emailSettings.SmtpSSL,
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
                result.Result = false;
                result.ErrorMessage = ex.Message;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                ErrorSignal.FromCurrentContext().Raise(ex);
                return result;
            }

            result.Result = true;
            return result;
        }

        public EmailTipo SendEmailText(string email, string subject, string message)
        {
            return SendEmail(email, subject, message, false);
        }
    }

}