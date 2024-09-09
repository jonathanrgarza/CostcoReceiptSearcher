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
    }

    public IMainWindowViewModel ViewModel { get; }

    private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel.OpenFileCommand.Execute(null);
    }
}