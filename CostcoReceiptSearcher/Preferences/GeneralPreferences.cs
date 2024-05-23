using Ncl.Common.Core.Preferences;

namespace CostcoReceiptSearcher.Preferences;

/// <summary>
/// Represents the general preferences for the application.
/// </summary>
public class GeneralPreferences : IPreference
{
    /// <summary>
    /// Gets or sets a value indicating whether wildcard search is allowed.
    /// </summary>
    public bool AllowWildcardSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the search is case-insensitive.
    /// </summary>
    public bool CaseInsensitiveSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to search in subdirectories.
    /// </summary>
    public bool SearchInSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets the directories to search for PDF files.
    /// </summary>
    public List<string> PdfDirectories { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class.
    /// </summary>
    public GeneralPreferences()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class with specified values.
    /// </summary>
    public GeneralPreferences(List<string> pdfDirectories, bool allowWildcardSearch,
        bool caseInsensitiveSearch, bool searchInSubdirectories)
    {
        PdfDirectories = pdfDirectories;
        AllowWildcardSearch = allowWildcardSearch;
        CaseInsensitiveSearch = caseInsensitiveSearch;
        SearchInSubdirectories = searchInSubdirectories;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class by copying values from another instance.
    /// </summary>
    public GeneralPreferences(GeneralPreferences other)
    {
        PdfDirectories = [..other.PdfDirectories];
        AllowWildcardSearch = other.AllowWildcardSearch;
        CaseInsensitiveSearch = other.CaseInsensitiveSearch;
        SearchInSubdirectories = other.SearchInSubdirectories;
    }

    /// <summary>
    /// Determines whether the current instance is equal to another instance of the <see cref="IPreference"/> interface.
    /// </summary>
    public bool Equals(IPreference? other)
    {
        if (other is not GeneralPreferences otherPreferences)
            return false;

        return AllowWildcardSearch == otherPreferences.AllowWildcardSearch &&
               CaseInsensitiveSearch == otherPreferences.CaseInsensitiveSearch &&
               SearchInSubdirectories == otherPreferences.SearchInSubdirectories &&
               PdfDirectories.SequenceEqual(otherPreferences.PdfDirectories);
    }

    /// <inheritdoc />
    public IPreference Clone()
    {
        return new GeneralPreferences(this);
    }

    /// <inheritdoc />
    public Version GetVersion() => new(1, 0);

    /// <inheritdoc />
    public IPreference OnDeserialization()
    {
        // Perform necessary post-deserialization tasks
        return this;
    }
}