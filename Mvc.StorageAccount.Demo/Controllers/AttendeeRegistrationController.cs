using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvc.StorageAccount.Demo.Data;
using Mvc.StorageAccount.Demo.Services;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mvc.StorageAccount.Demo.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueService _queueService;

        public AttendeeRegistrationController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService, IQueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _queueService = queueService;
        }

        // GET: AttendeeRegistrationController
        public async Task<ActionResult> Index()
        {
            var attendees = await _tableStorageService.GetAllAttendees();

            foreach (var attendee in attendees)
            {
                attendee.ImageName = await _blobStorageService.GetBlobUrl(attendee.ImageName);
            }

            return View(attendees);
        }

        // GET: AttendeeRegistrationController/Details/5
        public async Task<ActionResult> Details(string industry, string id)
        {
            var attendee = await _tableStorageService.GetAttendee(industry, id);
            attendee.ImageName = await _blobStorageService.GetBlobUrl(attendee.ImageName);

            return View(attendee);
        }

        // GET: AttendeeRegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeRegistrationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid().ToString();

                attendeeEntity.PartitionKey = attendeeEntity.Industry;
                attendeeEntity.RowKey = id;

                if (formFile.Length > 0)
                {
                    attendeeEntity.ImageName = await _blobStorageService.UploadBlob(formFile, id);
                }
                else
                {
                    attendeeEntity.ImageName = "default.jpg";
                }

                await _tableStorageService.UpsertAttendee(attendeeEntity);

                var emailMessage = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.Now,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName},\n\r Thank you for registering for this event.\n\r Your record has been saved for future reference."
                };

                await _queueService.SendMessage(emailMessage);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeRegistrationController/Edit/5
        public async Task<ActionResult> Edit(string industry, string id)
        {
            var attendee = await _tableStorageService.GetAttendee(industry, id);
            return View(attendee);
        }

        // POST: AttendeeRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AttendeeEntity attendeeEntity, IFormFile formFile)
        {
            try
            {
                if (formFile?.Length > 0)
                {
                    attendeeEntity.ImageName = await _blobStorageService.UploadBlob(formFile, attendeeEntity.RowKey, attendeeEntity.ImageName);
                }

                attendeeEntity.PartitionKey = attendeeEntity.Industry;

                await _tableStorageService.UpsertAttendee(attendeeEntity);

                var email = new EmailMessage
                {
                    EmailAddress = attendeeEntity.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendeeEntity.FirstName} {attendeeEntity.LastName},\n\r Your record was modified successfully"
                };

                await _queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: AttendeeRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, string industry)
        {
            try
            {
                var attendee = await _tableStorageService.GetAttendee(industry, id);

                await _tableStorageService.DeleteAttendee(industry, id);
                await _blobStorageService.DeleteBlob(attendee.ImageName);

                var email = new EmailMessage
                {
                    EmailAddress = attendee.EmailAddress,
                    TimeStamp = DateTime.UtcNow,
                    Message = $"Hello {attendee.FirstName} {attendee.LastName},\n\r Your record was removed successfully"
                };

                await _queueService.SendMessage(email);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
