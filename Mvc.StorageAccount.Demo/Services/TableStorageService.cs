using Azure.Data.Tables;
using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly TableClient _tableClient;

        public TableStorageService(IConfiguration configuration, TableClient tableClient)
        {
            _configuration = configuration;
            _tableClient = tableClient;
            _tableClient.CreateIfNotExists();
        }

        public async Task<AttendeeEntity> GetAttendee(string industry, string id)
        {
            return await _tableClient.GetEntityAsync<AttendeeEntity>(industry, id);
        }

        public async Task<List<AttendeeEntity>> GetAllAttendees()
        {
            var attendeeEntities = _tableClient.Query<AttendeeEntity>();
            return attendeeEntities.ToList();
        }

        public async Task UpsertAttendee(AttendeeEntity attendeeEntity)
        {
            await _tableClient.UpsertEntityAsync(attendeeEntity);
        }

        public async Task DeleteAttendee(string industry, string id)
        {
            await _tableClient.DeleteEntityAsync(industry, id);
        }

        [Obsolete]
        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["AzureStorage:ConnectionString"]);
            var tableClient = serviceClient.GetTableClient(_configuration["AzureStorage:TableName"]);

            await tableClient.CreateIfNotExistsAsync();

            return tableClient;
        }
    }
}
