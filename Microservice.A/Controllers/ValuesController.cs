using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Common.Models.Events;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Mvc;
using RawRabbit;

namespace Microservice.A.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IRawRabbitWrapper _rawRabbitClient;

        public ValuesController(IRawRabbitWrapper rawRabbitClient)
        {
            _rawRabbitClient = rawRabbitClient;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            //_rawRabbitClient.PublishAsync(new TestEvent { Id = 1, Name = "TestEvent" }).Wait();
            for (int i = 0; i < 10; i++)
            {
                _rawRabbitClient.PublishAsync(new TestEvent { Id = i, Name = "TestEvent" }).Wait();
            }

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
