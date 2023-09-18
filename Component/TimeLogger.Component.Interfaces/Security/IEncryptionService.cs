using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Component.Interfaces.Security
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
