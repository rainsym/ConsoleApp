using Microservice.Common.Models;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microservice.A.Controllers
{
    [Route("api/microservice-a")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly FirestoreService _firestoreService;
        private readonly IRawRabbitWrapper _rawRabbitClient;
        private readonly ILogger _logger;

        public ValuesController(IOrderService orderService, FirestoreService firestoreService, IRawRabbitWrapper rawRabbitClient, ILogger<ValuesController> logger)
        {
            _orderService = orderService;
            _firestoreService = firestoreService;
            _rawRabbitClient = rawRabbitClient;
            _logger = logger;
        }

        // GET api/values/5
        [HttpGet("create")]
        public async Task Get()
        {
            //await _rawRabbitClient.PublishAsync(new TestEvent { Id = 1, Name = "TestEvent" });
            var order = new Order { Status = OrderStatus.New, GuidId = Guid.NewGuid() };
            order = await _orderService.CreateAsync(order);
            var data = new Dictionary<string, object>
            {
                { "Status", order.Status },
                { ConstantServices.ServiceA, true },
                { ConstantServices.ServiceB, false },
                { ConstantServices.ServiceC, false }
            };
            await _firestoreService.AddDocumentAsync("order", order.GuidId.ToString(), data);
        }

        [HttpGet("update/{id}/{status}")]
        public async Task Update(Guid id, OrderStatus status)
        {
            var order = await _orderService.UpdateStatusAsync(id, status);
            var data = new Dictionary<string, object>
            {
                { "Status", order.Status },
                { ConstantServices.ServiceA, true },
                { ConstantServices.ServiceB, false },
                { ConstantServices.ServiceC, false }
            };
            await _firestoreService.AddDocumentAsync("order", order.GuidId.ToString(), data);
        }
    }
}
