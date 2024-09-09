using System.ComponentModel;
using System.Windows.Input;
using Ncl.Common.Core.UI;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

public class DesignDataAboutWindowViewModel : IAboutWindowViewModel
{
    public string Title => "Costco Receipt Searcher";

    public string Version => "1.0";

    public string Description => "This application allows you to search through Costco receipts to find the items you purchased.";

    public string Author => "Jonathan";

    public ICommand OkCommand => new RelayCommand(() => { });

    public event PropertyChangedEventHandler? PropertyChanged;
}