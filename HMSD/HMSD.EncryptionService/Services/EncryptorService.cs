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
        private readonly string initVector;
        private readonly int keysize;
        private readonly string passPhrase;

        public EncryptorService(IConfiguration config)
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

        public string Encrypt(string secret, string activekey)
        {

            // Getting the bytes of Input String.
            byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(secret);

            MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(activekey));
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
            throw new NotImplementedException();

        }

        private string GetTrueKey(string activekey)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(activekey);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
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
    }
}
