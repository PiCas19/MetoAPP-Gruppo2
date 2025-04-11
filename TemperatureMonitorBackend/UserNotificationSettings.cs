using Google.Cloud.Firestore;

namespace BackendProject
{
    [FirestoreData]
    public class UserNotificationSettings
    {
        [FirestoreProperty]
        public string Token { get; set; } = string.Empty;

        [FirestoreProperty]
        public string Location { get; set; } = string.Empty;

        [FirestoreProperty]
        public double TemperatureMax { get; set; }

        [FirestoreProperty]
        public double TemperatureMin { get; set; }
    }
}
