using System;
namespace HMSD.EncryptionService.Services.Interface
{
    public interface IEncryptorService
    {
        string Encrypt(string plainText, string key);
    }
}
