using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Models.Exceptions;
using Harrison314.PassiveMQ.Services.Configuration;
using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation.MsSql
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string connectionString;
        private readonly ITimeAccessor timeAccessor;

        public MessageRepository(ITimeAccessor timeAccessor, IOptions<MsSqlSettings> options)
        {
            this.connectionString = options.Value.ConnectionString;
            this.timeAccessor = timeAccessor;
        }

        public async Task<Guid> CreateAsync(Guid queuId, MessageCrateReqDto message)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            return await this.CreateInternal(connection, queuId, message).ConfigureAwait(false);
        }

        private async Task<Guid> CreateInternal(SqlConnection connection, Guid queuId, MessageCrateReqDto message)
        {
            Guid id = Guid.NewGuid();
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO [dbo].[Message]
([Id], [QueuRecordId], [Created], [Label], [Content], [NextVisible], [RetryCount])
VALUES (@id, @queuRecordId, @created, @label, @content, NULL, 0)
";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@queuRecordId", queuId);
            command.Parameters.AddWithValue("@created", this.timeAccessor.UtcNow);
            command.Parameters.AddWithValue("@label", message.Label.ToSqlValue());
            command.Parameters.AddWithValue("@content", message.Content);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            return id;
        }

        public async Task<PublishResult> Publish(string topic, MessageCrateReqDto message)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            List<Guid> queuIds = new List<Guid>(25);
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT [Id] FROM [dbo].[QueuRecord] WHERE [TopicPattern] IS NOT NULL AND CHARINDEX([TopicPattern], @topic) = 1";
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@topic", topic);

                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    queuIds.Add((Guid)reader["Id"]);
                }
            }

            PublishResult result = new PublishResult();
            foreach (Guid queuId in queuIds)
            {
                Guid messageId = await this.CreateInternal(connection, queuId, message).ConfigureAwait(false);
                result.CratedMessages.Add(new PublishPair()
                {
                    MessageId = messageId,
                    QueuId = queuId
                });
            }

            return result;
        }

        public async Task<int> GetCountAsync(Guid queuId)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT([Id]) AS [Count] 
FROM [dbo].[Message] WITH (READPAST)
WHERE [QueuRecordId] = @queuId AND ([NextVisible] IS NULL OR [NextVisible] > @now)
";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@queuId", queuId);
            command.Parameters.AddWithValue("@now", this.timeAccessor.UtcNow);

            int count = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
            return count;
        }

        public async Task<Guid?> PeekMessageAsync(Guid queuId)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP(1) [Id]
FROM [dbo].[Message]
WHERE [QueuRecordId] = @queuId AND  [Created] = (SELECT MIN([Created]) FROM [dbo].[Message] WITH (READPAST) WHERE [QueuRecordId] = @queuId AND ([NextVisible] IS NULL OR [NextVisible] < @now))";

            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@queuId", queuId);
            command.Parameters.AddWithValue("@now", this.timeAccessor.UtcNow);

            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                return (Guid)reader["Id"];
            }

            return null;
        }

        public async Task<MessageDto> ReadById(Guid id)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP(1) [Id], [QueuRecordId], [Created], [Label], [Content], [NextVisible], [RetryCount]
FROM [dbo].[Message]
WHERE [Id] = @id
";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                MessageDto item = new MessageDto();
                item.Content = (string)reader["Content"];
                item.Id = (Guid)reader["Id"];
                item.InsertionTime = (DateTime)reader["Created"];
                item.Label = reader.GetString("Label");
                item.NextVisibleTime = reader.GetNullableDateTime("NextVisible");
                item.RetryCount = (int)reader["RetryCount"];

                return item;
            }

            throw new PassiveMQNotFoundException("Message", id);
        }

        public async Task RemoveAsync(Guid messageId)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM [dbo].[Message] WHERE [Id] = @id";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@id", messageId);

            int count = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (count == 0)
            {
                throw new PassiveMQNotFoundException("Message", messageId);
            }
        }

        public async Task<Guid?> ReserveMessageAsync(Guid queuId, TimeSpan nextVisibility)
        {
            DateTime nextVisibilityTime = this.timeAccessor.UtcNow + nextVisibility;
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE TOP(1) [dbo].[Message] WITH (UPDLOCK, ROWLOCK, READPAST)
SET [NextVisible] = @nextVisibilityTime,
    [RetryCount] = [RetryCount] + 1,
    @id = [Id]
WHERE [QueuRecordId] = @queuId AND ([NextVisible] IS NULL OR [NextVisible] < @now)
      AND [Created] = (SELECT MIN([Created]) FROM [dbo].[Message] WITH (UPDLOCK, ROWLOCK, READPAST) WHERE [QueuRecordId] = @queuId AND ([NextVisible] IS NULL OR [NextVisible] < @now))";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@nextVisibilityTime", nextVisibilityTime);
            command.Parameters.AddWithValue("@queuId", queuId);
            command.Parameters.AddWithValue("@now", this.timeAccessor.UtcNow);
            SqlParameter idParameter = command.Parameters.Add("@id", System.Data.SqlDbType.UniqueIdentifier);
            idParameter.Direction = System.Data.ParameterDirection.Output;
            idParameter.Value = DBNull.Value;

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            if (idParameter.Value == DBNull.Value)
            {
                return null;
            }
            else
            {
                return (Guid)idParameter.Value;
            }
        }
    }
}
