using Common.Web.Middleware;
using ComponentRegistrar;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scheduler.Clients;
using Scheduler.Configurations;
using Scheduler.Jobs;
using Scheduler.Services;
using Serilog;

namespace Scheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHangfire(c => c.UseMemoryStorage());
            
            services.Configure<IntegrationClientConfig>(Configuration.GetSection("IntegrationClientConfiguration"));
            services.Configure<JobsConfig>(Configuration.GetSection("JobsConfig"));

            services.AddHttpClient<IIntegrationClient, IntegrationClient>();
            
            services.AddSingleton<IWorkloadService, WorkloadService>();

            services.RegisterCommonServices();

            services.AddHostedService<JobStartHostedService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddSerilog();

            app.UseHangfireServer();

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
