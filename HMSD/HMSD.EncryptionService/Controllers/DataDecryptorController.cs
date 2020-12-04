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
    public class DataDecryptorController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<EncryptorConfig> enconfigmonitor;
        private readonly IEncryptorService service;
        public DataDecryptorController(IOptionsMonitor<EncryptorConfig> optionsMonitor,
            IEncryptorService encryptorservice, ILogger<DataDecryptorController> logger)
        {
            enconfigmonitor = optionsMonitor;
            service = encryptorservice;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DecryptSecret(string secret, string activekey)
        {
            activekey = activekey.Replace(" ", "+");
            _logger.LogInformation($"DecryptSecret [activekey: {activekey}] [secret={secret}]");            

            try
            {                
                return Ok(service.Decrypt(secret, activekey));
            }
            catch (Exception ex)
            {
                _logger.LogError($"FAILED: DecryptSecret - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
           
        }
    }
}