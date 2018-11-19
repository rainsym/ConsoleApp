using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RawRabbit;
using RawRabbit.Configuration.Publish;

namespace Microservice.Common.Services
{
    public class RawRabbitWrapper : IRawRabbitWrapper
    {
        private readonly EventDbContext _context;
        private readonly IBusClient _rawRabbitClient;
        private readonly ILogger _logger;

        public RawRabbitWrapper(EventDbContext context, IBusClient rawRabbitClient, ILogger<RawRabbitWrapper> logger)
        {
            _context = context;
            _rawRabbitClient = rawRabbitClient;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message = default(T), Guid globalMessageId = default(Guid), Action<IPublishConfigurationBuilder> configuration = null)
        {
            globalMessageId = globalMessageId == default(Guid) ? Guid.NewGuid() : globalMessageId;
            await _rawRabbitClient.PublishAsync(message, globalMessageId, configuration);

            await SubscribedAsync(message, globalMessageId);
        }

        public async Task RegisterEventAsync(string name, string subscriber)
        {
            var rawRabbitEvent = new RawRabbitEvent
            {
                Name = name,
                Subscriber = subscriber
            };

            _context.Add(rawRabbitEvent);
            await _context.SaveChangesAsync();
        }

        public async Task SubscribedAsync<T>(T message, Guid globalMessageId, string subscriber = null, EventType type = EventType.Publish)
        {
            var payload = JsonConvert.SerializeObject(message);
            try
            {
                var eventTracker = new EventTracker
                {
                    MessageId = globalMessageId == default(Guid) ? Guid.NewGuid() : globalMessageId,
                    Name = message.GetType().ToString(),
                    PayLoad = payload,
                    Type = type,
                    Subscriber = subscriber
                };

                _context.Add(eventTracker);

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"SubscribedAsync ERROR {JsonConvert.SerializeObject(payload)}");
            }
        }

        public async Task<List<UnsubscribeEvent>> GetUnsubscribeEvents()
        {
            var query = _context.UnsubscribeEvents.FromSqlObject();
            return await query.ToListAsync();
        }
    }
}
