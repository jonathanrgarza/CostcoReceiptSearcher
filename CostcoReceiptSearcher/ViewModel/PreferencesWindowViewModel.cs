using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CostcoReceiptSearcher.Preferences;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel;

public interface IPreferencesWindowViewModel : INotifyPropertyChanged
{
    GeneralPreferences Preferences { get; set; }
    bool AllowWildcardSearch { get; set; }
    bool CaseInsensitiveSearch { get; set; }
    bool SearchInSubdirectories { get; set; }
    ObservableCollection<string> PdfDirectories { get; set; }
    string SelectedDirectory { get; set; }
    string NewDirectory { get; set; }
    RelayCommand<ICloseable> OkCommand { get; }
    RelayCommand<ICloseable> CancelCommand { get; }
    ICommand DefaultsCommand { get; }
    bool AddPdfDirectory(string directory);
    bool RemovePdfDirectory(string directory);
    void LoadPreferences();
}

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


    public PreferencesWindowViewModel(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService ?? throw new ArgumentNullException(nameof(preferenceService));
        _preferences = new GeneralPreferences();
        _newDirectory = string.Empty;
        _selectedDirectory = string.Empty;
        _pdfDirectories = new ObservableCollection<string>(_preferences.PdfDirectories);

        OkCommand = new RelayCommand<ICloseable>(OkExecute);
        CancelCommand = new RelayCommand<ICloseable>(CancelExecute);
        DefaultsCommand = new RelayCommand(DefaultsExecute);
    }

    public PreferencesWindowViewModel(IPreferenceService preferenceService, GeneralPreferences preferences) : this(
        preferenceService)
    {
        LoadPreferences(preferences);
    }

    public GeneralPreferences Preferences
    {
        get => _preferences;
        set
        {
            if (Equals(value, _preferences))
                return;
            LoadPreferences(value);
        }
    }

    public bool AllowWildcardSearch
    {
        get => _allowWildcardSearch;
        set
        {
            if (value == _allowWildcardSearch)
                return;
            _allowWildcardSearch = value;
            OnPropertyChanged();
        }
    }

    public bool CaseInsensitiveSearch
    {
        get => _caseInsensitiveSearch;
        set
        {
            if (value == _caseInsensitiveSearch)
                return;
            _caseInsensitiveSearch = value;
            OnPropertyChanged();
        }
    }

    public bool SearchInSubdirectories
    {
        get => _searchInSubdirectories;
        set
        {
            if (value == _searchInSubdirectories)
                return;
            _searchInSubdirectories = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> PdfDirectories
    {
        get => _pdfDirectories;
        set
        {
            if (Equals(value, _pdfDirectories))
                return;
            _pdfDirectories = value;
            OnPropertyChanged();
        }
    }

    public string SelectedDirectory
    {
        get => _selectedDirectory;
        set
        {
            if (value == _selectedDirectory)
                return;
            _selectedDirectory = value;
            OnPropertyChanged();
        }
    }

    public string NewDirectory
    {
        get => _newDirectory;
        set
        {
            if (value == _newDirectory)
                return;
            _newDirectory = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand<ICloseable> OkCommand { get; }
    public RelayCommand<ICloseable> CancelCommand { get; }
    public ICommand DefaultsCommand { get; }

    public bool AddPdfDirectory(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory) || PdfDirectories.Contains(directory))
            return false;

        PdfDirectories.Add(directory);
        return true;
    }

    public bool RemovePdfDirectory(string directory)
    {
        return !string.IsNullOrWhiteSpace(directory) && PdfDirectories.Remove(directory);
    }

    public void LoadPreferences()
    {
        var preferences = _preferenceService.GetPreference<GeneralPreferences>();
        if (preferences == null)
            return;
        LoadPreferences(preferences);
    }

    private void LoadPreferences(GeneralPreferences preferences)
    {
        // Load the preferences
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        _pdfDirectories.Clear();
        _pdfDirectories = new ObservableCollection<string>(_preferences.PdfDirectories);
        SelectedDirectory = string.Empty;
        OnPropertyChanged(nameof(Preferences));
    }

    private void OkExecute(ICloseable closeable)
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
        var preferences = _preferenceService.GetDefaultPreference<GeneralPreferences>();
        LoadPreferences(preferences);
    }
}