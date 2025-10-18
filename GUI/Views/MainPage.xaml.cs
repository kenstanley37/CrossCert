using GUI.ViewModels;

namespace GUI.Views;

public partial class MainPage : ContentPage
{

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }
}
