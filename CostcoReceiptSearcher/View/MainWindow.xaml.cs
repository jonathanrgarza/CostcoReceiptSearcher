using System.Windows;
using System.Windows.Input;
using CostcoReceiptSearcher.ViewModel;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, ICloseable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public MainWindow(IMainWindowViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        DataContext = viewModel;

        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    /// <summary>
    /// Gets the view model.
    /// </summary>
    private IMainWindowViewModel ViewModel { get; }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnLoaded();
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OpenFileCommand.Execute(null);
    }
}