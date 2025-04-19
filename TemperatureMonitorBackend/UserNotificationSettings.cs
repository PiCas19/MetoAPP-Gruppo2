using Google.Cloud.Firestore;

namespace BackendProject
{
    /// <summary>
    /// Modello che rappresenta le impostazioni di notifica personalizzate di un utente.
    /// Salvato in Firestore nella collezione "settings".
    /// </summary>
    /// <remarks>
    /// Utilizzato dal backend ASP.NET Core per recuperare le soglie definite
    /// e inviare notifiche push tramite Firebase Cloud Messaging.
    /// </remarks>
    [FirestoreData]
    public class UserNotificationSettings
    {
        /// <summary>
        /// Token FCM univoco del dispositivo mobile.
        /// Utilizzato per identificare l’utente e inviare notifiche push.
        /// </summary>
        [FirestoreProperty]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Nome della località associata alle notifiche (es. "Milano, Italia").
        /// </summary>
        [FirestoreProperty]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Soglia di temperatura massima oltre la quale viene inviata una notifica.
        /// </summary>
        [FirestoreProperty]
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Soglia di temperatura minima sotto la quale viene inviata una notifica.
        /// </summary>
        [FirestoreProperty]
        public double TemperatureMin { get; set; }
        
        /// <summary>
        /// Indica se le notifiche sono attivate o disattivate per questa località.
        /// </summary>
        [FirestoreProperty]
        public bool IsEnabled { get; set; } = true; 
    }
}
