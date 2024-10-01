using System.ComponentModel;
using System.Windows.Input;
using Ncl.Common.Core.UI;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

/// <summary>
/// Represents the design-time data for the About Window view model.
/// </summary>
public class DesignDataAboutWindowViewModel : IAboutWindowViewModel
{
    /// <inheritdoc/>
    public string Title => "Costco Receipt Searcher";

    /// <inheritdoc/>
    public string Version => "1.0";

    /// <inheritdoc/>
    public string Description =>
        "This application allows you to search through Costco receipts to find the items you purchased.";

    /// <inheritdoc/>
    public string Author => "Jonathan";

    /// <inheritdoc/>
    public ICommand OkCommand => new RelayCommand(() => { });

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}