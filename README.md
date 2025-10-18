CrossCert Manager
🛡️ Certificate Management on the Go
CrossCert is a cross-platform application built with .NET MAUI and Entity Framework Core (SQLite) designed to help manage and monitor SSL/TLS certificates for multiple domains. This application is intended to serve as a portable client for handling the entire lifecycle of certificates, from initial issuance to automated renewal.

The architecture is currently divided into two primary parts:

MAUI_Client: The cross-platform user interface and local data storage, providing a domain dashboard and configuration pages.

Renewal Service (Planned): A background service (to be developed) responsible for communicating with ACME providers (like Let's Encrypt) and executing certificate renewal jobs.

🚀 Getting Started
Prerequisites
.NET 9 SDK: Ensure you have the latest .NET 9 SDK installed.

Visual Studio 2022 (v17.9+) or JetBrains Rider with MAUI workload installed.

Platform SDKs: Depending on your target platform (Android, iOS, Windows, Mac Catalyst), you will need the corresponding development tools installed (e.g., Android SDK, Xcode for iOS/Mac).

Building and Running
Clone the Repository:

git clone [Your Repository URL]
cd CrossCert

Restore Dependencies:

dotnet restore

Run the Application:
Open the CrossCert.sln file in your IDE. Select your target platform (e.g., net9.0-android or net9.0-windows10.0.19041.0) and run the project.

📁 Project Structure
MAUI_Client/: Contains the main MAUI application project (the primary UI).

Data/: Entity Framework Core DbContext, migrations, and factory classes.

Models/: Core data models (Domain, Certificate, ClientConfig, etc.).

Services/: Business logic, notably CertManagerDataService.cs.

ViewModels/: Logic for the XAML pages (MainPageViewModel.cs, AddDomainPageViewModel.cs).

CrossCert.sln: The Visual Studio solution file.

🗄️ Database
The application uses SQLite with Entity Framework Core for local data persistence.

Database File: CrossCert.db (Stored in the platform-specific application data directory).

Migrations: Database schema updates are managed using EF Core Migrations, run automatically on application startup via MauiProgram.cs.

To create a new migration (when working on the data models):

dotnet ef migrations add [MigrationName] --project MAUI_Client --startup-project MAUI_Client

(Note: Replace MAUI_Client with the actual project folder name if it's different)