using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MeteoAPP
{

    public class BlazorPage : ContentPage
    {
        public BlazorPage()
        {
            Title = "Blazor Hybrid";

            var blazorWebView = new BlazorWebView
            {
                HostPage = "wwwroot/index.html",
                RootComponents = { new RootComponent {
                Selector = "#app",
                ComponentType = typeof(MeteoAPP.Components.Pages.Main)
            }}
            };

            Content = blazorWebView;
        }
    }
}