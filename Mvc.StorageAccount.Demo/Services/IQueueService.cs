using Mvc.StorageAccount.Demo.Data;

namespace Mvc.StorageAccount.Demo.Services
{
    public interface IQueueService
    {
        Task SendMessage(EmailMessage emailMessage);
    }
}