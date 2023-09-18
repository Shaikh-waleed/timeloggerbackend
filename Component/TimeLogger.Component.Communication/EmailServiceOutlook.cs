using TimeLogger.Component.Interfaces.Communication;
using TimeLogger.Infrastructure.Models.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TimeLogger.Component.Communication
{
    public class EmailServiceOutlook : IEmailService
    {
        private readonly OutlookOptions outlookOptions;
        private string FromName = string.Empty;
        private string FromEmail = string.Empty;
        private string Username = string.Empty;
        private string Password = string.Empty;
        private string Host = string.Empty;
        private int Port;
        private bool EnableSsl;
        private bool UseDefaultCredentials;

        public EmailServiceOutlook(IOptionsSnapshot<OutlookOptions> outlookOptions)
        {
            this.outlookOptions = outlookOptions.Value;
            FromName = this.outlookOptions.FromName;
            FromEmail = this.outlookOptions.FromEmail;
            Username = this.outlookOptions.Username;
            Password = this.outlookOptions.Password;
            Host = this.outlookOptions.Host;
            Port = this.outlookOptions.Port;
            EnableSsl = this.outlookOptions.EnableSsl;
            UseDefaultCredentials = this.outlookOptions.UseDefaultCredentials;
        }

        public async Task<bool> SendEmail(string subject, string content, string toEmail, string fromEmail = null, string fromName = null, string attachment = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subject))
                    subject = FromName;
                if (string.IsNullOrWhiteSpace(fromName))
                    fromName = FromName;
                if (string.IsNullOrWhiteSpace(fromEmail))
                    fromEmail = FromEmail;

                var mailMsg = new MailMessage()
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };

                if (!string.IsNullOrEmpty(attachment))
                    mailMsg.Attachments.Add(new Attachment(attachment));

                mailMsg.To.Add(toEmail);

                SmtpClient smtpClient = new SmtpClient
                {
                    Host = Host,
                    Port = Port,
                    EnableSsl = EnableSsl,
                    UseDefaultCredentials = UseDefaultCredentials,
                    Credentials = new System.Net.NetworkCredential(Username, Password)
                };
                smtpClient.Send(mailMsg);
                mailMsg.Dispose();
            }
            catch (Exception ex)
            {
                var test = ex;
                return false;
            }
            return true;
        }
    }
}
