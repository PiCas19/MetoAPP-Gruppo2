namespace MeteoApp;
using System.ComponentModel;

[QueryProperty(nameof(LocationName), "location")]
[QueryProperty(nameof(TemperatureString), "temperature")]  
[QueryProperty(nameof(Description), "description")]
[QueryProperty(nameof(Icon), "icon")]
[QueryProperty(nameof(MinTemperature), "minTemperature")]
[QueryProperty(nameof(MaxTemperature), "maxTemperature")]
public partial class MeteoItemPage : ContentPage, INotifyPropertyChanged
{
    private string locationName;
    private string description;
    private string temperatureString; 
    private double temperatureValue;
    private string icon;
    private string minTemperature;
    private string maxTemperature;
    private double minTemperatureValue;
    private double maxTemperatureValue;

    public string LocationName
    {
        get => locationName;
        set
        {
            locationName = Uri.UnescapeDataString(value);
            Console.WriteLine($"Set LocationName: {locationName}");
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => description;
        set
        {
            description = Uri.UnescapeDataString(value);
            Console.WriteLine($"Set Description: {description}");
            OnPropertyChanged();
        }
    }

    public string TemperatureString
    {
        get => temperatureString;
        set
        {
            temperatureString = value ?? "0";
            if (double.TryParse(temperatureString, System.Globalization.CultureInfo.InvariantCulture, out double temp))
            {
                temperatureValue = temp;
                Console.WriteLine($"Parsed Temperature: {temperatureValue}");
                OnPropertyChanged(nameof(TemperatureValue));
            }
            OnPropertyChanged();
        }
    }
    public double TemperatureValue => temperatureValue;

    public string Icon
    {
        get => icon;
        set
        {
            icon = value;
            Console.WriteLine($"Set Icon: {icon}");
            OnPropertyChanged();
            OnPropertyChanged(nameof(WeatherIconUrl));
        }
    }

    public string WeatherIconUrl => !string.IsNullOrEmpty(Icon) 
        ? $"https://openweathermap.org/img/wn/{Icon}.png"
        : string.Empty;

    public string MinTemperature
    {
        get => minTemperature;
        set
        {
            minTemperature = value ?? "0";
            if (double.TryParse(minTemperature, System.Globalization.CultureInfo.InvariantCulture, out double temp))
            {
                minTemperatureValue = temp;
                Console.WriteLine($"Parsed Min Temperature: {minTemperatureValue}");
                OnPropertyChanged(nameof(MinTemperatureValue));
            }
            OnPropertyChanged();
        }
    }
    public double MinTemperatureValue => minTemperatureValue;

    public string MaxTemperature
    {
        get => maxTemperature;
        set
        {
            maxTemperature = value ?? "0";
            if (double.TryParse(maxTemperature, System.Globalization.CultureInfo.InvariantCulture, out double temp))
            {
                maxTemperatureValue = temp;
                Console.WriteLine($"Parsed Max Temperature: {maxTemperatureValue}");
                OnPropertyChanged(nameof(MaxTemperatureValue));
            }
            OnPropertyChanged();
        }
    }
    public double MaxTemperatureValue => maxTemperatureValue;

    public MeteoItemPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

     protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width > height)
        {
            TemperatureLabel.FontSize = 48;
            TemperatureLabel.Margin = new Thickness(0, 40, 0, 5);
            WeatherIcon.WidthRequest = 80;
            WeatherIcon.HeightRequest = 80;
            Console.WriteLine("📱 Landscape Mode: FontSize 48, Icon 80px");
        }
        else
        {
            TemperatureLabel.FontSize = 64;
            TemperatureLabel.Margin = new Thickness(0, 10, 0, 10);
            WeatherIcon.WidthRequest = 100;
            WeatherIcon.HeightRequest = 100;
            Console.WriteLine("📱 Portrait Mode: FontSize 64, Icon 100px");
        }
    }
    
}
