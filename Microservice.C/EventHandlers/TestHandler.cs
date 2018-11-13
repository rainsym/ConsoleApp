using Microservice.Common.Models.Events;
using Microservice.Common.RawRabbit;
using Microsoft.Extensions.Logging;
using RawRabbit.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.C.EventHandlers
{
    public class TestHandler : IMessageHandle<TestEvent>
    {
        private readonly ILogger _logger;

        public TestHandler(ILogger<TestHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(TestEvent e, IMessageContext context)
        {
            _logger.LogInformation("C: taken event");
        }
    }
}
