using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Microservice.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microservice.Common.Services
{
    public class FirestoreService
    {
        private readonly IOrderService _orderService;
        private readonly IHostingEnvironment _env;
        private readonly ILogger _logger;

        public FirestoreService(IOrderService orderService, IHostingEnvironment env, ILogger<FirestoreService> logger)
        {
            _orderService = orderService;
            _env = env;
            _logger = logger;
        }

        public async Task AddDocumentAsync(string collectionPath, string documentPath, object data)
        {
            try
            {
                var _firestoreDb = InitiFirestore();
                var docRef = _firestoreDb.Collection(collectionPath).Document(documentPath);
                await docRef.SetAsync(data);
                _logger.LogInformation($"Successful save data: {collectionPath}\\{documentPath}");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateDocumentAsync(string collectionPath, string documentPath, Dictionary<string, object> data)
        {
            var _firestoreDb = InitiFirestore();
            var docRef = _firestoreDb.Collection(collectionPath).Document(documentPath);
            await docRef.UpdateAsync(data);
            _logger.LogInformation($"Successful save data: {collectionPath}\\{documentPath}");
        }

        public async Task ListenNewItem(string collectionName, string serviceName)
        {
            var _firestoreDb = InitiFirestore();
            var query = _firestoreDb.Collection(collectionName)
                                    .WhereEqualTo("Status", (int)OrderStatus.New)
                                    .WhereEqualTo(serviceName, false);

            var capitalQuerySnapshot = await query.GetSnapshotAsync();
            FirestoreChangeListener listener = query.Listen(async (snapshots) => await Process(snapshots, serviceName));
        }

        public async Task ListenItemUpdated(string collectionName, string serviceName)
        {
            var _firestoreDb = InitiFirestore();
            var query = _firestoreDb.Collection(collectionName).WhereGreaterThan("status", (int)OrderStatus.New);
            FirestoreChangeListener listener = query.Listen(async (snapshots) => await Process(snapshots, serviceName));
        }

        public async Task Process(QuerySnapshot snapshots, string serviceName)
        {
            _logger.LogInformation($"[{DateTime.Now}] Process - Start");
            foreach (var snapshot in snapshots)
            {
                if (!snapshot.Exists) continue;

                var dic = snapshot.ToDictionary();
                var order = dic.ConvertJson<Order>();
                var action = "add new";
                if (order.Status == OrderStatus.New)
                {
                    await _orderService.CreateAsync(order);
                    var updateData = new Dictionary<string, object>
                    {
                        { serviceName, true }
                    };
                    await UpdateDocumentAsync("order", snapshot.Id, updateData);
                }
                else
                {
                    action = "update status";
                    await _orderService.UpdateStatusAsync(new Guid(snapshot.Id), order.Status);
                }

                _logger.LogInformation($"[{DateTime.Now}] Process - documentId: {snapshot.Id}, action: {action} - {JsonConvert.SerializeObject(dic)}");
            }
            _logger.LogInformation($"[{DateTime.Now}] Process - Done");
        }

        private FirestoreDb InitiFirestore()
        {
            var fireStoreKeysFolder = Path.Combine(_env.ContentRootPath, "..", "Microservice.Common", $"firestore.{_env.EnvironmentName}.json");
            var cred = GoogleCredential.FromFile(fireStoreKeysFolder);
            var channel = new Channel(FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port, cred.ToChannelCredentials());
            var client = FirestoreClient.Create(channel);
            return FirestoreDb.Create("taskkap", client);
        }
    }
}
