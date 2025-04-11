using Plugin.Firebase.CloudMessaging;

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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to proceed: {ex.Message}", "OK");
            }
        }
    }
}