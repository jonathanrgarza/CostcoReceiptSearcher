using System.IO;

namespace CostcoReceiptSearcher.Model;

public class PdfFile(string filePath)
{
    public string FilePath { get; init; } = filePath;

    public string FileName { get; } = Path.GetFileName(filePath);

    public string[]? Lines { get; set; }
}