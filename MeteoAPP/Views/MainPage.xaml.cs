using MeteoApp;

namespace MeteoAPP
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//ListMeteoPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Impossibile procedere: {ex.Message}", "OK");
            }
        }
    }
}