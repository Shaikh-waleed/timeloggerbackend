using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Interfaces.Communication
{
    public interface ICommunicationService
    {
        Task<bool> SendEmail(string subject, string content, string toEmail, string fromEmail = null, string fromName = null, string attachment = null);
        Task<bool> SendSms(string content, string toPhone, string fromPhone = null);
    }
}
