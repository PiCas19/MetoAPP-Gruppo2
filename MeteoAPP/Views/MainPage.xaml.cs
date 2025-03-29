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
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                await DisplayAlert("OK", token, "Ok");
                //await Shell.Current.GoToAsync("//ListMeteoPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to proceed: {ex.Message}", "OK");
            }
        }
    }
}