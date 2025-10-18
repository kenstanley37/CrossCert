using GUI.ViewModels;

namespace GUI;

public partial class AppShell : Shell
{
    public AppShell(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }
}
