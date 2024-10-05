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
    /// <inheritdoc/>
    public GeneralPreferences Preferences { get; set; } = new();

    public bool CopyOnOpenFile { get; set; } = true;

    /// <inheritdoc/>
    public bool AllowWildcardSearch { get; set; }

    /// <inheritdoc/>
    public bool CaseInsensitiveSearch { get; set; }

    /// <inheritdoc/>
    public bool SearchInSubdirectories { get; set; }

    /// <inheritdoc/>
    public bool EnableCaching { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<string> PdfDirectories { get; set; } =
    [
        "C:\\PDFs",
        "D:\\Other_PDFs"
    ];

    /// <inheritdoc/>
    public string SelectedDirectory { get; set; } = "C:\\PDF";

    /// <inheritdoc/>
    public string NewDirectory { get; set; } = "E:\\PDFs";

    /// <inheritdoc/>
    public RelayCommand<ICloseableDialog> OkCommand { get; } = new(_ => { });

    /// <inheritdoc/>
    public RelayCommand<ICloseableDialog> CancelCommand { get; } = new(_ => { });

    /// <inheritdoc/>
    public ICommand DefaultsCommand { get; } = new RelayCommand(() => { });

    /// <inheritdoc/>
    public bool AddPdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool RemovePdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void LoadPreferences()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;
}