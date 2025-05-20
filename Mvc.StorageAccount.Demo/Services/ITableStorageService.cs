using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public interface ITableStorageService
    {
        Task DeleteAttendee(string industry, string id);
        Task<List<AttendeeEntity>> GetAllAttendees();
        Task<AttendeeEntity> GetAttendee(string industry, string id);
        Task UpsertAttendee(AttendeeEntity attendeeEntity);
    }
}