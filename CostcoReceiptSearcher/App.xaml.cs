using System.Reflection;
using System.Windows;
using CostcoReceiptSearcher.Extensions;
using CostcoReceiptSearcher.Infrastructure;
using CostcoReceiptSearcher.Preferences;
using CostcoReceiptSearcher.View;
using CostcoReceiptSearcher.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Wpf.Infrastructure;
using NLog.Extensions.Logging;

namespace CostcoReceiptSearcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly ILogger<App> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        AppHost = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();

        _logger = AppHost.Services.GetRequiredService<ILogger<App>>();
    }

    /// <summary>
    /// Gets the application host.
    /// </summary>
    public static IHost? AppHost { get; private set; }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    /// <param name="hostContext">The host builder context.</param>
    /// <param name="services">The service collection.</param>
    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    {
        // Add the logger
        services.AddLogging(builder =>
        {
            // configure Logging with NLog
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog();
        });

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

    /// <summary>
    /// Handles the startup event of the application.
    /// </summary>
    /// <param name="e">The startup event arguments.</param>
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            await AppHost!.StartAsync();

            var services = AppHost.Services;

            //Setup unhandled exception handling
            SetupUnhandledExceptionHandling();

            // Setup window manager
            var windowManager = SetupWindowManagerRegistrations(services);

            // Setup dialog registrations
            SetupDialogManagerRegistrations(services);

            // Setup preference service
            SetupPreferenceService(services);

            // Show the main window
            var mainWindow = services.GetRequiredService<MainWindow>();
            windowManager.ShowWindow(mainWindow);

            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application startup.");
            Shutdown();
        }
    }

    /// <summary>
    /// Handles the exit event of the application.
    /// </summary>
    /// <param name="e">The exit event arguments.</param>
    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application startup.");
            Shutdown();
        }
    }

    /// <summary>
    /// Sets up the window manager registrations.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>The window manager instance.</returns>
    private static IWindowManager SetupWindowManagerRegistrations(IServiceProvider services)
    {
        var windowManager = services.GetRequiredService<IWindowManager>();

        // Register the windows

        return windowManager;
    }

    /// <summary>
    /// Sets up the dialog manager registrations.
    /// </summary>
    /// <param name="services">The service provider.</param>
    private static void SetupDialogManagerRegistrations(IServiceProvider services)
    {
        var dialogService = (DialogService)services.GetRequiredService<IDialogService>();

        // Register the dialogs
        dialogService.Register<IAboutWindowViewModel>(services.GetRequiredService<IWindowFactory<AboutWindow>>());
        dialogService.Register<IPreferencesWindowViewModel>(services
            .GetRequiredService<IWindowFactory<PreferencesWindow>>());
    }

    /// <summary>
    /// Sets up the preference service.
    /// </summary>
    /// <param name="services">The service provider.</param>
    private static void SetupPreferenceService(IServiceProvider services)
    {
        var preferenceService = services.GetRequiredService<ICustomizablePreferenceService>();

        // Register the preferences
        preferenceService.RegisterDefaultPreferences([new GeneralPreferences()]);
    }

    private void SetupUnhandledExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        DispatcherUnhandledException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
    }

    private void LogUnhandledException(Exception ex, string method)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        _logger.LogCritical(ex, "Unhandled Exception occured in {assembly} via {method}", assemblyName.Name, method);
    }
}