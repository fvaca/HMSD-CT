using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HMSD.APIGateway.Model;
using HMSD.APIGateway.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HMSD.APIGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DecryptorServiceController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly EncryptionServiceConfig config;
        private readonly IEndpointCallerService service;

        public DecryptorServiceController(IOptionsMonitor<EncryptionServiceConfig> optionsMonitor,
            IEndpointCallerService endpointcaller, ILogger<DecryptorServiceController> logger)
        {
            config = optionsMonitor.CurrentValue;
            service = endpointcaller;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Decrypt(string secret)
        {
            try
            {
                var activekey = service.CallServiceEnpoint(config.BaseUrl, config.KeyRotator);
                _logger.LogInformation($"KeyRotator [activekey: {activekey} secret={secret}]");
         
                string urlparam = $"?secret={secret}&activekey={activekey}";
                string result = service.CallServiceEnpoint(config.BaseUrl, config.DecryptorEndpoint, urlparam);
                _logger.LogInformation($"EncryptorEndpoint [activekey: {activekey} secret={secret}]");

                return Ok(result);
            }
            catch (HttpResponseException ex)
            {
                _logger.LogError($"FAILED: Decrypt - {ex.Message}");
                return StatusCode(ex.Status, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FAILED: Decrypt - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }


        }
    }
}
