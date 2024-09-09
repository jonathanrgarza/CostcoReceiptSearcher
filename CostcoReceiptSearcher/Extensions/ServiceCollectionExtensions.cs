using Microsoft.Extensions.DependencyInjection;
using Ncl.Common.Core.Infrastructure;

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
}
