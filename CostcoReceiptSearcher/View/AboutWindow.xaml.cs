using System.Windows;
using CostcoReceiptSearcher.ViewModel;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.View;

public partial class AboutWindow : Window, ICloseable
{
    public AboutWindow(IAboutWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();
    }

    public IAboutWindowViewModel ViewModel { get; }
}