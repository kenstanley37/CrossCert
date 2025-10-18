using CrossCert.Pages; // Added this using statement

namespace CrossCert
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // We register the routes here so we can navigate to them by name
            // Register all pages that are not directly included in the Shell Content definition here.

            // AddDomainPage route registration removed as it is now deleted.

            // Registering the routes for the new pages
            Routing.RegisterRoute("settings", typeof(SettingsPage)); // Simplified type reference due to 'using'
            Routing.RegisterRoute("about", typeof(AboutPage));     // Simplified type reference due to 'using'
        }

        // The OnAddDomainClicked method has been removed.

        /// <summary>
        /// Handles the click event from the "Settings" button in the toolbar.
        /// </summary>
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // Navigate to the Settings Page
            await Shell.Current.GoToAsync("settings");
        }

        /// <summary>
        /// Handles the click event from the "About" button in the toolbar.
        /// NOTE: This button needs to be added to AppShell.xaml.
        /// </summary>
        private async void OnAboutClicked(object sender, EventArgs e)
        {
            // Navigate to the About Page
            await Shell.Current.GoToAsync("about");
        }
    }
}
