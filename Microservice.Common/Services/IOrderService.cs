using Microservice.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Common.Services
{
    public interface IOrderService
    {
        Task<Order> GetAsync(Guid guidId);
        Task<Order> CreateAsync(Order model);
        Task<Order> UpdateStatusAsync(Guid guidId, OrderStatus status);
        Task DeleteAsync(int id);
    }
}
