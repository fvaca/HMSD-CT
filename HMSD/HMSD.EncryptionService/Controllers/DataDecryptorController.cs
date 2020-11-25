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
    public class DataDecryptorController : ControllerBase
    {
        private readonly IOptionsMonitor<EncryptorConfig> enconfigmonitor;
        private readonly IEncryptorService service;
        public DataDecryptorController(IOptionsMonitor<EncryptorConfig> optionsMonitor, IEncryptorService encryptorservice)
        {
            enconfigmonitor = optionsMonitor;
            service = encryptorservice;
        }

        [HttpGet]
        public string DecryptSecret(string secret, string activekey)
        {
            return service.Decrypt(secret, activekey);
        }
    }
}