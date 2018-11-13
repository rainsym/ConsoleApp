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

        Task CreateEventAsync<T>(T message, Guid globalMessageId, EventType type = EventType.Publish);
    }
}
