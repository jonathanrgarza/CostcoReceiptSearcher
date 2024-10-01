using System.Windows;
using CostcoReceiptSearcher.ViewModel;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.View;

/// <summary>
/// Represents the AboutWindow in the application.
/// </summary>
public partial class AboutWindow : Window, ICloseable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model for the AboutWindow.</param>
    public AboutWindow(IAboutWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();
    }

    /// <summary>
    /// Gets the view model for the AboutWindow.
    /// </summary>
    private IAboutWindowViewModel ViewModel { get; }
}
