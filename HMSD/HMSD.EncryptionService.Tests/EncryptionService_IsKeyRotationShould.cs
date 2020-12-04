using System;
using System.IO;
using System.Text;
using System.Threading;
using HMSD.EncryptionService.Services;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.Extensions.Configuration;
using Xunit;
namespace HMSD.EncryptionService.Tests
{
    public class EncryptionService_IsKeyRotationShould
    {

        [Fact]
        public void IsKeyRotation_LongIntervalPresent_WaitForNewKey_ReturnsNewKey()
        {
            
            IKeyRotatorService service = new KeyRotatorService();

            //act
            string firstkey_result, secondkey_result;
            firstkey_result = secondkey_result = service.GetActiveKey();

            Thread.Sleep(30000);
            secondkey_result = service.GetActiveKey();

            //assert
            Assert.True(firstkey_result != secondkey_result);

        }

        [Fact]
        public void IsKeyRotation_ShortIntervalPresent_WaitForNewKey_ReturnsSameKey()
        {
           
            IKeyRotatorService service = new KeyRotatorService();
            string firstkey_result, secondkey_result;

            //act
            firstkey_result = service.GetActiveKey();
            Thread.Sleep(40);
            secondkey_result = service.GetActiveKey();

            //assert
            Assert.True(firstkey_result == secondkey_result);

        }

    }
}
