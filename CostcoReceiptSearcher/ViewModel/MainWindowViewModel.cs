using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CostcoReceiptSearcher.Model;
using CostcoReceiptSearcher.View;
using Ncl.Common.Core.UI;

namespace CostcoReceiptSearcher.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<PdfFile> _matchingPdfFiles = [];

        private string _searchText = string.Empty;
        private PdfFile? _selectedPdfFile;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (Equals(value, _searchText))
                    return;
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
                    return;
                _matchingPdfFiles = value;
                OnPropertyChanged();
            }
        }

        public PdfFile? SelectedPdfFile
        {
            get => _selectedPdfFile;
            set
            {
                if (Equals(value, _selectedPdfFile))
                    return;
                _selectedPdfFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand MenuExitCommand { get; }

        public ICommand MenuPreferencesCommand { get; }

        public ICommand MenuAboutCommand { get; }

        public ICommand SearchCommand { get; }

        public ICommand OpenFileCommand { get; }

        public ICommand OpenFolderCommand { get; }

        public MainWindowViewModel()
        {
            SearchCommand = new RelayCommandAsync(SearchExecute);
            OpenFileCommand = new RelayCommand(OpenFileExecute);
            OpenFolderCommand = new RelayCommand(OpenFolderExecute);
            MenuExitCommand = new RelayCommand(() => Application.Current.Shutdown());
            MenuPreferencesCommand = new RelayCommand(() => new PreferencesWindow().ShowDialog());
            MenuAboutCommand = new RelayCommand(() => new AboutWindow().ShowDialog());
        }

        private async Task SearchExecute()
        {
            // Implement the logic to search for the text string within the PDF files
            // Add the matching files to the PdfFiles collection
            string searchText = SearchText;
            if (string.IsNullOrWhiteSpace(searchText))
                return;
        }

        private void OpenFileExecute()
        {
            if (_selectedPdfFile == null)
                return;
            Process.Start(new ProcessStartInfo(_selectedPdfFile.FilePath) { UseShellExecute = true });
        }

        private void OpenFolderExecute()
        {
            if (_selectedPdfFile == null)
                return;
            string argument = "/select, \"" + _selectedPdfFile.FilePath + "\"";
            Process.Start("explorer.exe", argument);
        }
    }
}