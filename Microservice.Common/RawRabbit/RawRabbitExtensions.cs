using Microservice.Common.Exceptions;
using Microservice.Common.Models;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration.Publish;
using RawRabbit.Configuration.Request;
using RawRabbit.Configuration.Respond;
using RawRabbit.Configuration.Subscribe;
using RawRabbit.Context;
using System;
using System.Threading.Tasks;

namespace Microservice.Common.RawRabbit
{
    public static class RawRabbitExtensions
    {
        public static void AddMessageHandler<TMessage, TMessageHandler>(this IApplicationBuilder app, Action<ISubscriptionConfigurationBuilder> configuration = null)
                where TMessage : IMessage
                where TMessageHandler : IMessageHandle<TMessage>
        {
            var client = GetClient(app);
            var _logger = GetLogging<IMessage>(app);
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
                        await rawRabbitWrapper.CreateEventAsync(e, context.GlobalRequestId, EventType.Subscribe);
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

        private static ILogger<TResponse> GetLogging<TResponse>(IApplicationBuilder app)
        {
            //if you want to write log the response, please add logging into your service in the starup.cs (services.AddLogging())
            var loggingFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            ILogger<TResponse> logger = null;
            logger = loggingFactory != null ? loggingFactory.CreateLogger<TResponse>() : null;

            return logger;
        }
    }
}
