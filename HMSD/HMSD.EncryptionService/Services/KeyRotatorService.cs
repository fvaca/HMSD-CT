using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using HMSD.EncryptionService.Services.Interface;
using HMSD.EncryptionService.Utils;
using Microsoft.Extensions.Configuration;

[assembly: InternalsVisibleTo("HMSD.EncryptionService.Tests")]
namespace HMSD.EncryptionService.Services
{
    public class KeyRotatorService : IKeyRotatorService
    {
        private const string secret = "0be1d1080caa4bc987055e44f148a65b";
        string basekey = "uxZ9UHEJNg0eK2a8xN7cFCLlGNdjA+wXsbCkxISX5rI=";
        string baseIV = "w9rVa3zRassW35ijvC4adw==";

        public string GetActiveKey()
        {
            string timekey = KeyUtils.GetTimeKey().ToString();
            
            string key = timekey + basekey.Remove(0, timekey.Length);
            string IV = timekey + baseIV.Remove(0, timekey.Length);

            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {
               
                byte[] encrypted;
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = Convert.FromBase64String(key);
                    aesAlg.IV = Convert.FromBase64String(IV);
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                swEncrypt.Write(secret);

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                
                return Convert.ToBase64String(encrypted, 0, encrypted.Length);
            }
        }

        
    }
}
