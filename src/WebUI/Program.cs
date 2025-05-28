// src/WebUI/Program.cs - Configuration CORS corrigée
using _Net6CleanArchitectureQuizzApp.Application.Common.OpenAI;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();

// Configuration CORS - CORRIGÉE
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Important pour les cookies/auth si nécessaire
    });

    // Politique plus permissive pour le développement
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    // Utiliser la politique CORS pour le développement
    app.UseCors("Development");

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
else
{
    // Production: utiliser la politique restrictive
    app.UseCors("AllowFrontend");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseStaticFiles();

// Configuration Swagger/OpenAPI
app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";
    settings.DocumentPath = "/api/specification.json";
});

app.UseRouting();

// IMPORTANT: L'ordre est crucial - CORS doit être avant Authentication/Authorization
// app.UseCors() est déjà appelé plus haut selon l'environnement

app.UseAuthentication();
//app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }