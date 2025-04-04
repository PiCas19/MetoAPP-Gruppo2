namespace BackendProject
{
    // Modello per le impostazioni di notifica utente
    public class UserNotificationSettings
    {
        public string Token { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double TemperatureMax { get; set; }
        public double TemperatureMin { get; set; }
    }
}