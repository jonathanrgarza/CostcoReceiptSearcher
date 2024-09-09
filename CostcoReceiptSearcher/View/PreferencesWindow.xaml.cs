using System.ComponentModel;
using System.Windows;
using CostcoReceiptSearcher.ViewModel;
using Microsoft.Win32;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.View;

/// <summary>
/// Interaction logic for PreferencesWindow.xaml
/// </summary>
public partial class PreferencesWindow : Window, ICloseable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesWindow"/> class.
    /// </summary>
    public PreferencesWindow(IPreferencesWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;

        InitializeComponent();

        Loaded += OnLoaded;
    }

    private IPreferencesWindowViewModel ViewModel { get; }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ViewModel.LoadPreferences();

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedDirectory))
        {

        }
    }

    private void AddPdfDirectoryClicked(object? sender, RoutedEventArgs e)
    {
        string newDirectory = ViewModel.NewDirectory;

        if (!string.IsNullOrWhiteSpace(newDirectory))
        {
            if (!ViewModel.AddPdfDirectory(newDirectory))
                MessageBox.Show(this, "The selected directory already exists in the list.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var folderDialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (folderDialog.ShowDialog(this) != true)
            return;

        string? folderName = folderDialog.FolderName;
        if (string.IsNullOrWhiteSpace(folderName))
            return;

        ViewModel.NewDirectory = folderName;
        if (!ViewModel.AddPdfDirectory(folderName))
            MessageBox.Show(this, "The selected directory already exists in the list.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void RemovePdfDirectoryClicked(object? sender, RoutedEventArgs e)
    {
        string folderName = ViewModel.SelectedDirectory;
        if (string.IsNullOrEmpty(folderName))
            return;

        if (!ViewModel.RemovePdfDirectory(folderName))
            MessageBox.Show(this, "The selected directory does not exist in the list.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }
}