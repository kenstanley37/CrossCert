using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CrossCert.Models;
using CrossCert.Services;
using System.Collections.ObjectModel;

namespace CrossCert.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly DomainCertService _domainCertService;
        private readonly RenewalService _renewalService;
        private readonly RenewalLogService _renewalLogService;

        public MainPageViewModel(
            DomainCertService domainCertService,
            RenewalService renewalService,
            RenewalLogService renewalLogService)
        {
            _domainCertService = domainCertService;
            _renewalService = renewalService;
            _renewalLogService = renewalLogService;

            Domains = new ObservableCollection<Domain>();
            RenewalLogs = new ObservableCollection<RenewalLog>();

            LoadDomainsCommand = new AsyncRelayCommand(LoadDomainsAsync);
            AddDomainCommand = new AsyncRelayCommand(AddDomainAsync);
            DeleteDomainCommand = new AsyncRelayCommand<Domain>(DeleteDomainAsync);
            ManualRenewCommand = new AsyncRelayCommand(RunManualRenewalAsync);
            ExportCertCommand = new AsyncRelayCommand<Domain>(ExportCertificateAsync);
        }

        public ObservableCollection<Domain> Domains { get; }
        public ObservableCollection<RenewalLog> RenewalLogs { get; }

        public IAsyncRelayCommand LoadDomainsCommand { get; }
        public IAsyncRelayCommand AddDomainCommand { get; }
        public IAsyncRelayCommand<Domain> DeleteDomainCommand { get; }
        public IAsyncRelayCommand ManualRenewCommand { get; }
        public IAsyncRelayCommand<Domain> ExportCertCommand { get; }

        private async Task LoadDomainsAsync()
        {
            var domainList = await _domainCertService.GetDomainsAsync();
            if (domainList is null) return;

            Domains.Clear();
            foreach (var domain in domainList)
                Domains.Add(domain);
        }

        private async Task LoadRenewalLogsAsync()
        {
            var logs = await _renewalLogService.GetRecentLogsAsync();
            if (logs is null) return;

            RenewalLogs.Clear();
            foreach (var log in logs)
                RenewalLogs.Add(log);
        }

        private async Task AddDomainAsync()
        {
            var newDomain = new Domain
            {
                DomainName = "newdomain.com",
                ClientConfigId = 1
            };

            if (string.IsNullOrWhiteSpace(newDomain.DomainName))
                return;

            await _domainCertService.AddDomainAsync(newDomain);
            await LoadDomainsAsync();
        }

        private async Task DeleteDomainAsync(Domain? domain)
        {
            if (domain is null)
                return;
            await _domainCertService.DeleteDomainAsync(domain.Id);
            Domains.Remove(domain);
        }

        private async Task RunManualRenewalAsync()
        {
            await _renewalService.RunRenewalCheckAsync();
            await LoadDomainsAsync();
            await LoadRenewalLogsAsync();
        }

        private async Task ExportCertificateAsync(Domain? domain)
        {
            // TODO: Implement export logic
            await Task.Delay(500);
        }
    }
}