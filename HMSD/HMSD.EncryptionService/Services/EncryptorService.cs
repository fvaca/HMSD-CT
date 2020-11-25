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

namespace HMSD.EncryptionService.Services
{
    public class EncryptorService : IEncryptorService
    {
        private readonly EncryptorConfig enconfig;


        public EncryptorService(IOptionsMonitor<EncryptorConfig> configmonitor)
        {
            enconfig = configmonitor.CurrentValue;
        }

        public string Decrypt(string secret, string activekey)
        {
            string passPhrase = GetTrueKey(activekey);

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

        private string GetTrueKey(string activekey)
        {
            string SecurityKey = GetTimeKey().ToString() + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678912";
            byte[] toEncryptArray = Convert.FromBase64String(activekey);
            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(SecurityKey));
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            objTripleDESCryptoService.Clear();

            //Convert and return the decrypted data/byte into string format.
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private int GetTimeKey()
        {
            int current_time = DateTimeToTimeKey(DateTime.Now);
            int last_digit = current_time % 10;

            if (IsFibonacci(last_digit))
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
