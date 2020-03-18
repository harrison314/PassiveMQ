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
    public class QueuRepository : IQueuRepository
    {
        private readonly string connectionString;
        private readonly ITimeAccessor timeAccessor;

        public QueuRepository(ITimeAccessor timeAccessor, IOptions<MsSqlSettings> options)
        {
            this.connectionString = options.Value.ConnectionString;
            this.timeAccessor = timeAccessor;
        }

        public async Task<Guid> CreateAsync(QueuCreateReqDto queuDto)
        {
            if (queuDto == null) throw new ArgumentNullException(nameof(queuDto));

            Guid id = Guid.NewGuid();

            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO [dbo].[QueuRecord] ([Id], [Name], [TopicPattern],[Created]) VALUES (@id, @name, @topicPattern, @created)";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@created", this.timeAccessor.UtcNow);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", queuDto.Name);
            command.Parameters.AddWithValue("@topicPattern", queuDto.TopicPattern ?? DBNull.Value as object);

            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            return id;
        }

        public async Task DeleteAsync(Guid id)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM [dbo].[QueuRecord] WHERE [Id] = @id";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@id", id);

            int count = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            if (count == 0)
            {
                throw new PassiveMQNotFoundException("Queu", id);
            }
        }

        public async Task<List<QueuDto>> GetAllAsync()
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT [Id], [Name], [TopicPattern], [NotificationAdress], [Created] FROM [dbo].[QueuRecord]";
            command.CommandType = System.Data.CommandType.Text;

            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            List<QueuDto> result = new List<QueuDto>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                result.Add(this.MapDto(reader));
            }

            return result;
        }

        public async Task<QueuDto> GetByIdAsync(Guid id)
        {
            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP(1) [Id], [Name], [TopicPattern], [NotificationAdress], [Created] FROM [dbo].[QueuRecord] WHERE [Id] = @id";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@id", id);

            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                QueuDto dto = this.MapDto(reader);

                return dto;
            }

            throw new PassiveMQNotFoundException("Queu", id);
        }


        public async Task<QueuDto> GetByNameOrDefaultAsync(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            using SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP(1) [Id], [Name], [TopicPattern], [NotificationAdress], [Created] FROM [dbo].[QueuRecord] WHERE [Name] = @name";
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@name", name);

            using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                QueuDto dto = this.MapDto(reader);

                return dto;
            }

            return null;
        }

        private QueuDto MapDto(SqlDataReader reader)
        {
            QueuDto dto = new QueuDto();
            dto.Created = (DateTime)reader["Created"];
            dto.Id = (Guid)reader["Id"];
            dto.Name = (string)reader["Name"];
            dto.NotificationAdress = reader.GetString("NotificationAdress");
            dto.TopicPattern = reader.GetString("TopicPattern");

            return dto;
        }
    }
}
