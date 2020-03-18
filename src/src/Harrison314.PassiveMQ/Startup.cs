using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harrison314.PassiveMQ.Infrastructure;
using Harrison314.PassiveMQ.Services.Configuration;
using Harrison314.PassiveMQ.Services.Contracts;
using Harrison314.PassiveMQ.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Harrison314.PassiveMQ
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MqSettings>(cfg =>
            {
                cfg.DefaultRetry = TimeSpan.FromMinutes(5.0);
                cfg.MaxDefaultRetryCount = 5;
            });


            services.AddSingleton<ITimeAccessor, TimeAccessor>();
            services.AddMemoryCache();

            string sqlConnectionString = this.Configuration.GetConnectionString("MsSqlDatabase");
            if(!string.IsNullOrEmpty(sqlConnectionString))
            {
                services.PassiveMQMsSQL(sqlConnectionString);
            }

            services.AddControllers(options=>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ValidateModelStateFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() 
                { 
                    Title = "PassiveMQ API", 
                    Version = "v1" 
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger(c=>c.SerializeAsV2 = true);

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassiveMQ API API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
