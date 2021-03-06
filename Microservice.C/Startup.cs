﻿using Microservice.C.EventHandlers;
using Microservice.Common;
using Microservice.Common.EventHandlers;
using Microservice.Common.Models;
using Microservice.Common.Models.Events;
using Microservice.Common.RawRabbit;
using Microservice.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using RawRabbit.Extensions.Client;

namespace Microservice.C
{
    public class Startup
    {
        private string _contentRootPath;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EventDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("EventDbConnection")));

            //config raw rabbit
            services.AddRawRabbit(cfg => cfg.SetBasePath(_contentRootPath).AddJsonFile("rabbitmq.json"));

            // Try Add IHttpContextAccessor for get http context in db context
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IRawRabbitWrapper, RawRabbitWrapper>();
            services.AddScoped<TestHandler, TestHandler>();
            services.AddScoped<FirestoreHandler, FirestoreHandler>();
            services.AddScoped<FirestoreService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.LogDeployDate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            EventDbContext.UpdateDatabase(app);

            app.UseMvc();

            app.AddMessageHandler<TestEvent, TestHandler>();

            var _firestoreDb = CommonFunctions.InitiFirestore(env);
            var query = _firestoreDb.Collection(ConstantServices.OrderCollection)
                                    //.WhereEqualTo("Status", (int)OrderStatus.New)
                                    .WhereEqualTo(ConstantServices.ServiceC, false);
            var eve = new ListenNewOrder { ServeName = ConstantServices.ServiceC, CollectionName = ConstantServices.OrderCollection };
            app.AddFirestoreHandler<ListenNewOrder, FirestoreHandler>(eve, query);

            app.CheckEventRegistered(typeof(Startup).Namespace);
        }
    }
}
