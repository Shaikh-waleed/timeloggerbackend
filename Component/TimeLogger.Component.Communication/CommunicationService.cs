using TimeLogger.Component.Interfaces.Communication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Communication
{
    public class CommunicationService : ICommunicationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public CommunicationService(IEmailService emailService, ISmsService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<bool> SendEmail(string subject, string content, string toEmail, string fromEmail = null, string fromName = null, string attachment = null)
        {
           return await _emailService.SendEmail(subject, content, toEmail, fromEmail, fromName, attachment);
        }

        public async Task<bool> SendSms(string content, string toPhone, string fromPhone = null)
        {
            return await _smsService.SendSms(content, toPhone, fromPhone);
        }
    }
}
