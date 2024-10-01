using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CostcoReceiptSearcher.Model;
using CostcoReceiptSearcher.Preferences;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;
using Ncl.Common.Wpf.ViewModels;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace CostcoReceiptSearcher.ViewModel;

/// <summary>
/// Represents interface for the view model for the main window of the application.
/// </summary>
public interface IMainWindowViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the total number of files searched during the search operation.
    /// </summary>
    int TotalFilesSearched { get; set; }

    /// <summary>
    /// Gets or sets the text used for searching within the PDF files.
    /// </summary>
    string SearchText { get; set; }

    /// <summary>
    /// Gets or sets the collection of PDF files that match the search criteria.
    /// </summary>
    ObservableCollection<PdfFile> MatchingPdfFiles { get; set; }

    /// <summary>
    /// Gets the number of PDF files that match the search criteria.
    /// </summary>
    int MatchCount { get; }

    /// <summary>
    /// Gets a value indicating whether the application is currently searching for matching PDF files.
    /// </summary>
    bool IsSearching { get; }

    /// <summary>
    /// Gets or sets the currently selected PDF file.
    /// </summary>
    PdfFile? SelectedPdfFile { get; set; }

    /// <summary>
    /// Gets the command for exiting the application.
    /// </summary>
    ICommand MenuExitCommand { get; }

    /// <summary>
    /// Gets the command for opening the preferences window.
    /// </summary>
    ICommand MenuPreferencesCommand { get; }

    /// <summary>
    /// Gets the command for opening the about window.
    /// </summary>
    ICommand MenuAboutCommand { get; }

    /// <summary>
    /// Gets the command for executing the search operation.
    /// </summary>
    ICommand SearchCommand { get; }

    /// <summary>
    /// Gets the command for opening a PDF file.
    /// </summary>
    ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets the command for opening the folder containing a PDF file.
    /// </summary>
    ICommand OpenFolderCommand { get; }

    /// <summary>
    /// Performs initialization tasks when the main window is loaded.
    /// </summary>
    void OnLoaded();
}

/// <summary>
/// Represents the view model for the main window of the application.
/// </summary>
public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    /// <summary>
    /// The dialog service used for displaying dialogs.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// The dictionary that stores the PDF files and their corresponding file path.
    /// </summary>
    private readonly Dictionary<string, PdfFile?> _pdfFiles = new();

    /// <summary>
    /// The preference service used for managing application preferences.
    /// </summary>
    private readonly IPreferenceService _preferenceService;

    /// <summary>
    /// The general preferences of the application.
    /// </summary>
    private GeneralPreferences _generalPreferences;

    /// <summary>
    /// Indicates whether the application is currently searching for matching PDF files.
    /// </summary>
    private bool _isSearching;

    /// <summary>
    /// The collection of PDF files that match the search criteria.
    /// </summary>
    private ObservableCollection<PdfFile> _matchingPdfFiles = [];

    /// <summary>
    /// The text used for searching within the PDF files.
    /// </summary>
    private string _searchText = string.Empty;

    /// <summary>
    /// The currently selected PDF file.
    /// </summary>
    private PdfFile? _selectedPdfFile;

    /// <summary>
    /// The total number of files searched during the search operation.
    /// </summary>
    private int _totalFilesSearched;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="dialogService">The dialog service used for displaying dialogs.</param>
    /// <param name="preferenceService">The preference service used for managing application preferences.</param>
    public MainWindowViewModel(IDialogService dialogService, IPreferenceService preferenceService)
    {
        _dialogService = dialogService;
        _preferenceService = preferenceService;

        _generalPreferences = new GeneralPreferences();

        SearchCommand = new RelayCommandAsync(SearchExecute);
        OpenFileCommand = new RelayCommand(OpenFileExecute);
        OpenFolderCommand = new RelayCommand(OpenFolderExecute);
        MenuExitCommand = new RelayCommand(() => Application.Current.Shutdown());
        MenuPreferencesCommand = new RelayCommand(() => { _dialogService.ShowDialog<IPreferencesWindowViewModel>(); });
        MenuAboutCommand = new RelayCommand(() => { _dialogService.ShowDialog<IAboutWindowViewModel>(); });

        MatchingPdfFiles.CollectionChanged += OnMatchingPdfFilesOnCollectionChanged;
    }

    /// <summary>
    /// Gets or sets the total number of files searched during the search operation.
    /// </summary>
    public int TotalFilesSearched
    {
        get => _totalFilesSearched;
        set
        {
            if (_totalFilesSearched == value) return;

            _totalFilesSearched = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the text used for searching within the PDF files.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (Equals(value, _searchText))
            {
                return;
            }

            _searchText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the collection of PDF files that match the search criteria.
    /// </summary>
    public ObservableCollection<PdfFile> MatchingPdfFiles
    {
        get => _matchingPdfFiles;
        set
        {
            if (Equals(value, _matchingPdfFiles))
            {
                return;
            }

            _matchingPdfFiles = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MatchCount));
        }
    }

    /// <summary>
    /// Gets the number of PDF files that match the search criteria.
    /// </summary>
    public int MatchCount => MatchingPdfFiles.Count;

    /// <summary>
    /// Gets or sets the currently selected PDF file.
    /// </summary>
    public PdfFile? SelectedPdfFile
    {
        get => _selectedPdfFile;
        set
        {
            if (Equals(value, _selectedPdfFile)) return;

            _selectedPdfFile = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application is currently searching for matching PDF files.
    /// </summary>
    public bool IsSearching
    {
        get => _isSearching;
        set
        {
            if (_isSearching == value) return;
            _isSearching = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the command for exiting the application.
    /// </summary>
    public ICommand MenuExitCommand { get; }

    /// <summary>
    /// Gets the command for opening the preferences window.
    /// </summary>
    public ICommand MenuPreferencesCommand { get; }

    /// <summary>
    /// Gets the command for opening the about window.
    /// </summary>
    public ICommand MenuAboutCommand { get; }

    /// <summary>
    /// Gets the command for executing the search operation.
    /// </summary>
    public ICommand SearchCommand { get; }

    /// <summary>
    /// Gets the command for opening a PDF file.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets the command for opening the folder containing a PDF file.
    /// </summary>
    public ICommand OpenFolderCommand { get; }

    /// <summary>
    /// Performs initialization tasks when the main window is loaded.
    /// </summary>
    public void OnLoaded()
    {
        _generalPreferences = _preferenceService.GetPreference<GeneralPreferences>();
        _preferenceService.PreferenceChanged += OnPreferenceChanged;
    }

    /// <summary>
    /// Handles the collection changed event of the matching PDF files.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The event arguments.</param>
    private void OnMatchingPdfFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        OnPropertyChanged(nameof(MatchCount));
    }

    /// <summary>
    /// Handles the preference changed event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPreferenceChanged(object? sender, PreferenceChangedEventArgs e)
    {
        if (e.Type != typeof(GeneralPreferences)) return;

        var newPreferences = (GeneralPreferences)e.NewValue;
        _generalPreferences = newPreferences;

        // Check if the caching preference has changed
        if (newPreferences.EnableCaching == ((GeneralPreferences)e.OldValue).EnableCaching) return;

        foreach (var pdfFile in _pdfFiles)
        {
            if (pdfFile.Value == null) continue;
            pdfFile.Value.Lines = null;
            pdfFile.Value.FileHash = [];
        }
    }

    /// <summary>
    /// Executes the search operation asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SearchExecute()
    {
        // Implement the logic to search for the text string within the PDF files
        // Add the matching files to the PdfFiles collection
        string searchText = SearchText;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            _dialogService.ShowDialog(
                MessageBoxDialogViewModel.CreateErrorDialog("Please enter a search string.", "Search Error"));
            return;
        }

        // Clear the existing list of matching PDF files
        MatchingPdfFiles.Clear();

        IsSearching = true;
        await Task.Run(ProcessPdfFiles);
        IsSearching = false;
    }

    /// <summary>
    /// Processes the PDF files and searches for the specified text.
    /// </summary>
    private void ProcessPdfFiles()
    {
        string searchText = SearchText;
        var comparison = StringComparison.Ordinal;
        if (_generalPreferences.CaseInsensitiveSearch)
        {
            comparison = StringComparison.OrdinalIgnoreCase;
        }

        var directorySearchOption = SearchOption.AllDirectories;
        if (!_generalPreferences.SearchInSubdirectories)
        {
            directorySearchOption = SearchOption.TopDirectoryOnly;
        }

        bool allowWildcardSearch = _generalPreferences.AllowWildcardSearch;
        bool enableCaching = _generalPreferences.EnableCaching;

        Regex? regex = null;
        if (allowWildcardSearch)
        {
            // Convert wildcard search text to regex pattern
            string patternStart = ".*";
            string patternEnd = ".*";
            if (searchText.StartsWith('^'))
            {
                patternStart = "^";
            }

            if (searchText.EndsWith('$'))
            {
                patternEnd = "$";
            }

            string regexPattern = patternStart + Regex.Escape(searchText).Replace("\\*", ".*") + patternEnd;
            var options = comparison == StringComparison.OrdinalIgnoreCase
                ? RegexOptions.IgnoreCase
                : RegexOptions.None;
            regex = new Regex(regexPattern, options);
        }

        int totalFilesSearched = 0;
        TotalFilesSearched = 0;

        // Perform the search
        foreach (string pdfDirectory in _generalPreferences.PdfDirectories)
        {
            // Get all the PDF files in the directory
            string[] pdfFiles = Directory.GetFiles(pdfDirectory, "*.pdf", directorySearchOption);

            // Read the PDF file
            foreach (string pdfFilePath in pdfFiles)
            {
                // Check if we have already processed this file
                _pdfFiles.TryGetValue(pdfFilePath, out var pdfFile);

                if (pdfFile == null)
                {
                    // Would do if statement on TryGetValue but VS doesn't see it as a null check
                    pdfFile = new PdfFile(pdfFilePath);
                    _pdfFiles.Add(pdfFilePath, pdfFile);
                }

                // Read PDF file if the hash is empty or different
                string[]? lines = pdfFile.Lines;
                byte[] fileHash = GetFileHash(pdfFile);
                if (!enableCaching || pdfFile.FileHash.Length == 0 || !pdfFile.FileHash.SequenceEqual(fileHash))
                {
                    lines = ReadPdfFile(pdfFile, enableCaching);
                }

                // Search the PDF file
                if (PdfContainsSearchText(lines, searchText, comparison, regex))
                {
                    int searched = totalFilesSearched;
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MatchingPdfFiles.Add(pdfFile);
                        // Update the total files searched to show progress
                        TotalFilesSearched = searched;
                    });
                }

                totalFilesSearched++;
            }
        }

        TotalFilesSearched = totalFilesSearched;
    }

    /// <summary>
    /// Gets the hash of the specified PDF file.
    /// </summary>
    /// <param name="pdfFile">The PDF file.</param>
    /// <returns>The hash of the PDF file.</returns>
    private static byte[] GetFileHash(PdfFile pdfFile)
    {
        // Implement the logic to get the hash of the PDF file
        if (!File.Exists(pdfFile.FilePath))
        {
            return [];
        }

        using var stream = File.OpenRead(pdfFile.FilePath);
        using var sha = SHA256.Create();

        byte[] hash = sha.ComputeHash(stream);
        return hash;
    }

    /// <summary>
    /// Reads the specified PDF file and extracts the lines of text.
    /// </summary>
    /// <param name="pdfFile">The PDF file.</param>
    /// <param name="enableCaching">Should the PDF content be cached.</param>
    /// <returns>The PDF content split into lines.</returns>
    private static string[] ReadPdfFile(PdfFile pdfFile, bool enableCaching)
    {
        // Implement the logic to read the PDF file and return the lines
        if (!File.Exists(pdfFile.FilePath))
        {
            return [];
        }

        // Read the PDF file
        using var document = PdfDocument.Open(pdfFile.FilePath);
        int pageCount = document.NumberOfPages;

        List<string> lines = [];
        // Page number starts from 1, not 0.
        for (int i = 1; i <= pageCount; i++)
        {
            var page = document.GetPage(i);

            //string text = page.Text;
            string text = ContentOrderTextExtractor.GetText(page, true);
            lines.AddRange(text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
        }

        string[] lineArray = lines.ToArray();
        if (!enableCaching) return lineArray;

        pdfFile.Lines = lineArray;
        pdfFile.FileHash = GetFileHash(pdfFile);

        return lineArray;
    }

    /// <summary>
    /// Checks if the specified PDF lines contains the search text.
    /// </summary>
    /// <param name="lines">The lines of a PDF file.</param>
    /// <param name="searchText">The search text.</param>
    /// <param name="comparison">The string comparison method.</param>
    /// <param name="regex">The regular expression for wildcard search.</param>
    /// <returns><c>true</c> if the PDF lines contains the search text; otherwise, <c>false</c>.</returns>
    private static bool PdfContainsSearchText(string[]? lines, string searchText, StringComparison comparison,
        Regex? regex)
    {
        if (lines == null) return false;

        return regex == null
            ? lines.Any(line => line.Contains(searchText, comparison))
            : lines.Any(regex.IsMatch);
    }

    /// <summary>
    /// Opens the currently selected PDF file.
    /// </summary>
    private void OpenFileExecute()
    {
        if (_selectedPdfFile == null)
        {
            return;
        }

        Process.Start(new ProcessStartInfo(_selectedPdfFile.FilePath) { UseShellExecute = true });
    }

    /// <summary>
    /// Opens the folder containing the currently selected PDF file.
    /// </summary>
    private void OpenFolderExecute()
    {
        if (_selectedPdfFile == null)
        {
            return;
        }

        string argument = "/select, \"" + _selectedPdfFile.FilePath + "\"";
        Process.Start("explorer.exe", argument);
    }
}