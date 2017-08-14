using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BackEnd.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using InfluxDB.Collector;
using Microsoft.AspNetCore.Http;

namespace BackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration Required from the environment:
            // * Authentication:Tenant
            // * Authentication:ClientId
            // * ConnectionStrings:DefaultConnectionString

            var influxAddress = Configuration["Metrics:InfluxDb:Url"];
            var influxDb = Configuration["Metrics:InfluxDb:Database"];
            if (!string.IsNullOrEmpty(influxAddress) && !string.IsNullOrEmpty(influxDb))
            {
                Console.WriteLine($"Using influxdb metrics collection: {influxDb}");
                var collector = new CollectorConfiguration()
                    .Tag.With("host", Environment.GetEnvironmentVariable("HOSTNAME"))
                    .Tag.With("conferenceplanner.app", "backend")
                    .Batch.AtInterval(TimeSpan.FromSeconds(2))
                    .WriteTo.InfluxDB(influxAddress, influxDb)
                    .CreateCollector();

                services.AddSingleton(collector);
            }
            else
            {
                var collector = new CollectorConfiguration()
                    .CreateCollector();
                services.AddSingleton(collector);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["Authentication:Tenant"]}/{Configuration["Authentication:PolicyId"]}/v2.0/";
                    options.Audience = Configuration["Authentication:ClientId"];
                });

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("http://schemas.microsoft.com/identity/claims/objectidentifier")
                    .Build();
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Conference Planner API", Version = "v1" });
                options.CustomSchemaIds(t => t.FullName);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TelemetryConfiguration telemetryConfiguration, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices);
            telemetryConfiguration.TelemetryInitializers.Add(new ServiceNameTelemetryInitializer("Backend"));

            app.Use((context, next) =>
            {
                var collector = context.RequestServices.GetService<MetricsCollector>();
                if (collector != null)
                {
                    CollectMetrics(context, collector);
                }

                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Planner API v1")
            );

            app.UseAuthentication();

            app.UseMvc();

            app.Run(context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });
        }

        private void CollectMetrics(HttpContext context, MetricsCollector collector)
        {
            collector.Increment("requests", tags: new Dictionary<string, string>()
            {
                {"path", context.Request.Path},
                {"client_ip", context.Connection.RemoteIpAddress.ToString() },
                {"user_agent", context.Request.Headers["User-Agent"].ToString() }
            });
        }
    }
}
