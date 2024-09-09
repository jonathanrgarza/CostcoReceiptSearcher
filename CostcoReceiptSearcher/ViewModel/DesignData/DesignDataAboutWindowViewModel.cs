using System.Windows.Input;
using Ncl.Common.Core.UI;

namespace CostcoReceiptSearcher.ViewModel.DesignData;

public class DesignDataAboutWindowViewModel : IAboutWindowViewModel
{
    public string Title { get; } = "Costco Receipt Searcher";

    public string Version { get; } = "1.0";

    public string Description { get; } =
        "This application allows you to search through Costco receipts to find the items you purchased.";

    public string Author { get; } = "Jonathan";

    public ICommand OkCommand => new RelayCommand(() => { });
}