using Plugin.Firebase.CloudMessaging;

namespace MeteoAPP
{
    /// <summary>
    /// Pagina iniziale dell'applicazione MeteoAPP.
    /// Contiene la logica per avviare l'interazione iniziale dell'utente.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Costruttore della MainPage.
        /// Inizializza i componenti grafici della pagina.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metodo gestore dell'evento di click sul pulsante di avvio.
        /// Attualmente non implementato, ma predisposto per avviare operazioni iniziali.
        /// </summary>
        /// <param name="sender">Il controllo che ha generato l'evento (es. Button).</param>
        /// <param name="e">Argomenti associati all'evento di click.</param>
        private async void OnStartClicked(object sender, EventArgs e)
        {
            try
            {
                string selectedProvider = providerPicker.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedProvider))
                {
                    await DisplayAlert("Errore", "Seleziona un provider meteo prima di continuare.", "OK");
                    return;
                }

                if (selectedProvider == "OpenWeather")
                {
                    // Naviga alla ListMeteoPage che usa già OpenWeatherService
                    await Navigation.PushAsync(new ListMeteoPage(selectedProvider));
                }
                else
                {
                    await DisplayAlert("Provider non supportato", "Per ora è disponibile solo OpenWeather.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Errore durante l'avvio: {ex.Message}", "OK");
            }
        }
    }
}