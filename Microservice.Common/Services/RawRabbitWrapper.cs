using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microservice.Common.Models;
using Newtonsoft.Json;
using RawRabbit;
using RawRabbit.Configuration.Publish;

namespace Microservice.Common.Services
{
    public class RawRabbitWrapper : IRawRabbitWrapper
    {
        private readonly EventDbContext _context;
        private readonly IBusClient _rawRabbitClient;

        public RawRabbitWrapper(EventDbContext context, IBusClient rawRabbitClient)
        {
            _context = context;
            _rawRabbitClient = rawRabbitClient;
        }

        public async Task PublishAsync<T>(T message = default(T), Guid globalMessageId = default(Guid), Action<IPublishConfigurationBuilder> configuration = null)
        {
            //Task.Run(() =>
            //{

            //});
            var eventTracker = new EventTracker
            {
                MessageId = globalMessageId,
                Name = message.GetType().ToString(),
                PayLoad = JsonConvert.SerializeObject(message)
            };

            _context.Add(eventTracker);

            await _rawRabbitClient.PublishAsync(message, globalMessageId, configuration);

            _context.SaveChanges();
        }
    }
}
