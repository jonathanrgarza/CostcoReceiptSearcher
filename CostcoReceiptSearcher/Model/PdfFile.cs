using System.IO;

namespace CostcoReceiptSearcher.Model;

/// <summary>
/// Represents a PDF file.
/// </summary>
public class PdfFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfFile"/> class.
    /// </summary>
    /// <param name="filePath">The path of the PDF file.</param>
    public PdfFile(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
    }

    /// <summary>
    /// Gets the path of the PDF file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the name of the PDF file.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets or sets the lines of text extracted from the PDF file.
    /// </summary>
    public string[]? Lines { get; set; }

    /// <summary>
    /// Gets or sets the hash value of the PDF file.
    /// </summary>
    public byte[] FileHash { get; set; } = [];

    /// <summary>
    /// Returns the name of the PDF file.
    /// </summary>
    /// <returns>The name of the PDF file.</returns>
    public override string ToString()
    {
        return FileName;
    }
}
