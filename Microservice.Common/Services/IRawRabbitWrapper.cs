using Microservice.Common.Models;
using RawRabbit.Configuration.Publish;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Common.Services
{
    public interface IRawRabbitWrapper
    {
        Task PublishAsync<T>(T message = default(T), Guid globalMessageId = default(Guid), Action<IPublishConfigurationBuilder> configuration = null);

        Task SubscribedAsync<T>(T message, Guid globalMessageId, string subscriber = null, EventType type = EventType.Publish);

        Task RegisterEventAsync(string name, string subscriber);

        Task<List<UnsubscribeEvent>> GetUnsubscribeEvents();
    }
}
