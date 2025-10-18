using Core.Interfaces;
using Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GUI.ViewModels
{
    public class MainPageViewModel
    {
        private readonly ICertificateManager _certManager;
        private readonly IRenewalScheduler _renewalScheduler;

        public ObservableCollection<Domain> Domains { get; } = [];
        public ObservableCollection<RenewalLog> RenewalLogs { get; } = [];

        public ICommand AddDomainCommand { get; }
        public ICommand ManualRenewCommand { get; }

        public ICommand ShowAboutCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowHelpCommand { get; }

        public MainPageViewModel(ICertificateManager certManager, IRenewalScheduler renewalScheduler)
        {
            _certManager = certManager;
            _renewalScheduler = renewalScheduler;

            _renewalScheduler.RenewalCompleted += OnRenewalCompleted;

            AddDomainCommand = new Command(AddDomain);
            ManualRenewCommand = new Command(async () => await _renewalScheduler.RunManualRenewalAsync());

            ShowAboutCommand = new Command(() =>
                Application.Current?.Windows[0].Page?.DisplayAlert("About", "CrossCert v1.0\nSSL Certificate Manager", "OK"));

            ShowSettingsCommand = new Command(() =>
                Application.Current?.Windows[0].Page?.DisplayAlert("Settings", "Settings panel coming soon.", "OK"));

            ShowHelpCommand = new Command(() =>
                Application.Current?.Windows[0].Page?.DisplayAlert("Help", "Visit our docs or contact support.", "OK"));
        }

        private void AddDomain()
        {
            var domain = new Domain { DomainName = "example.com", IsActive = true };
            Domains.Add(domain);
            _renewalScheduler.ScheduleRenewalAsync(domain);
        }

        private void OnRenewalCompleted(object? sender, RenewalLog log)
        {
            Application.Current?.Dispatcher.Dispatch(() => RenewalLogs.Add(log));
        }
    }
}