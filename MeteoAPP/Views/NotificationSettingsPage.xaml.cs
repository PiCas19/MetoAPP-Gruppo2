using MeteoAPP.ViewModels;

namespace MeteoAPP
{
    [QueryProperty("NotificationSettingsViewModel", "NotificationSettingsViewModel")]
    public partial class NotificationSettingsPage : ContentPage
    {
        private readonly NotificationSettingsViewModel _viewModel;

        public NotificationSettingsPage(NotificationSettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

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

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}