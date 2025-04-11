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
            if (settings == null || settings.Length == 0)
                return BadRequest("Payload non valido.");

            var settingsRef = _firestoreDb.Collection("settings");

            foreach (var setting in settings)
            {
                var query = settingsRef
                    .WhereEqualTo("Token", setting.Token)
                    .WhereEqualTo("Location", setting.Location);
                var snapshot = await query.GetSnapshotAsync();

                if (snapshot.Count > 0)
                {
                    foreach (var doc in snapshot.Documents)
                    {
                        var docRef = settingsRef.Document(doc.Id);
                        var updates = new Dictionary<string, object>
                        {
                            { "TemperatureMax", setting.TemperatureMax },
                            { "TemperatureMin", setting.TemperatureMin },
                            { "IsEnabled", setting.IsEnabled }
                        };
                        await docRef.UpdateAsync(updates);
                    }
                }
                else
                {
                    // Crea un nuovo documento
                    var newDoc = settingsRef.Document();
                    await newDoc.SetAsync(setting);
                }
            }

            return Ok(new { message = "Impostazioni salvate o aggiornate con successo." });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string token, [FromQuery] string location)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(location))
                return BadRequest("Token e location sono obbligatori.");

            var settingsRef = _firestoreDb.Collection("settings");
            var query = settingsRef
                .WhereEqualTo("Token", token)
                .WhereEqualTo("Location", location);
            var snapshot = await query.GetSnapshotAsync();

            var results = new List<UserNotificationSettings>();
            foreach (var doc in snapshot.Documents)
            {
                var setting = doc.ConvertTo<UserNotificationSettings>();
                results.Add(setting);
            }

            return Ok(results);
        }

    }
}
