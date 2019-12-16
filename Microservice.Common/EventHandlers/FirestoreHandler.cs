using Google.Cloud.Firestore;
using Microservice.Common;
using Microservice.Common.Models;
using Microservice.Common.Models.Events;
using Microservice.Common.RawRabbit;
using Microservice.Common.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microservice.Common.EventHandlers
{
    public class FirestoreHandler : IListenHandler<ListenNewOrder>
    {
        private readonly IOrderService _orderService;
        private readonly FirestoreService _firestoreService;
        private readonly ILogger _logger;

        public FirestoreHandler(IOrderService orderService, FirestoreService firestoreService, ILogger<FirestoreHandler> logger)
        {
            _orderService = orderService;
            _firestoreService = firestoreService;
            _logger = logger;
        }

        public async Task HandleAsync(ListenNewOrder eve, QuerySnapshot e)
        {
            if (e == null || e.Count == 0) return;
            foreach (var snapshot in e)
            {
                if (!snapshot.Exists) continue;

                var dic = snapshot.ToDictionary();
                var order = dic.ConvertJson<Order>();
                order.GuidId = new Guid(snapshot.Id);
                var action = "add new";
                var existing = await _orderService.GetAsync(order.GuidId);
                if (existing == null)
                {
                    await _orderService.CreateAsync(order);
                }
                else
                {
                    action = "update status";
                    await _orderService.UpdateStatusAsync(order.GuidId, order.Status);
                }

                var updateData = new Dictionary<string, object>
                {
                    { eve.ServeName, true }
                };
                await _firestoreService.UpdateDocumentAsync(eve.CollectionName, snapshot.Id, updateData);

                _logger.LogInformation($"[{DateTime.Now}] Process - documentId: {snapshot.Id}, action: {action} - {JsonConvert.SerializeObject(dic)}");
            }
        }
    }
}
