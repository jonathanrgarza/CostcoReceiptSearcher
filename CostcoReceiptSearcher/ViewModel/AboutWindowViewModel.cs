using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel;

/// <summary>
/// Represents the view model for the About Window.
/// </summary>
public interface IAboutWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the title of the application.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the author of the application.
    /// </summary>
    string Author { get; }

    /// <summary>
    /// Gets the command to close the About Window.
    /// </summary>
    ICommand OkCommand { get; }
}

/// <summary>
/// Represents the view model for the About Window.
/// </summary>
public class AboutWindowViewModel : ViewModelBase, IAboutWindowViewModel
{
    /// <summary>
    /// Gets the title of the application.
    /// </summary>
    public string Title
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            return titleAttribute?.Title ?? "Unknown Title";
        }
    }

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    public string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "Unknown Version";
        }
    }

    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    public string Description
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return descriptionAttribute?.Description ?? "No Description";
        }
    }

    /// <summary>
    /// Gets the author of the application.
    /// </summary>
    public string Author
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return descriptionAttribute?.Company ?? "No Author Provided";
        }
    }

    /// <summary>
    /// Gets the command to close the About Window.
    /// </summary>
    public ICommand OkCommand { get; } = new RelayCommand<ICloseable>(OkExecute);

    private static void OkExecute(ICloseable obj)
    {
        obj.Close();
    }
}