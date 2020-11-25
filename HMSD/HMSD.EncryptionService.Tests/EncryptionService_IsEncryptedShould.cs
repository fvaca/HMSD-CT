using System;
using System.IO;
using System.Text;
using System.Threading;
using HMSD.EncryptionService.Model;
using HMSD.EncryptionService.Services;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HMSD.EncryptionService.Tests
{
    public class EncryptionService_IsEncryptedShould
    {
        
        [Fact]
        public void IsEncrypted_GivenASecretandSameActiveKey_ReturnsEncryptedSecret()
        {            
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor);

            //act
            string secret = "hello World";
            string activekey = r_service.GetActiveKey();
            string en_result = service.Encrypt(secret, activekey);
            string result = service.Decrypt(en_result, activekey);

            //assert
            Assert.Equal(secret, result);

        }

        [Fact]
        public void IsEncrypted_GivenASecretandOldKey_ReturnsEncryptedSecret()
        {            

            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor);

            //act
            string secret = "hello World";
            string activekey = r_service.GetActiveKey();
            string en_result = service.Encrypt(secret, activekey);

            Thread.Sleep(180000);
            string result = service.Decrypt(en_result, activekey);

            //assert
            Assert.Equal(secret, result);
        }

    }
}
