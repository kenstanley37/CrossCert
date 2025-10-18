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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // --- Database Configuration (EF Core with SQLite) ---
            builder.Services.AddDbContext<CertDbContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "certs.db");
                options.UseSqlite($"Data Source={dbPath}");
            });

            // --- Service Registration ---
            builder.Services.AddSingleton<DomainCertService>();
            builder.Services.AddSingleton<AcmeClientService>(); // NEW
            builder.Services.AddSingleton<RenewalService>();
            builder.Services.AddSingleton<RenewalLogService>();



            // --- ViewModel Registration ---
            builder.Services.AddTransient<MainPageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            // --- Ensure DB is created and migrations applied ---
            CreateDatabase(app.Services);

            return app;
        }

        private static void CreateDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<CertDbContext>();
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL DB ERROR] Failed to apply migrations: {ex.Message}");
            }
        }
    }
}