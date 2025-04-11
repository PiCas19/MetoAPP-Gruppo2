using FirebaseAdmin;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackendProject;

namespace BackendProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;

        public NotificationSettingsController(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserNotificationSettings[] settings)
        {
            if (settings == null)
                return BadRequest("Invalid payload.");

            // Salva le impostazioni in Firestore
            foreach (var setting in settings)
            {
                var document = _firestoreDb.Collection("settings").Document();  // Crea un nuovo documento nella collezione 'settings'
                await document.SetAsync(setting);  // Salva l'oggetto nel documento
            }

            return Ok(new { message = "Settings saved to Firestore." });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Ottieni tutte le impostazioni dalla collezione 'settings'
            var settingsSnapshot = await _firestoreDb.Collection("settings").GetSnapshotAsync();
            var settings = new List<UserNotificationSettings>();

            foreach (var document in settingsSnapshot.Documents)
            {
                var setting = document.ConvertTo<UserNotificationSettings>();
                settings.Add(setting);
            }

            return Ok(settings);
        }
    }
}
