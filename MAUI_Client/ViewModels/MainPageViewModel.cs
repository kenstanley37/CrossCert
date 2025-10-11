using CrossCert.Models;
using CrossCert.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CrossCert.ViewModels;

/// <summary>
/// ViewModel for the MainPage (Domain Dashboard).
/// Responsible for fetching, managing, and adding new domains.
/// </summary>
public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly CertManagerDataService _dataService;

    // --- Domain List Management ---
    // ObservableCollection updates the UI automatically when items are added or removed
    public ObservableCollection<Domain> Domains { get; } = new ObservableCollection<Domain>();

    // --- New Domain Input Fields ---
    private string _newDomainName = string.Empty;
    public string NewDomainName
    {
        get => _newDomainName;
        set
        {
            if (SetProperty(ref _newDomainName, value))
            {
                // Re-evaluate the Save button state whenever the name changes
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }
    }

    private string _newAltNames = string.Empty;
    public string NewAltNames
    {
        get => _newAltNames;
        set => SetProperty(ref _newAltNames, value);
    }

    private bool _isAddingNewDomain = false;
    /// <summary>
    /// Controls the visibility of the "Add New Domain" form on the MainPage.
    /// </summary>
    public bool IsAddingNewDomain
    {
        get => _isAddingNewDomain;
        set => SetProperty(ref _isAddingNewDomain, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    // --- Commands ---
    public ICommand ToggleAddDomainCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }


    /// <summary>
    /// Constructor receives the data service via Dependency Injection.
    /// </summary>
    public MainPageViewModel(CertManagerDataService dataService)
    {
        _dataService = dataService;

        // Initialize Commands
        ToggleAddDomainCommand = new Command(() => IsAddingNewDomain = !IsAddingNewDomain);
        SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanSave);
        CancelCommand = new Command(() => IsAddingNewDomain = false);

        // As soon as the ViewModel is created, load the data
        Task.Run(LoadDomainsAsync);
    }

    /// <summary>
    /// Checks if the Save button can be executed (i.e., if the domain name is provided).
    /// </summary>
    private bool CanSave() => !string.IsNullOrWhiteSpace(NewDomainName);

    /// <summary>
    /// Executes the logic to save a new domain.
    /// </summary>
    private async Task ExecuteSaveCommand()
    {
        if (!CanSave() || IsBusy) return;

        IsBusy = true;

        try
        {
            var newDomain = new Domain
            {
                Name = NewDomainName.Trim(),
                SubjectAlternativeNames = NewAltNames.Trim(),
                // Default renewal attempt set for 30 days from now (placeholder logic)
                NextRenewalAttempt = DateTime.Now.AddDays(30)
            };

            await _dataService.AddDomainAsync(newDomain);

            // Add the new domain to the ObservableCollection for instant UI update
            Domains.Add(newDomain);

            // Clear inputs and hide the form
            NewDomainName = string.Empty;
            NewAltNames = string.Empty;
            IsAddingNewDomain = false;
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

    /// <summary>
    /// Fetches all domains from the database and populates the ObservableCollection.
    /// </summary>
    public async Task LoadDomainsAsync()
    {
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
