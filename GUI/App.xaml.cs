namespace GUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = MauiProgram.ServiceProvider!.GetService<AppShell>();

        return shell is null
            ? throw new InvalidOperationException("AppShell is not registered in the service container.")
            : new Window(shell);


        //return new Window(new AppShell());
    }
}