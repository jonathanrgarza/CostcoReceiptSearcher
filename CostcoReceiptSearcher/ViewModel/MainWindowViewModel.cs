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

public interface IMainWindowViewModel : INotifyPropertyChanged
{
    string SearchText { get; set; }
    ObservableCollection<PdfFile> MatchingPdfFiles { get; set; }
    int MatchCount { get; }
    public bool IsSearching { get; }
    PdfFile? SelectedPdfFile { get; set; }
    ICommand MenuExitCommand { get; }
    ICommand MenuPreferencesCommand { get; }
    ICommand MenuAboutCommand { get; }
    ICommand SearchCommand { get; }
    ICommand OpenFileCommand { get; }
    ICommand OpenFolderCommand { get; }

    void OnLoaded();
}

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    private readonly IDialogService _dialogService;
    private readonly Dictionary<string, PdfFile?> _pdfFiles = new();
    private readonly IPreferenceService _preferenceService;

    private GeneralPreferences _generalPreferences;
    private bool _isSearching;
    private ObservableCollection<PdfFile> _matchingPdfFiles = [];

    private string _searchText = string.Empty;
    private PdfFile? _selectedPdfFile;

    private int _totalFilesSearched;

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

    public int MatchCount => MatchingPdfFiles.Count;

    public PdfFile? SelectedPdfFile
    {
        get => _selectedPdfFile;
        set
        {
            if (Equals(value, _selectedPdfFile))
            {
                return;
            }

            _selectedPdfFile = value;
            OnPropertyChanged();
        }
    }

    public bool IsSearching
    {
        get => _isSearching;
        set
        {
            if (_isSearching != value)
            {
                _isSearching = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand MenuExitCommand { get; }

    public ICommand MenuPreferencesCommand { get; }

    public ICommand MenuAboutCommand { get; }

    public ICommand SearchCommand { get; }

    public ICommand OpenFileCommand { get; }

    public ICommand OpenFolderCommand { get; }

    public void OnLoaded()
    {
        _generalPreferences = _preferenceService.GetPreference<GeneralPreferences>();
        _preferenceService.PreferenceChanged += OnPreferenceChanged;
    }

    private void OnMatchingPdfFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        OnPropertyChanged(nameof(MatchCount));
    }

    private void OnPreferenceChanged(object? sender, PreferenceChangedEventArgs e)
    {
        if (e.Type == typeof(GeneralPreferences))
        {
            _generalPreferences = (GeneralPreferences)e.NewValue;
        }
    }

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
                byte[] fileHash = GetFileHash(pdfFile);
                // Debugging: Print the hashes
                if (pdfFile.FileHash.Length == 0 || !pdfFile.FileHash.SequenceEqual(fileHash))
                {
                    ReadPdfFile(pdfFile);
                }

                // Search the PDF file
                if (PdfContainsSearchText(pdfFile, searchText, comparison, regex))
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

    private static void ReadPdfFile(PdfFile pdfFile)
    {
        // Implement the logic to read the PDF file and return the lines
        if (!File.Exists(pdfFile.FilePath))
        {
            return;
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

        pdfFile.Lines = lines.ToArray();
        pdfFile.FileHash = GetFileHash(pdfFile);
    }

    private static bool PdfContainsSearchText(PdfFile pdfFile, string searchText, StringComparison comparison,
        Regex? regex)
    {
        if (pdfFile.Lines == null)
        {
            return false;
        }

        return regex == null
            ? pdfFile.Lines.Any(line => line.Contains(searchText, comparison))
            : pdfFile.Lines.Any(regex.IsMatch);
    }

    private void OpenFileExecute()
    {
        if (_selectedPdfFile == null)
        {
            return;
        }

        Process.Start(new ProcessStartInfo(_selectedPdfFile.FilePath) { UseShellExecute = true });
    }

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