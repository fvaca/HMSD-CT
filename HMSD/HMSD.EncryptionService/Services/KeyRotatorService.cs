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
        private const string secret = "0be1d1080caa4bc987055e44f148a65b";

        public string GetActiveKey()
        {
            return modeone();

        }

        public string modetwo()
        {
            string timepass = GetTimeKey().ToString() + "cABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {

                byte[] key_iv = Convert.FromBase64String(timepass);

                if (secret == null || secret.Length <= 0)
                    throw new ArgumentNullException("plainText");
               
                byte[] encrypted;

                // Create an AesCryptoServiceProvider object
                // with the specified key and IV.
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = key_iv;
                    aesAlg.IV = key_iv;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(secret);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                return System.Text.Encoding.UTF8.GetString(encrypted);
            }
        }

        public string modeone()
        {
            // Getting the bytes of Input String.
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(secret);
            string timepass = GetTimeKey().ToString() + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

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
                    nr = (current_time + i) - last_digit;

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
