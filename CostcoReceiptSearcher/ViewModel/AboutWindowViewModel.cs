using System.Reflection;
using System.Windows.Input;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel;

public interface IAboutWindowViewModel
{
    string Title { get; }
    string Version { get; }
    string Description { get; }
    string Author { get; }
    ICommand OkCommand { get; }
}

public class AboutWindowViewModel : ViewModelBase, IAboutWindowViewModel
{
    public string Title
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            return titleAttribute?.Title ?? "Unknown Title";
        }
    }

    public string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "Unknown Version";
        }
    }

    public string Description
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            return descriptionAttribute?.Description ?? "No Description";
        }
    }

    public string Author
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var descriptionAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return descriptionAttribute?.Company ?? "No Author Provided";
        }
    }

    public ICommand OkCommand { get; } = new RelayCommand<ICloseable>(OkExecute);

    private static void OkExecute(ICloseable obj)
    {
        obj.Close();
    }
}