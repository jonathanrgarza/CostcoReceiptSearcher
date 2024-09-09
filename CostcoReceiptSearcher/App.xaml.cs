using System.Windows;
using CostcoReceiptSearcher.Extensions;
using CostcoReceiptSearcher.Infrastructure;
using CostcoReceiptSearcher.View;
using CostcoReceiptSearcher.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ncl.Common.Core.Preferences;

namespace CostcoReceiptSearcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
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

        // Add the other windows
        services.AddTransient<IAboutWindowViewModel, AboutWindowViewModel>();
        services.AddGenericFactory<AboutWindow>();

        services.AddTransient<IPreferencesWindowViewModel, PreferencesWindowViewModel>();
        services.AddGenericFactory<PreferencesWindow>();

        // Add the main window
        services.AddTransient<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}