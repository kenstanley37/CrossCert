using CrossCert.Models;
using CrossCert.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CrossCert.ViewModels
{
    public class AddDomainPageViewModel : INotifyPropertyChanged
    {
        private readonly CertManagerDataService _dataService;

        // Properties bound to the UI input fields
        private string _domainName = string.Empty;
        public string DomainName
        {
            get => _domainName;
            set
            {
                if (SetProperty(ref _domainName, value))
                {
                    ((Command)SaveCommand).ChangeCanExecute(); // Re-evaluate button state
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
            set => SetProperty(ref _isBusy, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddDomainPageViewModel(CertManagerDataService dataService)
        {
            _dataService = dataService;
            SaveCommand = new Command(async () => await ExecuteSaveCommand(), CanSave);
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        private bool CanSave()
        {
            // Simple validation: DomainName must not be empty
            return !string.IsNullOrWhiteSpace(DomainName);
        }

        private async Task ExecuteSaveCommand()
        {
            if (!CanSave() || IsBusy) return;

            IsBusy = true;
            try
            {
                var newDomain = new Domain
                {
                    Name = DomainName.ToLowerInvariant(),
                    SubjectAlternativeNames = AltNames.ToLowerInvariant(),
                    // Default renewal attempt set for 30 days from now (placeholder logic)
                    NextRenewalAttempt = DateTime.Now.AddDays(30)
                };

                await _dataService.AddDomainAsync(newDomain);

                // Navigate back to the main dashboard after saving
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving domain: {ex.Message}");
                // In a real app, show a message box to the user here
                await Shell.Current.DisplayAlert("Save Error", "Failed to save the domain. Check logs for details.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}