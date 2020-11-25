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
            //arrange
            var appSettings = @"{""initVector"": ""gfedcba9uzpjih88"" , ""keysize"": ""256"" , ""passPhrase"": ""; *-RVcpcjHL <%$k: 7Sta(g < 4W~zj~Y""}";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            var config = builder.Build();
            IKeyRotatorService service = new KeyRotatorService(config);

            //act
            string firstkey_result, secondkey_result;
            firstkey_result = secondkey_result = service.GetActiveKey();

            Thread.Sleep(180000);
            secondkey_result = service.GetActiveKey();

            //assert
            Assert.True(firstkey_result != secondkey_result);

        }

        [Fact]
        public void IsKeyRotation_ShortIntervalPresent_WaitForNewKey_ReturnsSameKey()
        {
            //arrange
            var appSettings = @"{""initVector"": ""gfedcba9uzpjih88"" , ""keysize"": ""256"" , ""passPhrase"": ""; *-RVcpcjHL <%$k: 7Sta(g < 4W~zj~Y"" }";
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));
            var config = builder.Build();
            IKeyRotatorService service = new KeyRotatorService(config);

            //act
            string firstkey_result, secondkey_result;
            firstkey_result = secondkey_result = service.GetActiveKey();

            Thread.Sleep(40);
            secondkey_result = service.GetActiveKey();

            //assert
            Assert.True(firstkey_result == secondkey_result);

        }

    }
}
