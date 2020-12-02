using System;
using System.IO;
using System.Text;
using System.Threading;
using HMSD.EncryptionService.Model;
using HMSD.EncryptionService.Services;
using HMSD.EncryptionService.Services.Interface;
using HMSD.EncryptionService.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HMSD.EncryptionService.Tests
{
    public class EncryptionService_IsEncryptedShould
    {

        [Fact]
        public void IsSameTrueKey()
        {
            Encoding.UTF8.GetBytes("SaltBytes");
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);

            
            string activekey = r_service.GetActiveKey();
            Thread.Sleep(10000);
            string new_activekey = r_service.GetActiveKey();

            string timekey_En = EncryptorService.GetTrueKey(activekey);

            Assert.NotEqual(activekey, new_activekey);
            Assert.Equal("0be1d1080caa4bc987055e44f148a65b", timekey_En);
        }

        [Fact]
        public void IsEncrypted_GivenASecretWaitforNewKey_ReturnsEncryptedSecret()
        {
            Encoding.UTF8.GetBytes("SaltBytes");
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);

            //act
            string secret = "hello World";
            string activekey = r_service.GetActiveKey();
            string en_result = service.Encrypt(secret, activekey);

            string new_activekey = r_service.GetActiveKey();
            while (activekey == new_activekey)
            {
                Thread.Sleep(3000);
                new_activekey = r_service.GetActiveKey();
            }
            string result = service.Decrypt(en_result, activekey);

            //assert
            Assert.Equal(secret, result);
        }


        [Fact]
        public void IsEncrypted_GivenASecretandSameActiveKey_ReturnsEncryptedSecret()
        {
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);

            //act
            string secret = "hello World";
            string activekey = r_service.GetActiveKey();
            string en_result = service.Encrypt(secret, activekey);
            string result = service.Decrypt(en_result, activekey);

            //assert
            Assert.Equal(secret, result);

        }

       


    }
}
