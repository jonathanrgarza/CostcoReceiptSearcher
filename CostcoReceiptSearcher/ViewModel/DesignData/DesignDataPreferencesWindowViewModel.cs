using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CostcoReceiptSearcher.Preferences;
using Ncl.Common.Core.UI;
using Ncl.Common.Wpf.Infrastructure;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

internal class DesignDataPreferencesWindowViewModel : IPreferencesWindowViewModel
{
    public GeneralPreferences Preferences { get; set; } = new();

    public bool AllowWildcardSearch { get; set; }

    public bool CaseInsensitiveSearch { get; set; }

    public bool SearchInSubdirectories { get; set; }

    public ObservableCollection<string> PdfDirectories { get; set; } = ["C:\\PDFs", "D:\\Other_PDFs"];

    public string SelectedDirectory { get; set; } = "C:\\PDF";

    public string NewDirectory { get; set; } = "E:\\PDFs";

    public RelayCommand<ICloseable> OkCommand { get; } = new(_ => { });

    public RelayCommand<ICloseable> CancelCommand { get; } = new(_ => { });

    public ICommand DefaultsCommand { get; } = new RelayCommand(() => { });

    public bool AddPdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    public bool RemovePdfDirectory(string directory)
    {
        throw new NotImplementedException();
    }

    public void LoadPreferences()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}