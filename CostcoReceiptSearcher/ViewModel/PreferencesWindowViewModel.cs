using System.Windows;
using System.Windows.Input;
using CostcoReceiptSearcher.Preferences;
using Microsoft.Win32;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel;

public class PreferencesWindowViewModel : ViewModelBase
{
    private GeneralPreferences _preferences;

    public GeneralPreferences Preferences
    {
        get => _preferences;
        set
        {
            if (Equals(value, _preferences))
                return;
            _preferences = value;
            OnPropertyChanged();
        }
    }

    public string SelectedDirectory { get; set; }
    public string NewDirectory { get; set; }

    public ICommand AddDirectoryCommand { get; }
    public ICommand RemoveDirectoryCommand { get; }

    public RelayCommand<ICloseable> OkCommand { get; }
    public RelayCommand<ICloseable> CancelCommand { get; }
    public ICommand DefaultsCommand { get; }

    public PreferencesWindowViewModel()
    {
        _preferences = new GeneralPreferences();
        SelectedDirectory = string.Empty;
        NewDirectory = string.Empty;

        OkCommand = new RelayCommand<ICloseable>(OkExecute);
        CancelCommand = new RelayCommand<ICloseable>(CancelExecute);
        DefaultsCommand = new RelayCommand(DefaultsExecute);
        AddDirectoryCommand = new RelayCommand(AddDirectoryExecute);
        RemoveDirectoryCommand = new RelayCommand(RemoveDirectoryExecute);
    }

    private void AddDirectoryExecute()
    {
        var folderDialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (folderDialog.ShowDialog() != true)
            return;

        string? folderName = folderDialog.FolderName;
        if (string.IsNullOrWhiteSpace(folderName))
            return;

        if (Preferences.PdfDirectories.Contains(folderName))
        {
            MessageBox.Show("The directory is already in the list.", "Duplicate Directory", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        Preferences.PdfDirectories.Add(folderName);
        OnPropertyChanged(nameof(Preferences));
    }

    private void RemoveDirectoryExecute()
    {
        if (string.IsNullOrWhiteSpace(SelectedDirectory)) return;

        if (Preferences.PdfDirectories.Remove(SelectedDirectory))
        {
            OnPropertyChanged(nameof(Preferences));
        }
    }

    private void OkExecute(ICloseable closeable)
    {
        // Save the preferences
        // Close the window
        closeable.Close();
    }

    private void CancelExecute(ICloseable closeable)
    {
        // Discard changes
        // Close the window
        closeable.Close();
    }

    private void DefaultsExecute()
    {
        // Reset the preferences to their default values
    }
}