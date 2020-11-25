using System;
namespace HMSD.APIGateway.Model
{
    public class EncryptionServiceConfig
    {
        public string BaseUrl { get; set; }
        public string DecryptorEndpoint { get; set; }
        public string EncryptorEndpoint { get; set; }
        public string KeyRotator { get; set; }
    }
}
