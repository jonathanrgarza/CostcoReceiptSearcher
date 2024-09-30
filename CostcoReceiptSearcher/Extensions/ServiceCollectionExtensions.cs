using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Ncl.Common.Core.Infrastructure;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a generic factory to the service collection.
    /// </summary>
    /// <typeparam name="T">The type created by the factory.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the factory to.</param>
    public static void AddGenericFactory<T>(this IServiceCollection services)
        where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(x => () => x.GetService<T>()!);
        services.AddSingleton<IGenericFactory<T>, GenericFactory<T>>();
    }

    /// <summary>
    /// Adds a view model factory to the service collection.
    /// </summary>
    /// <typeparam name="TService">The type created by the factory.</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the factory to.</param>
    public static void AddViewModelFactory<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TService, TImplementation>();
        services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>()!);
        services.AddSingleton<IGenericFactory<TService>, GenericFactory<TService>>();
    }

    /// <summary>
    /// Adds a window factory to the service collection.
    /// </summary>
    /// <typeparam name="T">The type created by the factory.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the factory to.</param>
    public static void AddWindowFactory<T>(this IServiceCollection services)
        where T : Window
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(x => () => x.GetService<T>()!);
        services.AddSingleton<IWindowFactory<T>, WindowFactory<T>>();
    }
}
