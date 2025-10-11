using CrossCert.Models;
using CrossCert.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CrossCert.ViewModels;

/// <summary>
/// ViewModel for the MainPage (Domain Dashboard).
/// Responsible for fetching, managing, and adding new domains directly on the dashboard.
/// </summary>
public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly CertManagerDataService _dataService;

    // --- Commands for Domain Management ---
    public ICommand LoadDomainsCommand { get; }
    public ICommand DeleteDomainCommand { get; }
    public ICommand RenewDomainCommand { get; } // Placeholder

    // --- Commands for Adding New Domain Form ---
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ToggleAddFormCommand { get; } // Command to show/hide the input form

    // ObservableCollection updates the UI automatically when items are added or removed
    public ObservableCollection<Domain> Domains { get; } = new ObservableCollection<Domain>();

    // --- Properties for New Domain Input Form ---
    private string _domainName = string.Empty;
    public string DomainName
    {
        get => _domainName;
        set
        {
            if (SetProperty(ref _domainName, value))
            {
                // Re-evaluate the Save button state whenever the domain name changes
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    private string _altNames = string.Empty;
    public string AltNames
    {
        get => _altNames;
        set => SetProperty(ref _altNames, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                // Re-evaluate the Save button state when busy state changes
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    private bool _isAddFormVisible = false;
    /// <summary>
    /// Controls the visibility of the 'Add Domain' input form on the main page.
    /// </summary>
    public bool IsAddFormVisible
    {
        get => _isAddFormVisible;
        set => SetProperty(ref _isAddFormVisible, value);
    }

    /// <summary>
    /// Constructor receives the data service via Dependency Injection.
    /// </summary>
    public MainPageViewModel(CertManagerDataService dataService)
    {
        _dataService = dataService;

        // Initialize Commands for management
        LoadDomainsCommand = new Command(async () => await LoadDomainsAsync());
        DeleteDomainCommand = new Command<Domain>(async (domain) => await ExecuteDeleteDomainCommand(domain));
        RenewDomainCommand = new Command<Domain>((domain) => Console.WriteLine($"Renewing {domain.Name}... (Not implemented yet)"));

        // Initialize Commands for Add Form
        SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanSave);
        // FIX: Use ResetAddForm to ensure fields are cleared when canceling
        CancelCommand = new Command(ResetAddForm);
        ToggleAddFormCommand = new Command(() => IsAddFormVisible = !IsAddFormVisible);

        // Load data on creation for initial UI population
        Task.Run(LoadDomainsAsync);
    }

    // --- Domain Management Logic ---

    /// <summary>
    /// Fetches all domains from the database and populates the ObservableCollection.
    /// </summary>
    public async Task LoadDomainsAsync()
    {
        IsBusy = true;
        try
        {
            // Clear current list before loading new data
            Domains.Clear();

            var domains = await _dataService.GetAllDomainsAsync();
            foreach (var domain in domains)
            {
                Domains.Add(domain);
            }
        }
        catch (Exception ex)
        {
            // Log the error if data fetching fails
            Console.WriteLine($"Error loading domains: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes the delete operation for a given domain and updates the UI list.
    /// </summary>
    private async Task ExecuteDeleteDomainCommand(Domain domain)
    {
        if (domain == null) return;

        // IMPORTANT: Use Shell.Current.DisplayAlert for simple confirmations in MAUI
        bool confirmed = await Shell.Current.DisplayAlert(
            "Confirm Deletion",
            $"Are you sure you want to delete the domain '{domain.Name}' and its associated certificate data?",
            "Yes, Delete",
            "Cancel");

        if (confirmed)
        {
            try
            {
                await _dataService.DeleteDomainAsync(domain.Id);
                // Remove from the ObservableCollection to instantly update the UI
                Domains.Remove(domain);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting domain: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Could not delete {domain.Name}. See logs.", "OK");
            }
        }
    }

    // --- Add Domain Form Logic (Moved from AddDomainPageViewModel) ---

    /// <summary>
    /// Determines if the Save button should be enabled.
    /// </summary>
    private bool CanSave() => !string.IsNullOrWhiteSpace(DomainName) && !IsBusy;

    /// <summary>
    /// Resets the input fields and hides the add domain form.
    /// </summary>
    private void ResetAddForm()
    {
        DomainName = string.Empty;
        AltNames = string.Empty;
        IsAddFormVisible = false;
        // The DomainName setter already calls ChangeCanExecute, but calling it here 
        // ensures the Save button is always correctly disabled after a reset.
        ((Command)SaveCommand).ChangeCanExecute();
    }

    /// <summary>
    /// Executes the command to save the new domain.
    /// </summary>
    private async Task ExecuteSaveCommand()
    {
        if (IsBusy || !CanSave()) return;

        IsBusy = true;

        try
        {
            var newDomain = new Domain
            {
                Name = DomainName.Trim(),
                SubjectAlternativeNames = AltNames.Trim(),
                // Default renewal attempt set for 30 days from now (placeholder logic)
                NextRenewalAttempt = DateTime.Now.AddDays(30)
            };

            await _dataService.AddDomainAsync(newDomain);

            // 1. Refresh the main list to show the new domain
            Domains.Add(newDomain);

            // 2. Clear the form fields and hide the form
            ResetAddForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving domain: {ex.Message}");
            await Shell.Current.DisplayAlert("Save Error", "Failed to save the domain. Check logs for details.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }


    // --- Status Helpers for UI Display ---

    /// <summary>
    /// Determines the verification/certificate status of a domain.
    /// </summary>
    public static string GetDomainStatus(Domain domain)
    {
        if (domain.Certificate == null)
        {
            return "Pending Verification / No Certificate";
        }

        // Use UTC for comparison if data is stored in UTC, otherwise use local time for display comparison.
        // Assuming DateTime.Now for simplicity here.
        if (domain.Certificate.ExpiryDate < DateTime.Now)
        {
            return $"Expired ({domain.Certificate.ExpiryDate:yyyy-MM-dd})";
        }

        if (domain.Certificate.ExpiryDate < DateTime.Now.AddDays(30))
        {
            return $"Expiring Soon ({domain.Certificate.ExpiryDate:yyyy-MM-dd})";
        }

        return $"Active ({domain.Certificate.ExpiryDate:yyyy-MM-dd})";
    }


    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
