using TimeLogger.Infrastructure.Models.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public class DataEncryption
    {
        private readonly SecurityOptions securityOptions;
        private static int EncryptionIterationSize;
        private static string EncryptionPassword;
        private static string EncryptionSaltKey;
        private static string EncryptionVIKey;

        public DataEncryption(IOptionsSnapshot<SecurityOptions> securityOptions)
        {
            this.securityOptions = securityOptions.Value;
            EncryptionIterationSize = this.securityOptions.EncryptionIterationSize;
            EncryptionPassword = this.securityOptions.EncryptionPassword;
            EncryptionSaltKey = this.securityOptions.EncryptionSaltKey;
            EncryptionVIKey = this.securityOptions.EncryptionVIKey;
        }
        
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var rfc2898DerivedBytes = new Rfc2898DeriveBytes(EncryptionPassword,
                                                Encoding.ASCII.GetBytes(EncryptionSaltKey), EncryptionIterationSize))
            {
                byte[] keyBytes = rfc2898DerivedBytes.GetBytes(256 / 8);
                using (var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros })
                {
                    var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(EncryptionVIKey));
                    byte[] cipherTextBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            cipherTextBytes = memoryStream.ToArray();
                            cryptoStream.Close();
                        }
                        memoryStream.Close();
                    }
                    return Convert.ToBase64String(cipherTextBytes);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                using (var rfc2898DerivedBytes = new Rfc2898DeriveBytes(EncryptionPassword,
                                                    Encoding.ASCII.GetBytes(EncryptionSaltKey), EncryptionIterationSize))
                {
                    byte[] keyBytes = rfc2898DerivedBytes.GetBytes(256 / 8);
                    using (var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None })
                    {
                        var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(EncryptionVIKey));
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
                            }
                        }
                    }
                }
            }
            catch
            {
                return encryptedText;
            }
        }
    }
}
