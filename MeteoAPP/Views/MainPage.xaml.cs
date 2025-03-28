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
                await DisplayAlert("Error", $"Unable to proceed: {ex.Message}", "OK");
            }
        }
    }
}