using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Interfaces.Communication
{
    public interface ISmsService
    {
        Task<bool> SendSms(string content, string toPhone, string fromPhone = null);
    }
}
