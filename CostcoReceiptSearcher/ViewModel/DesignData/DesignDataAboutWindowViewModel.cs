using System.ComponentModel;
using System.Windows.Input;
using Ncl.Common.Core.UI;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

/// <summary>
/// Represents the design-time data for the About Window view model.
/// </summary>
public class DesignDataAboutWindowViewModel : IAboutWindowViewModel
{
    /// <summary>
    /// Gets the title of the application.
    /// </summary>
    public string Title => "Costco Receipt Searcher";

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    public string Version => "1.0";

    /// <summary>
    /// Gets the description of the application.
    /// </summary>
    public string Description =>
        "This application allows you to search through Costco receipts to find the items you purchased.";

    /// <summary>
    /// Gets the author of the application.
    /// </summary>
    public string Author => "Jonathan";

    /// <summary>
    /// Gets the command for the OK button.
    /// </summary>
    public ICommand OkCommand => new RelayCommand(() => { });

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}