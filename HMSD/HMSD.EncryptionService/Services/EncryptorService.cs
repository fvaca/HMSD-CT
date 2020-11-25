using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.Extensions.Configuration;
using HMSD.EncryptionService.Exceptions;

namespace HMSD.EncryptionService.Services
{
    public class EncryptorService : IEncryptorService
    {
        //IEncryptorWorker worker;
        //public EncryptorService(IEncryptorWorker encrypworker)
        //{
        //    worker = encrypworker;
        //}

        public string Encrypt(string messsge, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;

            string truekey = GetTrueKey(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(truekey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                {
                    streamWriter.Write(messsge);
                }

                array = memoryStream.ToArray();
            }

            return Convert.ToBase64String(array);

        }

        private string GetTrueKey(string key)
        {
            throw new NotImplementedException();

            //long time = worker.ConverKeyToUnixtime(key);


            //const int prime = 101;
            //const int notprime = 100;


        }
    }
}
