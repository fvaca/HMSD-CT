using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMSD.EncryptionService.Model;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMSD.EncryptionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataEncryptorController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<EncryptorConfig> enconfigmonitor;
        private readonly IEncryptorService service;
        public DataEncryptorController(IOptionsMonitor<EncryptorConfig> optionsMonitor,
            IEncryptorService encryptorservice, ILogger<DataEncryptorController> logger)
        {
            enconfigmonitor = optionsMonitor;
            service = encryptorservice;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult EncryptSecret(string secret, string activekey)
        {
            activekey = activekey.Replace(" ", "+");
            _logger.LogInformation($"EncryptSecret [activekey: {activekey}] [secret={secret}]");

            try
            {                
                return Ok(service.Encrypt(secret, activekey));
            }
            catch (Exception ex)
            {
                _logger.LogError($"FAILED: EncryptSecret - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
          
        }
    }
}