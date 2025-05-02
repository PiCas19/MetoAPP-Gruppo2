<<<<<<< HEAD
# MeteoAPP-Gruppo2
=======
# ☁️ MeteoAPP
**MeteoAPP** è un'applicazione mobile multipiattaforma sviluppata con .NET MAUI, che consente di consultare le previsioni meteo in tempo reale, configurare soglie personalizzate per la temperatura e ricevere notifiche push automatiche in caso di superamento.
>>>>>>> dev
## Componenti Gruppo:
- Pierpaolo Casati
- Valmir Alimi
## 📁 Struttura del progetto
La seguente struttura rappresenta l'organizzazione dei file principali del progetto MeteoAPP, suddivisa tra frontend mobile, backend e documentazione.
```plaintext
.
├── MeteoAPP/                     # Applicazione mobile sviluppata in .NET MAUI
│   ├── Views/                   # Pagine dell'interfaccia utente (UI)
│   ├── ViewModels/              # Logica di presentazione secondo il pattern MVVM
│   ├── Models/                  # Classi dati, come City e WeatherData
│   ├── Services/                # Servizi per meteo, geolocalizzazione, Appwrite, Firebase, SQLite
│   ├── Resources/               # Risorse statiche: immagini, font, icone
│   ├── Platforms/               # Codice e configurazioni specifiche per Android, iOS, ecc.
│   └── wwwroot/                 # Contenuti web (Chart.js, Leaflet.js) integrati via BlazorWebView
│
├── TemperatureMonitorBackend/   # Backend ASP.NET Core per API REST e monitoraggio meteo
│   ├── Controllers/             # API RESTful per la gestione delle impostazioni utente
│   ├── Services/                # Servizio in background (HostedService) per invio automatico notifiche
│   └── serviceAccountKey.json   # Chiave di servizio per l'accesso al Firebase Admin SDK
│
└── doc/
    └── 📄 Rapporto_Progetto_MetoAPP_CasatiAlimi.pdf   # Documentazione del progetto in formato PDF
```
## 🚀 Funzionalità principali
- Visualizzazione delle condizioni meteo attuali e giornaliere.
- Ricerca e gestione delle località preferite.
- Aggiunta di località tramite mappa interattiva (Leaflet).
- Sincronizzazione dati su cloud via Appwrite.
- Configurazione soglie di temperatura personalizzate.
- Invio automatico di notifiche push tramite Firebase Cloud Messaging.
- Backend RESTful in ASP.NET Core con salvataggio su Firestore.
- Servizio in background (HostedService) per monitoraggio e notifiche.
## 🛠️ Tecnologie utilizzate
**Frontend**
- [.NET MAUI](https://dotnet.microsoft.com/it-it/apps/maui) — Framework multipiattaforma per lo sviluppo di app native.
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm?view=net-maui-9.0) — Architettura per separare logica di business, dati e UI.
- [SQLite](https://www.sqlite.org/index.html) — Database locale leggero e integrabile su dispositivi mobili.
- [Appwrite SDK](https://appwrite.io/docs) — Backend-as-a-Service per sincronizzazione cloud sicura.
- [Firebase Cloud Messaging (FCM)](https://firebase.google.com/docs/cloud-messaging) — Infrastruttura per invio di notifiche push ai dispositivi mobili.
- [Blazor WebView](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/blazorwebview?view=net-maui-9.0) — Integrazione di componenti web all'interno di app .NET MAUI.
- [Chart.js](https://www.chartjs.org/) — Libreria JavaScript per visualizzazione di grafici dinamici.
- [Leaflet.js](https://leafletjs.com/) — Libreria JavaScript open-source per mappe interattive.

## ⚙️ Esecuzione del progetto
**1. Configura l’ambiente**
- Visual Studio 2022 o Visual Studio Code con workload **.NET MAUI**
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Appwrite](https://appwrite.io/) (in esecuzione locale o su cloud)
- [Firebase Console](https://console.firebase.google.com/): scarica i seguenti file e inseriscili nelle rispettive cartelle:
     - `google-services.json` (per Android)
     - `serviceAccountKey.json` (per il backend ASP.NET Core)
- [OpenWeather](https://openweathermap.org/api): genera una **API Key** gratuita per accedere ai dati meteo

**2. Configura `config.json`**
 Inserisci nella directory principale del progetto MAUI (`MeteoAPP/`) un file `config.json` con i seguenti contenuti (dati d'esempio):

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
**3. Avvia il frontend (MeteoAPP)**
Accedi alla directory del progetto MAUI:

```bash
cd MeteoAPP
dotnet build -t:Run -f net9.0-android
```
**4. Avvia il backend (TemperatureMonitorBackend)**
Spostati nella cartella del backend:
```bash
cd TemperatureMonitorBackend
dotnet run
```
L’API sarà esposta su http://localhost:5000

**5. Esporre l’API pubblicamente (opzionale ma necessario per il mobile)**
Per testare le notifiche push da dispositivi mobili, devi esporre il backend con Ngrok:
```bash
ngrok http 5000
```
Ngrok ti restituirà un URL pubblico tipo:
```perl
https://1234-56-789-000.ngrok-free.app -> http://localhost:5000
```
Copia questo URL e aggiornalo nel file `config.json` del progetto mobile nel campo `NotificationSettingsBaseUrl`:
```json
{
    "NotificationSettingsBaseUrl": "https://ec0d-xxxx-xxxx-xxxx.ngrok-free.app" 
}
```
## Documentazione
Per maggiori dettagli tecnici, architetturali e di implementazione, consulta il documento PDF allegato:
📄 [Visualizza la documentazione completa (PDF)](https://github.com/PiCas19/MetoAPP-Gruppo2/blob/dev/doc/Rapporto_Progetto_MetoAPP_CasatiAlimi.pdf)