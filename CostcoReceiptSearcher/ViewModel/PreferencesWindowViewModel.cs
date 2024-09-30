using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CostcoReceiptSearcher.Preferences;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel;

/// <summary>
/// Represents the view model for the preferences window.
/// </summary>
public interface IPreferencesWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the general preferences.
    /// </summary>
    GeneralPreferences Preferences { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether wildcard search is allowed.
    /// </summary>
    bool AllowWildcardSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the search is case-insensitive.
    /// </summary>
    bool CaseInsensitiveSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to search in subdirectories.
    /// </summary>
    bool SearchInSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets the collection of PDF directories.
    /// </summary>
    ObservableCollection<string> PdfDirectories { get; set; }

    /// <summary>
    /// Gets or sets the selected directory.
    /// </summary>
    string SelectedDirectory { get; set; }

    /// <summary>
    /// Gets or sets the new directory.
    /// </summary>
    string NewDirectory { get; set; }

    /// <summary>
    /// Gets the command for the OK button.
    /// </summary>
    RelayCommand<ICloseableDialog> OkCommand { get; }

    /// <summary>
    /// Gets the command for the Cancel button.
    /// </summary>
    RelayCommand<ICloseableDialog> CancelCommand { get; }

    /// <summary>
    /// Gets the command for the Defaults button.
    /// </summary>
    ICommand DefaultsCommand { get; }

    /// <summary>
    /// Adds a PDF directory to the collection.
    /// </summary>
    /// <param name="directory">The directory to add.</param>
    /// <returns><c>true</c> if the directory was added successfully; otherwise, <c>false</c>.</returns>
    bool AddPdfDirectory(string directory);

    /// <summary>
    /// Removes a PDF directory from the collection.
    /// </summary>
    /// <param name="directory">The directory to remove.</param>
    /// <returns><c>true</c> if the directory was removed successfully; otherwise, <c>false</c>.</returns>
    bool RemovePdfDirectory(string directory);

    /// <summary>
    /// Loads the preferences.
    /// </summary>
    void LoadPreferences();
}

/// <summary>
/// Represents the view model for the preferences window.
/// </summary>
public class PreferencesWindowViewModel : ViewModelBase, IPreferencesWindowViewModel
{
    private readonly IPreferenceService _preferenceService;
    private bool _allowWildcardSearch;
    private bool _caseInsensitiveSearch;
    private string _newDirectory;
    private ObservableCollection<string> _pdfDirectories;
    private GeneralPreferences _preferences;
    private bool _searchInSubdirectories;
    private string _selectedDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferencesWindowViewModel"/> class.
    /// </summary>
    /// <param name="preferenceService">The preference service.</param>
    public PreferencesWindowViewModel(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService ?? throw new ArgumentNullException(nameof(preferenceService));
        _preferences = new GeneralPreferences();
        _newDirectory = string.Empty;
        _selectedDirectory = string.Empty;
        _pdfDirectories = new ObservableCollection<string>(_preferences.PdfDirectories);

        OkCommand = new RelayCommand<ICloseableDialog>(OkExecute);
        CancelCommand = new RelayCommand<ICloseableDialog>(CancelExecute);
        DefaultsCommand = new RelayCommand(DefaultsExecute);
    }

    /// <summary>
    /// Gets or sets the general preferences.
    /// </summary>
    public GeneralPreferences Preferences
    {
        get => _preferences;
        set
        {
            if (Equals(_preferences, value))
            {
                return;
            }

            LoadPreferences(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether wildcard search is allowed.
    /// </summary>
    public bool AllowWildcardSearch
    {
        get => _allowWildcardSearch;
        set
        {
            if (_allowWildcardSearch == value)
            {
                return;
            }

            _allowWildcardSearch = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the search is case-insensitive.
    /// </summary>
    public bool CaseInsensitiveSearch
    {
        get => _caseInsensitiveSearch;
        set
        {
            if (_caseInsensitiveSearch == value)
            {
                return;
            }

            _caseInsensitiveSearch = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to search in subdirectories.
    /// </summary>
    public bool SearchInSubdirectories
    {
        get => _searchInSubdirectories;
        set
        {
            if (_searchInSubdirectories == value)
            {
                return;
            }

            _searchInSubdirectories = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the collection of PDF directories.
    /// </summary>
    public ObservableCollection<string> PdfDirectories
    {
        get => _pdfDirectories;
        set
        {
            if (Equals(_pdfDirectories, value))
            {
                return;
            }

            _pdfDirectories = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the selected directory.
    /// </summary>
    public string SelectedDirectory
    {
        get => _selectedDirectory;
        set
        {
            if (_selectedDirectory == value)
            {
                return;
            }

            _selectedDirectory = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the new directory.
    /// </summary>
    public string NewDirectory
    {
        get => _newDirectory;
        set
        {
            if (_newDirectory == value)
            {
                return;
            }

            _newDirectory = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the command for the OK button.
    /// </summary>
    public RelayCommand<ICloseableDialog> OkCommand { get; }

    /// <summary>
    /// Gets the command for the Cancel button.
    /// </summary>
    public RelayCommand<ICloseableDialog> CancelCommand { get; }

    /// <summary>
    /// Gets the command for the Defaults button.
    /// </summary>
    public ICommand DefaultsCommand { get; }

    /// <summary>
    /// Adds a PDF directory to the collection.
    /// </summary>
    /// <param name="directory">The directory to add.</param>
    /// <returns><c>true</c> if the directory was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddPdfDirectory(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory) || PdfDirectories.Contains(directory))
        {
            return false;
        }

        PdfDirectories.Add(directory);
        return true;
    }

    /// <summary>
    /// Removes a PDF directory from the collection.
    /// </summary>
    /// <param name="directory">The directory to remove.</param>
    /// <returns><c>true</c> if the directory was removed successfully; otherwise, <c>false</c>.</returns>
    public bool RemovePdfDirectory(string directory)
    {
        return !string.IsNullOrWhiteSpace(directory) && PdfDirectories.Remove(directory);
    }

    /// <summary>
    /// Loads the preferences.
    /// </summary>
    public void LoadPreferences()
    {
        var preferences = _preferenceService.GetPreference<GeneralPreferences>();
        if (preferences == null)
        {
            return;
        }

        LoadPreferences(preferences);
    }

    private void LoadPreferences(GeneralPreferences preferences)
    {
        // Load the preferences
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        AllowWildcardSearch = _preferences.AllowWildcardSearch;
        CaseInsensitiveSearch = _preferences.CaseInsensitiveSearch;
        SearchInSubdirectories = _preferences.SearchInSubdirectories;
        PdfDirectories.Clear();
        PdfDirectories = new ObservableCollection<string>(_preferences.PdfDirectories);
        SelectedDirectory = string.Empty;
        OnPropertyChanged(nameof(Preferences));
    }

    private void OkExecute(ICloseableDialog closeable)
    {
        // Update the preferences
        _preferences.AllowWildcardSearch = _allowWildcardSearch;
        _preferences.CaseInsensitiveSearch = _caseInsensitiveSearch;
        _preferences.SearchInSubdirectories = _searchInSubdirectories;
        _preferences.PdfDirectories = _pdfDirectories.ToList();
        // Save the preferences
        _preferenceService.SetPreference(_preferences);
        _preferenceService.SavePreference<GeneralPreferences>();

        // Close the window
        closeable.CloseDialog(true);
    }

    private void CancelExecute(ICloseableDialog closeable)
    {
        // Discard changes
        // Close the window
        closeable.CloseDialog(false);
    }

    private void DefaultsExecute()
    {
        // Reset the preferences to their default values
        var preferences = _preferenceService.GetDefaultPreference<GeneralPreferences>();
        LoadPreferences(preferences);
    }
}