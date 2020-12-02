using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.Extensions.Configuration;
using HMSD.EncryptionService.Exceptions;
using HMSD.EncryptionService.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using HMSD.EncryptionService.Utils;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HMSD.EncryptionService.Tests")]
namespace HMSD.EncryptionService.Services
{
    public class EncryptorService : IEncryptorService
    {
        private readonly ILogger _logger;
        private readonly EncryptorConfig enconfig;
         
        public EncryptorService(IOptionsMonitor<EncryptorConfig> configmonitor, ILogger<EncryptorService> logger)
        {
            enconfig = configmonitor.CurrentValue;
            _logger = logger;
        }

        public string Decrypt(string secret, string activekey)
        {
            string passPhrase = GetTrueKey(activekey);
            _logger.LogInformation($"Encrypt [GetTrueKey: {passPhrase}]");

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(enconfig.InitVector);
            byte[] cipherTextBytes = Convert.FromBase64String(secret);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(enconfig.Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public string Encrypt(string secret, string activekey)
        {

            string passPhrase = GetTrueKey(activekey);
            _logger.LogInformation($"Encrypt [GetTrueKey: {passPhrase}]");

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(enconfig.InitVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(secret);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(enconfig.Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);

        }

        internal static string GetTrueKey(string activekey)
        {
            string plaintext = null;

            string basekey = "uxZ9UHEJNg0eK2a8xN7cFCLlGNdjA+wXsbCkxISX5rI=";
            string baseIV = "w9rVa3zRassW35ijvC4adw==";

            string timekey = KeyUtils.GetTimeKey().ToString();
            string key = timekey + basekey.Remove(0, timekey.Length);
            string IV = timekey + baseIV.Remove(0, timekey.Length);

            byte[] activekey_64 = Convert.FromBase64String(activekey);
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = Convert.FromBase64String(IV);
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                using (MemoryStream msDecrypt = new MemoryStream(activekey_64))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;

        }
    }
}
