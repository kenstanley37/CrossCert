using CrossCert.ViewModels;

namespace CrossCert
{
    // CRITICAL FIX: Ensure this class is PUBLIC
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel viewModel) // Constructor is Public
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }
}