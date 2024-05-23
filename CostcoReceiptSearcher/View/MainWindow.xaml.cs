using System.Windows;
using System.Windows.Input;
using CostcoReceiptSearcher.ViewModel;

namespace CostcoReceiptSearcher.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var viewModel = (MainWindowViewModel)DataContext;
        viewModel.OpenFileCommand.Execute(null);
    }
}