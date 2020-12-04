using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMSD.EncryptionService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HMSD.EncryptionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeyRotatorController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IKeyRotatorService service;
        public KeyRotatorController(IKeyRotatorService rotatorservice, ILogger<KeyRotatorController> logger)
        {
            service = rotatorservice;
            _logger = logger;
        }
        // GET: api/values
        [HttpGet]
        public IActionResult GetKey()
        {
            try
            {
                var activekey = service.GetActiveKey();
                _logger.LogInformation($"GetKey [activekey: {activekey}]");
                return Ok(activekey);
            }
            catch (Exception ex)
            {

                _logger.LogError($"FAILED: GetKey - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
           
        }
    }
}
