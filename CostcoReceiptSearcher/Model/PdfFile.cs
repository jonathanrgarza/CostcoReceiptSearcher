using System.IO;

namespace CostcoReceiptSearcher.Model;

public class PdfFile
{
    public PdfFile(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
    }

    public string FilePath { get; init; }

    public string FileName { get; }

    public string[]? Lines { get; set; }
}