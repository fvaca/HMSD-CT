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
        private readonly string initVector;
        private readonly int keysize;
        private readonly string passPhrase;
        private const string secret = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678912";

        public KeyRotatorService(IConfiguration config)
        {
            try
            {
                initVector = config["initVector"];
                keysize = int.Parse(config["keysize"]);
                passPhrase = config["passPhrase"];
            }
            catch (Exception ex)
            {
                throw; //future: caption custom exception
            }
        }

        public string GetActiveKey()
        {          
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

        public string GetActiveKey2()
        {
            // Getting the bytes of Input String.
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(secret);
            string timepass = GetTimeKey().ToString() + ".ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz012345678912";

            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(timepass));
            //De-allocatinng the memory after doing the Job.
            objMD5CryptoService.Clear();

            var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDESCryptoService.Key = securityKeyArray;
            //Mode of the Crypto service is Electronic Code Book.
            objTripleDESCryptoService.Mode = CipherMode.ECB;
            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDESCryptoService.Padding = PaddingMode.PKCS7;


            var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
            objTripleDESCryptoService.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);

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
