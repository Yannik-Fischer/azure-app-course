using Blazor.AzureCosmosDb.Demo.Data;

namespace Blazor.AzureCosmosDb.Demo.Service
{
    public interface IEngineerService
    {
        Task DeleteEngineer(string? id);
        Task<List<Engineer>> GetAllEngineerDetails();
        Task<Engineer> GetEngineerDetailsById(string? id);
        Task UpsertEngineer(Engineer engineer);
    }
}