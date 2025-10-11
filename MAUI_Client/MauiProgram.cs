using CommunityToolkit.Maui; // <--- ADDED
using CrossCert.Data;
using CrossCert.Services;
using CrossCert.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrossCert
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the MAUI Community Toolkit here, chained to the main app setup.
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // --- Database Configuration (EF Core with SQLite) ---

            // 1. Configure the DbContext as a Scoped Service
            builder.Services.AddDbContext<CrossCertDbContext>(options =>
            {
                // Configure the context to use SQLite and point it to the platform-specific path
                options.UseSqlite($"Filename={CrossCertDbContext.GetDatabasePath()}");
            });

            // 2. Register Services and ViewModels for Dependency Injection

            // Register the Data Service (Singleton recommended for application-wide data access)
            builder.Services.AddSingleton<CertManagerDataService>();

            // Register View Models (Transient recommended for page lifecycle)
            builder.Services.AddTransient<MainPageViewModel>(); // CRITICAL: This needs access to the publ...
            builder.Services.AddTransient<AddDomainPageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            // 3. Ensure the database is created and migrations are applied on startup
            CreateDatabase(app.Services);

            return app;
        }

        /// <summary>
        /// Ensures the SQLite database exists and applies any pending Entity Framework migrations.
        /// This method is run once during application startup.
        /// </summary>
        private static void CreateDatabase(IServiceProvider serviceProvider)
        {
            // Using a service scope is necessary to resolve scoped services
            // (like DbContext) outside of the main application request scope.
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<CrossCertDbContext>();

                // This will create the database file if it doesn't exist and
                // apply all pending migrations, ensuring the schema is up-to-date.
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Log the critical error if migrations fail.
                Console.WriteLine($"[CRITICAL DB ERROR] Failed to apply migrations: {ex.Message}");
            }
        }
    }
}
