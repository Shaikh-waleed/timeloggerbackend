using TimeLogger.Component.Interfaces.Communication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Communication
{
    public class SmsServiceTest : ISmsService
    {
        public async Task<bool> SendSms(string content, string toPhone, string fromPhone = null)
        {
            throw new NotImplementedException();
        }
    }
}
