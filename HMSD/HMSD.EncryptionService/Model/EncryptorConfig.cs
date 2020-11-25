using System;
namespace HMSD.EncryptionService.Model
{
    public class EncryptorConfig
    {
        public string InitVector { get; set; }
        public int Keysize { get; set; }
    }
}
