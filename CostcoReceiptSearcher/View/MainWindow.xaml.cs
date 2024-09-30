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
    public MainWindow(IMainWindowViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        DataContext = viewModel;

        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnLoaded();
    }

    public IMainWindowViewModel ViewModel { get; }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OpenFileCommand.Execute(null);
    }
}