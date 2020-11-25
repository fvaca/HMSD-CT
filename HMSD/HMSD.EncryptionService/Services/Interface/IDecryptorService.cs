using System;
namespace HMSD.EncryptionService.Services.Interface
{
    public interface IDecryptorService
    {
        string Decrypt(string plainText, string key);
    }
}
