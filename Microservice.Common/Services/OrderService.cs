using Microservice.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microservice.Common.Services
{
    public class OrderService : IOrderService
    {
        private EventDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public OrderService(EventDbContext context, IHostingEnvironment env, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
            _httpContext = httpContext;
        }

        public async Task<Order> GetAsync(Guid guidId)
        {
            return await _context.Orders.FirstOrDefaultAsync(t => t.GuidId == guidId);
        }

        public async Task<Order> CreateAsync(Order model)
        {
            _context.Orders.Add(model);
            await _context.SaveChangesAsync();

            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

        }

        public async Task<Order> UpdateStatusAsync(Guid guidId, OrderStatus status)
        {
            var order = await GetAsync(guidId);
            if (order == null) return null;

            order.Status = status;

            await _context.SaveChangesAsync();

            return order;
        }

        private bool CheckDbIsDisposed()
        {
            if (!_context.IsDisposed) return false;

            var optionsBuilder = new DbContextOptionsBuilder<EventDbContext>();
            var appsettingPath = Path.Combine(_env.ContentRootPath, $"appsettings.{_env.EnvironmentName}.json");
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("EventDbConnection"));
            _context = new EventDbContext(optionsBuilder.Options, _httpContext);

            return true;
        }
    }
}
