namespace MeteoApp;

public partial class MeteoPromptPage : ContentPage
{
    public MeteoPromptPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(cityEntry.Text))
        {
            await DisplayAlert("City Added", $"You have added: {cityEntry.Text}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await DisplayAlert("Error", "Please enter a city name", "OK");
        }
    }
}