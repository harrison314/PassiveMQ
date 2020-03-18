using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harrison314.PassiveMQ.Services.Configuration;
using Harrison314.PassiveMQ.Services.Contracts;
using Harrison314.PassiveMQ.Services.Implementation.MsSql;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection PassiveMQMsSQL(this IServiceCollection services, Action<MsSqlSettings> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            services.Configure(options);
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IQueuRepository, QueuRepository>();
            services.AddTransient<INotificationSender, SqlWebHookNotificationSender>();

            return services;
        }

        public static IServiceCollection PassiveMQMsSQL(this IServiceCollection services, string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            return services.PassiveMQMsSQL(cfg => cfg.ConnectionString = connectionString);
        }
    }
}
