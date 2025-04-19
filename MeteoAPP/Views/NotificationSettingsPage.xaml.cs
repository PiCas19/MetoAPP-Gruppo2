using MeteoAPP.ViewModels;

namespace MeteoAPP
{
    /// <summary>
    /// Pagina per la configurazione delle notifiche personalizzate meteo.
    /// Consente all'utente di impostare soglie di temperatura per ricevere notifiche.
    /// </summary>
    [QueryProperty("NotificationSettingsViewModel", "NotificationSettingsViewModel")]
    public partial class NotificationSettingsPage : ContentPage
    {
        /// <summary>
        /// ViewModel associata alla pagina, contenente la logica di salvataggio e caricamento.
        /// </summary>
        private readonly NotificationSettingsViewModel _viewModel;


        /// <summary>
        /// Costruttore della pagina di configurazione notifiche.
        /// </summary>
        /// <param name="viewModel">ViewModel che gestisce le impostazioni dell'utente.</param>
        public NotificationSettingsPage(NotificationSettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Evento che viene eseguito quando l’utente clicca su "Salva".
        /// Valida gli input e richiama il salvataggio tramite ViewModel.
        /// </summary>
        /// <param name="sender">Oggetto mittente.</param>
        /// <param name="e">Argomento dell’evento.</param>
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                if (!double.TryParse(HighTempThresholdEntry.Text, out double highTemp) ||
                    !double.TryParse(LowTempThresholdEntry.Text, out double lowTemp))
                {
                    await DisplayAlert("Invalid Input", "Please enter valid temperature values", "OK");
                    return;
                }
                _viewModel.HighTemperatureThreshold = highTemp;
                _viewModel.LowTemperatureThreshold = lowTemp;
                
                await _viewModel.SaveSettingsAsync();
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Evento che viene eseguito quando l’utente clicca su "Annulla".
        /// Chiude la pagina modale senza salvare.
        /// </summary>
        /// <param name="sender">Oggetto mittente.</param>
        /// <param name="e">Argomento dell’evento.</param>
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}