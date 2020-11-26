using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMSD.EncryptionService.Model;
using HMSD.EncryptionService.Services.Interface;
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
        public DataEncryptorController(IOptionsMonitor<EncryptorConfig> optionsMonitor, IEncryptorService encryptorservice, ILogger<DataEncryptorController> logger)
        {
            enconfigmonitor = optionsMonitor;
            service = encryptorservice;
            _logger = logger;
        }

        [HttpGet]
        public string EncryptSecret(string secret, string activekey)
        {
            _logger.LogInformation($"activekey: {activekey} secret={secret}");
            return service.Encrypt(secret, activekey);
        }
    }
}