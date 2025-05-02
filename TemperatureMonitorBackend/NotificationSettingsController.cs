using FirebaseAdmin;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackendProject;

namespace BackendProject.Controllers
{

    public class TokenUpdateRequest
    {
        public string OldToken { get; set; }
        public string NewToken { get; set; }
    }

    /// <summary>
    /// Controller per la gestione delle impostazioni di notifica degli utenti.
    /// Espone endpoint RESTful per il salvataggio e il recupero delle soglie personalizzate.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationSettingsController : ControllerBase
    {
        private readonly FirestoreDb _firestoreDb;

        /// <summary>
        /// Costruttore che riceve l'istanza Firestore tramite dependency injection.
        /// </summary>
        /// <param name="firestoreDb">Istanza di FirestoreDb</param>
        public NotificationSettingsController(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        /// <summary>
        /// Endpoint POST per creare o aggiornare le impostazioni di notifica utente.
        /// Se esiste già un documento con la stessa coppia Token + Location, viene aggiornato.
        /// Altrimenti viene creato un nuovo documento.
        /// </summary>
        /// <param name="settings">Array di impostazioni di notifica da salvare</param>
        /// <returns>Risultato dell'operazione</returns>
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
                    var newDoc = settingsRef.Document();
                    await newDoc.SetAsync(setting);
                }
            }

            return Ok(new { message = "Impostazioni salvate o aggiornate con successo." });
        }

        /// <summary>
        /// Endpoint GET per recuperare le impostazioni utente in base a Token e Località.
        /// </summary>
        /// <param name="token">Token FCM del dispositivo</param>
        /// <param name="location">Località associata alle impostazioni</param>
        /// <returns>Lista di impostazioni trovate</returns>

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

        /// <summary>
        /// Endpoint POST per aggiornare il token FCM.
        /// </summary>
        /// <param name="requets">DTO di richiesta per l'aggiornamento del token FCM</param>
        /// <returns>Messaggio di conferm dell'aggiornamento del token</returns>

        [HttpPost("update-token")]
        public async Task<IActionResult> UpdateToken([FromBody] TokenUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.OldToken) || string.IsNullOrEmpty(request.NewToken))
                return BadRequest("Token non valido.");

            var settingsRef = _firestoreDb.Collection("settings");
            var query = settingsRef.WhereEqualTo("Token", request.OldToken);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
                return NotFound("Nessun documento associato al vecchio token.");

            foreach (var doc in snapshot.Documents)
            {
                var docRef = settingsRef.Document(doc.Id);
                var updates = new Dictionary<string, object>
                {
                    { "Token", request.NewToken }
                };
                await docRef.UpdateAsync(updates);
            }

            return Ok(new { message = "Token aggiornato con successo." });
        }

        /// <summary>
        /// Endpoint DELETE per cancellare il token FCM.
        /// </summary>
        /// <param name="token">Token FCM del dispositivo</param>
        /// <param name="location">Località associata alle impostazioni</param>
        /// <returns>Messaggio di conferm dell'aggiornamento del token</returns>

        [HttpDelete("delete-token")]
        public async Task<IActionResult> DeleteToken([FromQuery] string token, [FromQuery] string location)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(location))
                return BadRequest("Token e location sono obbligatori.");

            var settingsRef = _firestoreDb.Collection("settings");
            var query = settingsRef
                .WhereEqualTo("Token", token)
                .WhereEqualTo("Location", location);
            var snapshot = await query.GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                await doc.Reference.DeleteAsync();
            }

            return Ok(new { message = "Combinazione token-location rimossa con successo." });
        }


    }
}
