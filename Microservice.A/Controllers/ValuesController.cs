using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.A.VietCredit;
using Microservice.Common.Exceptions;
using Microservice.Common.Models.Events;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RawRabbit;

namespace Microservice.A.Controllers
{
    [Route("api/microservice-a")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRawRabbitWrapper _rawRabbitClient;
        private readonly ILogger _logger;

        public ValuesController(IRawRabbitWrapper rawRabbitClient, ILogger<ValuesController> logger)
        {
            _rawRabbitClient = rawRabbitClient;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IActionResult> Get()
        {
            _logger.LogInformation("Test log information");
            _logger.LogWarning("Test log warning");
            _logger.LogError("Test log error");
            //var checkScoring = new CheckScoring();
            //checkScoring.Run(_logger);
            return Ok();
        }
    }
}
