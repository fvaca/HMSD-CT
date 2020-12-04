using System;
using HMSD.APIGateway.Service;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HMSD.APIGateway.Tests
{
    public class EndpointCallerTester
    {
        [Fact]
        public void ActiveKeyCall_IsValid()
        {
            var logger = Mock.Of<ILogger<EndpointCallerService>>();
            var service = new EndpointCallerService(logger);

            var base_address = "http://localhost:5002/";

            var keyrotator_endpoint = "KeyRotator";
            var active_key = service.CallServiceEnpoint(base_address, keyrotator_endpoint);

            var message = "Hello World!";
            var dataencryptor_endpoint = "DataEncryptor";
            var dataencryptor_param = $"?secret={message}&activekey={active_key}";
            var result = service.CallServiceEnpoint(base_address, dataencryptor_endpoint, dataencryptor_param);

            Assert.NotEqual("", result);

        }

        [Fact]
        public void ActiveKeyCall_ValidResponse()
        {
            var logger = Mock.Of<ILogger<EndpointCallerService>>();
            var service = new EndpointCallerService(logger);

            var base_address = "http://localhost:5002/";
            var endpoint = "KeyRotator";

            var result = service.CallServiceEnpoint(base_address, endpoint);

            Assert.NotNull(result);

        }

        
    }
}
