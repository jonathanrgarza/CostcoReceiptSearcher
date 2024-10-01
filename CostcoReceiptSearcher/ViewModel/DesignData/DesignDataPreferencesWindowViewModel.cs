using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CostcoReceiptSearcher.Preferences;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

/// <summary>
/// Represents the view model for the preferences window in design mode.
/// </summary>
internal class DesignDataPreferencesWindowViewModel : IPreferencesWindowViewModel
{
    /// <summary>
    /// Gets or sets the general preferences.
    /// </summary>
    public GeneralPreferences Preferences { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether wildcard search is allowed.
    /// </summary>
    public bool AllowWildcardSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the search is case-insensitive.
    /// </summary>
    public bool CaseInsensitiveSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the search should be performed in subdirectories.
    /// </summary>
    public bool SearchInSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets the collection of PDF directories.
    /// </summary>
    public ObservableCollection<string> PdfDirectories { get; set; } =
    [
        "C:\\PDFs",
        "D:\\Other_PDFs"
    ];

    /// <summary>
    /// Gets or sets the selected directory.
    /// </summary>
    public string SelectedDirectory { get; set; } = "C:\\PDF";

    /// <summary>
    /// Gets or sets the new directory.
    /// </summary>
    public string NewDirectory { get; set; } = "E:\\PDFs";

    /// <summary>
    /// Gets the command for the OK button.
    /// </summary>
    public RelayCommand<ICloseableDialog> OkCommand { get; } = new(_ => { });

    /// <summary>
    /// Gets the command for the Cancel button.
    /// </summary>
    public RelayCommand<ICloseableDialog> CancelCommand { get; } = new(_ => { });

    /// <summary>
    /// Gets the command for the Defaults button.
    /// </summary>
    public ICommand DefaultsCommand { get; } = new RelayCommand(() => { });

    /// <summary>
    /// Adds a PDF directory to the collection.
    /// </summary>
    /// <param name="directory">The directory to add.</param>
    /// <returns><c>true</c> if the directory was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddPdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes a PDF directory from the collection.
    /// </summary>
    /// <param name="directory">The directory to remove.</param>
    /// <returns><c>true</c> if the directory was removed successfully; otherwise, <c>false</c>.</returns>
    public bool RemovePdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Loads the preferences.
    /// </summary>
    public void LoadPreferences()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}