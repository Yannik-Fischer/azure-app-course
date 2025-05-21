using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvc.StorageAccount.Demo.Data;
using Mvc.StorageAccount.Demo.Services;
using System.Threading.Tasks;

namespace Mvc.StorageAccount.Demo.Controllers
{
    public class AttendeeRegistrationController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;

        public AttendeeRegistrationController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
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

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
