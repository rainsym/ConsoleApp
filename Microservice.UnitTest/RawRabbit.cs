using Microservice.Common.Models;
using Microservice.Common.Models.Events;
using Microservice.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Microservice.UnitTest
{
    public class RawRabbit
    {
        private readonly RawRabbitWrapper _rawRabbitClient;

        public RawRabbit()
        {
            var _context = new Mock<EventDbContext>().Object;
            var _busClient = new Mock<IBusClient>().Object;
            var _logger = new Mock<ILogger<RawRabbitWrapper>>().Object;
            _rawRabbitClient = new RawRabbitWrapper(_context, _busClient, _logger);
        }

        [Fact]
        public void Publish_10_Event_At_The_Same_Time()
        {
            _rawRabbitClient.PublishAsync(new TestEvent { Id = 0, Name = "TestEvent" }).Wait();

            //var array = new List<int>();
            //for (int i = 0; i < 10; i++)
            //{
            //    array.Add(i + 1);
            //}
            //Parallel.ForEach(array, (i) =>
            //{
            //    _rawRabbitClient.PublishAsync(new TestEvent { Id = i, Name = "TestEvent" }).Wait();
            //});
        }
    }
}
