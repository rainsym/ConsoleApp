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
            globalMessageId = globalMessageId == default(Guid) ? Guid.NewGuid() : globalMessageId;
            await _rawRabbitClient.PublishAsync(message, globalMessageId, configuration);

            CreateEventAsync(message, globalMessageId);
        }

        public async Task CreateEventAsync<T>(T message, Guid globalMessageId, EventType type = EventType.Publish)
        {
            var eventTracker = new EventTracker
            {
                MessageId = globalMessageId == default(Guid) ? Guid.NewGuid() : globalMessageId,
                Name = message.GetType().ToString(),
                PayLoad = JsonConvert.SerializeObject(message),
                Type = type
            };

            _context.Add(eventTracker);

            await _context.SaveChangesAsync();
        }
    }
}
