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
        public void sEncrypted_EncryptDecryptMessage_WithFiveDifferentKey()
        {
            Encoding.UTF8.GetBytes("SaltBytes");
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);


            string message = "hello World";
            string original_active_key = r_service.GetActiveKey();
            string encrypted_message = service.Encrypt(message, original_active_key);

            int counter = 0;
            string new_active_key = original_active_key;

            do
            {
                Thread.Sleep(5000); //wait for a new key
                new_active_key = r_service.GetActiveKey();
                if (new_active_key != original_active_key)
                {
                    string result = service.Decrypt(encrypted_message, new_active_key);
                    Assert.NotEqual(original_active_key, new_active_key);
                    Assert.Equal(message, result);
                    counter++;
                }               

            } while (counter < 5);
            
        }


        [Fact]
        public void IsEncrypted_EncryptDecryptMessage_WithSameKey()
        {
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);

            //act
            string message = "hello World";
            string active_key = r_service.GetActiveKey();
                     
            string encrypted_message = service.Encrypt(message, active_key);
            string result = service.Decrypt(encrypted_message, active_key);

            //assert
            Assert.Equal(message, result);

        }

        [Fact]
        public void IsEncrypted_IsTrueKeyValid()
        {
            EncryptorConfig enconfig = new EncryptorConfig() { InitVector = "gfedcba9uzpjih88", Keysize = 256 };
            var monitor = Mock.Of<IOptionsMonitor<EncryptorConfig>>(_ => _.CurrentValue == enconfig);
            var logger = Mock.Of<ILogger<EncryptorService>>();
            var secret = "0be1d1080caa4bc987055e44f148a65b";

            IKeyRotatorService r_service = new KeyRotatorService();
            IEncryptorService service = new EncryptorService(monitor, logger);

            //act
            var active_key = r_service.GetActiveKey();
            var true_key = EncryptorService.GetTrueKey(active_key);


            //assert
            Assert.Equal(secret, true_key);

        }






    }
}
