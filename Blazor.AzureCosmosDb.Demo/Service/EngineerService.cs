using Blazor.AzureCosmosDb.Demo.Data;
using Microsoft.Azure.Cosmos;

namespace Blazor.AzureCosmosDb.Demo.Service
{
    public class EngineerService : IEngineerService
    {
        private readonly string? _cosmosDbConnectionString;
        private const string _cosmosDbName = "Contractors";
        private const string _cosmosDbContainerName = "Engineers";

        public EngineerService(IConfiguration configuration)
        {
            _cosmosDbConnectionString = configuration.GetConnectionString("CosmosDbConnectionString");
        }

        private Container GetContainerClient()
        {
            var cosmosDbClient = new CosmosClient(_cosmosDbConnectionString);
            return cosmosDbClient.GetContainer(_cosmosDbName, _cosmosDbContainerName);
        }

        public async Task UpsertEngineer(Engineer engineer)
        {
            try
            {
                if (engineer.id == null)
                {
                    engineer.id = Guid.NewGuid();
                }

                var container = GetContainerClient();
                var response = await container.UpsertItemAsync(engineer, new PartitionKey(engineer.id.ToString()));
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }

        public async Task DeleteEngineer(string? id)
        {
            try
            {
                var container = GetContainerClient();
                var response = await container.DeleteItemAsync<Engineer>(id, new PartitionKey(id));
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }

        public async Task<List<Engineer>> GetAllEngineerDetails()
        {
            List<Engineer> engineers = new List<Engineer>();

            try
            {
                var container = GetContainerClient();
                var sqlQuery = "SELECT * FROM c";
                var queryDefinition = new QueryDefinition(sqlQuery);
                var queryResultIterator = container.GetItemQueryIterator<Engineer>(queryDefinition);

                while (queryResultIterator.HasMoreResults)
                {
                    var currentResultSet = await queryResultIterator.ReadNextAsync();

                    foreach (var engineer in currentResultSet)
                    {
                        engineers.Add(engineer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }

            return engineers;
        }

        public async Task<Engineer> GetEngineerDetailsById(string? id)
        {
            try
            {
                var container = GetContainerClient();
                var response = await container.ReadItemAsync<Engineer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception", ex);
            }
        }
    }
}