using Harrison314.PassiveMQ.Services.Configuration;
using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation.MsSql
{
    public class SqlWebHookNotificationSender: WebHookNotificationSender
    {
        private readonly string connectionString;

        public SqlWebHookNotificationSender(ITimeAccessor timeAccessor, IMemoryCache memoryCache, ILoggerFactory loggerFactory, IOptions<MsSqlSettings> options)
            : base(timeAccessor, memoryCache, loggerFactory)
        {
            this.connectionString = options.Value.ConnectionString;
        }

        protected override async Task<string> ReadAdress(Guid queuId)
        {
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT [NotificationAdress] FROM [dbo].[QueuRecord] WHERE [Id] = @id";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@id", queuId);

                        object adress = await command.ExecuteScalarAsync().ConfigureAwait(false);
                        if (adress == DBNull.Value)
                        {
                            return null;
                        }
                        else
                        {
                            return (string)adress;
                        }
                    }
                }
            }
        }

        protected override async Task<bool> SetNotificationAdressInternal(Guid queuId, string adress)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE [dbo].[QueuRecord] SET [NotificationAdress] = @adress WHERE [Id] = @id";
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@id", queuId);
                    command.Parameters.AddWithValue("@adress", adress != null ? (object)adress : DBNull.Value);

                    count = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                }
            }

            return count > 0;
        }
    }
}
