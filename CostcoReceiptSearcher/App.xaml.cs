using System.Windows;
using CostcoReceiptSearcher.Extensions;
using CostcoReceiptSearcher.Infrastructure;
using CostcoReceiptSearcher.View;
using CostcoReceiptSearcher.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        AppHost = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
    }

    public static IHost? AppHost { get; private set; }

    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        // Add preferences service
        services.AddSingleton<PreferenceService>(); // Explicitly register the preference service so we can "forward" it
        // Forward the preference service as the two interfaces it implements
        services.AddSingleton<IPreferenceService>(x => x.GetRequiredService<PreferenceService>());
        services.AddSingleton<ICustomizablePreferenceService>(x => x.GetRequiredService<PreferenceService>());

        // Add the window manager
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<IDialogService, DialogService>();

        // Add the other windows
        services.AddViewModelFactory<IAboutWindowViewModel, AboutWindowViewModel>();
        services.AddWindowFactory<AboutWindow>();

        services.AddViewModelFactory<IPreferencesWindowViewModel, PreferencesWindowViewModel>();
        services.AddWindowFactory<PreferencesWindow>();

        // Add the main window
        services.AddTransient<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var services = AppHost.Services;
        // Setup window manager
        var windowManager = SetupWindowManagerRegistrations(services);

        // Setup dialog registrations
        SetupDialogManagerRegistrations(services);

        // Show the main window
        var mainWindow = services.GetRequiredService<MainWindow>();
        windowManager.ShowWindow(mainWindow);

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }

    private static IWindowManager SetupWindowManagerRegistrations(IServiceProvider services)
    {
        var windowManager = services.GetRequiredService<IWindowManager>();

        // Register the windows

        return windowManager;
    }

    private static void SetupDialogManagerRegistrations(IServiceProvider services)
    {
        var dialogService = (DialogService)services.GetRequiredService<IDialogService>();

        // Register the dialogs
        dialogService.Register<IAboutWindowViewModel>(services.GetRequiredService<IWindowFactory<AboutWindow>>());
        dialogService.Register<IPreferencesWindowViewModel>(services
            .GetRequiredService<IWindowFactory<PreferencesWindow>>());
    }
}