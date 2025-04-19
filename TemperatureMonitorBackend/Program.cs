using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Text.Json;
using BackendProject;

var builder = WebApplication.CreateBuilder(args);

// Percorso alla chiave Firebase
var keyPath = Path.Combine(AppContext.BaseDirectory, "serviceAccountKey.json");

// Crea credenziali da file
var credential = GoogleCredential.FromFile(keyPath);

// Inizializza Firebase
FirebaseApp.Create(new AppOptions
{
    Credential = credential
});

// Ottieni projectId dal JSON
string json = File.ReadAllText(keyPath);
using var doc = JsonDocument.Parse(json);
string? projectId = doc.RootElement.GetProperty("project_id").GetString();

if (string.IsNullOrEmpty(projectId))
    throw new InvalidOperationException("project_id non trovato nel file serviceAccountKey.json");

// Crea Firestore client con le credenziali
var firestoreClient = new FirestoreClientBuilder
{
    Credential = credential
}.Build();

// Istanzia FirestoreDb
var db = FirestoreDb.Create(projectId, firestoreClient);

// Servizi
builder.Services.AddSingleton(db);
builder.Services.AddHostedService<TemperatureMonitoringService>();
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Temperature Monitor API", Version = "v1" });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
