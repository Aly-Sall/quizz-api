// src/WebUI/Program.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.OpenAI;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

try
{
    // ✅ Configuration des services dans le bon ordre
    Console.WriteLine("🔍 Configuring services...");

    // 1. Services HTTP et externes
    builder.Services.AddHttpClient<OpenAIService>();

    // 2. Services d'application (MediatR, AutoMapper, Validators)
    Console.WriteLine("🔍 Adding Application services...");
    builder.Services.AddApplicationServices();

    // 3. Services d'infrastructure (Identity, DbContext, etc.)
    Console.WriteLine("🔍 Adding Infrastructure services...");
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // 4. Services Web UI (Controllers, CORS, etc.)
    Console.WriteLine("🔍 Adding WebUI services...");
    builder.Services.AddWebUIServices();

    // ✅ Configuration CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });

        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    Console.WriteLine("✅ Services configured successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error configuring services: {ex.Message}");
    Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
    throw;
}

var app = builder.Build();

// ✅ Configuration du pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseCors("Development");

    // ✅ Initialisation de la base de données avec gestion d'erreur
    try
    {
        Console.WriteLine("🔍 Initializing database...");
        using (var scope = app.Services.CreateScope())
        {
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
        Console.WriteLine("✅ Database initialization completed");
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInit");
        logger.LogError(ex, "❌ An error occurred during database initialization");
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
        // Ne pas arrêter l'application, continuer sans l'initialisation
    }
}
else
{
    app.UseCors("AllowFrontend");
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseStaticFiles();

// ✅ Configuration Swagger/OpenAPI
app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";
    settings.DocumentPath = "/api/specification.json";
});

// ✅ Pipeline de requêtes - ORDRE IMPORTANT
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Configuration des routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapFallbackToFile("index.html");

Console.WriteLine("🚀 Application starting...");
app.Run();

public partial class Program { }