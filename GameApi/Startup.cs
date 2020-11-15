using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TbspRgpLib.Jwt;
using TbspRgpLib.Settings;

using GameApi.Repositories;
using GameApi.Services;
using GameApi.Adapters;
using GameApi.Events;
using GameApi.Workers;

namespace GameApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IAdventureService, AdventureService>();
            services.AddScoped<IAdventureRepository, AdventureRepository>();
            services.AddScoped<IEventAdapter, EventAdapter>();
            services.AddScoped<IEventService, EventService>();

            services.Configure<DatabaseSettings>(Configuration.GetSection("Database"));
            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
                
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddSingleton<IJwtSettings>(sp =>
                sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            
            services.Configure<EventStoreSettings>(Configuration.GetSection("EventStore"));
            services.AddSingleton<IEventStoreSettings>(sp =>
                sp.GetRequiredService<IOptions<EventStoreSettings>>().Value);

            //start workers
            services.AddHostedService<NewGameWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<JwtMiddleware>();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
