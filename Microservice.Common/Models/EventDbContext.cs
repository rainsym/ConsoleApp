using Microservice.Common.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Common.Models
{
    public class EventDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContext;

        public EventDbContext(DbContextOptions<EventDbContext> options, IHttpContextAccessor httpContext) : base(options)
        {
            _httpContext = httpContext;
        }

        public EventDbContext()
        {
        }

        public DbSet<EventTracker> EventTrackers { get; set; }
        public DbSet<RawRabbitEvent> RawRabbitEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Skip shadow types
                if (entityType.ClrType == null)
                {
                    continue;
                }

                entityType.Relational().TableName = entityType.ClrType.Name;
            }

            base.OnModelCreating(modelBuilder);
        }

        public static void UpdateDatabase(IApplicationBuilder app)
        {
            using(var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();

                context.Database.Migrate();

                Seed(context);
            }
        }

        private static void Seed(EventDbContext context)
        {
            //seed code
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateTimeTracker();

            return base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            UpdateTimeTracker();

            return base.SaveChanges();
        }

        /// <summary>
        /// Auto record create and modify time
        /// </summary>
        private void UpdateTimeTracker()
        {
            var currentTime = DateTime.Now;
            int? accountId = null; ;
            if (_httpContext.HttpContext != null && _httpContext.HttpContext.User != null)
            {
                accountId = _httpContext.HttpContext.User.GetAccountId();
            }

            //Find all Entities that are Added/Modified that inherit from my EntityBase
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Metadata.FindProperty("CreatedAt") != null)
                    {
                        if (entry.Property("CreatedAt").CurrentValue == null)
                            entry.Property("CreatedAt").CurrentValue = currentTime;
                    }

                    if (entry.Metadata.FindProperty("CreatedBy") != null)
                    {
                        if (entry.Property("CreatedBy").CurrentValue == null)
                            entry.Property("CreatedBy").CurrentValue = accountId;
                    }
                }

                if (entry.Metadata.FindProperty("ModifiedAt") != null)
                {
                    if (entry.Property("ModifiedAt").CurrentValue == null)
                        entry.Property("ModifiedAt").CurrentValue = currentTime;
                }

                if (entry.Metadata.FindProperty("ModifiedBy") != null)
                {
                    if (entry.Property("ModifiedBy").CurrentValue == null)
                        entry.Property("ModifiedBy").CurrentValue = accountId;
                }
            }
        }
    }
}
