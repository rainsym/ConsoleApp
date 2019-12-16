using Microservice.Common.Exceptions;
using Microservice.Common.Models;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RawRabbit;
using RawRabbit.Configuration.Subscribe;
using System;

namespace Microservice.Common.RawRabbit
{
    public static class RawRabbitExtensions
    {
        public static void AddMessageHandler<TMessage, TMessageHandler>(this IApplicationBuilder app, Action<ISubscriptionConfigurationBuilder> configuration = null)
                where TMessage : IMessage
                where TMessageHandler : IMessageHandle<TMessage>
        {
            var client = GetClient(app);
            var _logger = app.GetLogging<IMessage>();
            //get namespace of service
            var subscriber = typeof(TMessageHandler).Assembly.FullName.Split(',')[0];

            client.SubscribeAsync<TMessage>(async (e, context) =>
            {
                var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    try
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<TMessageHandler>();
                        if (_logger != null)
                        {
                            _logger.LogInformation($"Subscribe on Event {e.GetType()} {JsonConvert.SerializeObject(e)}");
                        }
                        await handler.HandleAsync(e, context);

                        //insert one row into EventTracker when finished
                        var rawRabbitWrapper = scope.ServiceProvider.GetRequiredService<IRawRabbitWrapper>();
                        await rawRabbitWrapper.SubscribedAsync(e, context.GlobalRequestId, subscriber, EventType.Subscribe);
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(CustomException)) return;

                        if (_logger != null)
                        {
                            _logger.LogError($"ERROR {ex.Message} - Subscribe on Event {e.GetType()} {JsonConvert.SerializeObject(e)}\r\n{ex.StackTrace}");
                        }
                    }
                }
            }, configuration);

            //subscribe event
            using(var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var rawRabbitWrapper = scope.ServiceProvider.GetRequiredService<IRawRabbitWrapper>();
                var name = typeof(TMessage).ToString();
                rawRabbitWrapper.RegisterEventAsync(name, subscriber).Wait();
            }
        }

        private static IBusClient GetClient(IApplicationBuilder app)
        {
            var client = app.ApplicationServices.GetService<IBusClient>();
            if (client == null)
            {
                throw new InvalidOperationException("RawRabbit.IBusClient cannot be found in service collection");
            }

            return client;
        }
    }
}
