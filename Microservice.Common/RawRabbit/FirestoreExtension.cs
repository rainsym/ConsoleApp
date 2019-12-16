using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Microservice.Common.RawRabbit
{
    public static class FirestoreExtension
    {
        public static void AddFirestoreHandler<TMessage, IListenHandler>(this IApplicationBuilder app, TMessage e, Query query)
                where TMessage : IMessage
                where IListenHandler : IListenHandler<TMessage>
        {
            var logger = app.GetLogging<TMessage>();
            var listener = query.Listen(async (snapshots) =>
            {
                logger.LogInformation($"[{DateTime.Now}] Listen start- {typeof(TMessage).Name}");
                var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetRequiredService<IListenHandler>();
                    await handler.HandleAsync(e, snapshots);
                }
                logger.LogInformation($"[{DateTime.Now}] Listen done- {typeof(TMessage).Name}");
            });
        }
    }
}
