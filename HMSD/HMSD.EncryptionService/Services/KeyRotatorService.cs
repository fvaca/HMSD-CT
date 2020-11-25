using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace HMSD.EncryptionService.Services
{
    public class KeyRotatorService : IKeyRotatorService
    {
        private const string initVector = "pemgail9uzpgzl88";
        private const int keysize = 256;

        public KeyRotatorService(IConfiguration config)
        {
            config[""]
        }

        public string GetActiveKey()
        {            
            string passPhrase = "; *-RVcpcjHL <%$k: 7Sta(g < 4W~zj~Y";
            string timepass = GetTimeKey().ToString() + ".ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678912";

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(timepass);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
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

        private int GetTimeKey()
        {
            int current_time = DateTimeToTimeKey(DateTime.Now);
            int last_digit = current_time % 10;

            if(IsFibonacci(last_digit))
                return current_time;

          
            if (last_digit == 9)
                return current_time - 1;

            int nr = 0;
            for (int i = 0; i <= last_digit; i++)
            {
                if (IsFibonacci(i))
                    nr = i;
            }

            return nr;
        }

        private static bool IsPerfectSquare(int x)
        {
            int s = (int)Math.Sqrt(x);
            return (s * s == x);
        }

        private static bool IsFibonacci(int n)
        {
            return IsPerfectSquare(5 * n * n + 4) ||
              IsPerfectSquare(5 * n * n - 4);
        }

        private static DateTime TimeKeyToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMinutes(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private static int DateTimeToTimeKey(DateTime datetime)
        {
            return (int)(datetime.Subtract(new DateTime(1970, 1, 1))).TotalMinutes;
        }
    }
}
