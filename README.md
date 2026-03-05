# MeteoAPP-Group2
# ☁️ MeteoAPP
**MeteoAPP** is a cross-platform mobile application developed with .NET MAUI that allows users to check real-time weather forecasts, configure custom temperature thresholds, and receive automatic push notifications when those thresholds are exceeded.

## Group Members:
- Pierpaolo Casati
- Valmir Alimi

## 📁 Project Structure
The following structure represents the organization of the main files of the MeteoAPP project, divided between the mobile frontend, backend, and documentation.

```plaintext
.
├── MeteoAPP/                     # Mobile application developed with .NET MAUI
│   ├── Views/                   # User interface (UI) pages
│   ├── ViewModels/              # Presentation logic following the MVVM pattern
│   ├── Models/                  # Data classes such as City and WeatherData
│   ├── Services/                # Services for weather, geolocation, Appwrite, Firebase, SQLite
│   ├── Resources/               # Static resources: images, fonts, icons
│   ├── Platforms/               # Platform-specific code and configurations (Android, iOS, etc.)
│   └── wwwroot/                 # Web content (Chart.js, Leaflet.js) integrated via BlazorWebView
│
├── TemperatureMonitorBackend/   # ASP.NET Core backend for REST APIs and weather monitoring
│   ├── Controllers/             # RESTful APIs for managing user settings
│   ├── Services/                # Background service (HostedService) for automatic notification sending
│   └── serviceAccountKey.json   # Service key for accessing the Firebase Admin SDK
│
└── doc/
    └── 📄 Project_Report_MeteoAPP_CasatiAlimi.pdf   # Project documentation in PDF format
````

## 🚀 Main Features

* Display of current and daily weather conditions.
* Search and management of favorite locations.
* Adding locations via an interactive map (Leaflet).
* Cloud data synchronization via Appwrite.
* Custom temperature threshold configuration.
* Automatic push notification delivery via Firebase Cloud Messaging.
* RESTful backend in ASP.NET Core with Firestore storage.
* Background service (HostedService) for monitoring and notifications.

## 🛠️ Technologies Used

**Frontend**

* [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui) — Cross-platform framework for developing native applications.
* [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm?view=net-maui-9.0) — Architecture for separating business logic, data, and UI.
* [SQLite](https://www.sqlite.org/index.html) — Lightweight local database suitable for mobile devices.
* [Appwrite SDK](https://appwrite.io/docs) — Backend-as-a-Service for secure cloud synchronization.
* [Firebase Cloud Messaging (FCM)](https://firebase.google.com/docs/cloud-messaging) — Infrastructure for sending push notifications to mobile devices.
* [Blazor WebView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/blazorwebview?view=net-maui-9.0) — Integration of web components within .NET MAUI applications.
* [Chart.js](https://www.chartjs.org/) — JavaScript library for dynamic chart visualization.
* [Leaflet.js](https://leafletjs.com/) — Open-source JavaScript library for interactive maps.

## ⚙️ Running the Project

**1. Configure the environment**

* Visual Studio 2022 or Visual Studio Code with the **.NET MAUI** workload
* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [Appwrite](https://appwrite.io/) (running locally or on the cloud)
* [Firebase Console](https://console.firebase.google.com/): download the following files and place them in the respective folders:

  * `google-services.json` (for Android)
  * `serviceAccountKey.json` (for the ASP.NET Core backend)
* [OpenWeather](https://openweathermap.org/api): generate a free **API Key** to access weather data

**2. Configure `config.json`**
Insert a `config.json` file in the main directory of the MAUI project (`MeteoAPP/`) with the following content (example data):

```json
{
  "OpenWeatherApiKey": "<YOUR_OPENWEATHER_API_KEY>",
  "AppwriteProjectId": "<YOUR_APPWRITE_PROJECT_ID>",
  "AppwriteApiKey": "<YOUR_APPWRITE_API_KEY>",
  "AppwriteDatabaseId": "<YOUR_DATABASE_ID>",
  "AppwriteCollectionId": "<YOUR_COLLECTION_ID>",
  "NotificationSettingsBaseUrl": "https://ec0d-xxxx-xxxx-xxxx.ngrok-free.app"
}
```

**3. Start the frontend (MeteoAPP)**
Go to the MAUI project directory:

```bash
cd MeteoAPP
dotnet build -t:Run -f net9.0-android
```

**4. Start the backend (TemperatureMonitorBackend)**
Move to the backend folder:

```bash
cd TemperatureMonitorBackend
dotnet run
```

The API will be available at:
`http://localhost:5000`

**5. Expose the API publicly (optional but required for mobile)**
To test push notifications from mobile devices, you need to expose the backend using Ngrok:

```bash
ngrok http 5000
```

Ngrok will return a public URL similar to:

```perl
https://1234-56-789-000.ngrok-free.app -> http://localhost:5000
```

Copy this URL and update it in the mobile project's `config.json` file under the `NotificationSettingsBaseUrl` field:

```json
{
  "NotificationSettingsBaseUrl": "https://ec0d-xxxx-xxxx-xxxx.ngrok-free.app"
}
```

## Documentation

For more technical, architectural, and implementation details, refer to the attached PDF document:

📄 [View the complete documentation (PDF)](https://github.com/PiCas19/MetoAPP-Gruppo2/blob/dev/doc/Rapporto_Progetto_MetoAPP_CasatiAlimi.pdf)
