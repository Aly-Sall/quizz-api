// src/WebUI/Program.cs - Assurez-vous que ces lignes sont présentes
using _Net6CleanArchitectureQuizzApp.Application.Common.OpenAI;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration); // ✅ Ceci devrait enregistrer IApplicationDbContext
builder.Services.AddWebUIServices();

// Configuration CORS
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseCors("Development");

    // Initialiser la base de données
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
else
{
    app.UseCors("AllowFrontend");
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/swagger";
    settings.DocumentPath = "/api/specification.json";
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }