using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CostcoReceiptSearcher.Model;

namespace CostcoReceiptSearcher.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
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
                OnPropertyChanged(nameof(SearchText));
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
                OnPropertyChanged(nameof(MatchingPdfFiles));
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
                OnPropertyChanged(nameof(PdfFile));
            }
        }

        public ICommand SearchCommand => new RelayCommand(Search);
        public ICommand OpenFileCommand => new RelayCommand(OpenFile);
        public ICommand OpenFolderCommand => new RelayCommand(OpenFolder);

        private void Search()
        {
            // Implement the logic to search for the text string within the PDF files
            // Add the matching files to the PdfFiles collection
        }

        private void OpenFile()
        {
            if (_selectedPdfFile == null)
                return;
            Process.Start(new ProcessStartInfo(_selectedPdfFile.FilePath) { UseShellExecute = true });
        }

        private void OpenFolder()
        {
            if (_selectedPdfFile == null)
                return;
            string argument = "/select, \"" + _selectedPdfFile.FilePath + "\"";
            Process.Start("explorer.exe", argument);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        public RelayCommand(Action action)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler? CanExecuteChanged;
    }
}