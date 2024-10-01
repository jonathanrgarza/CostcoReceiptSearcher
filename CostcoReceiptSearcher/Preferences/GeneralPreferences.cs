using Ncl.Common.Core.Preferences;

namespace CostcoReceiptSearcher.Preferences;

/// <summary>
/// Represents the general preferences for the application.
/// </summary>
public class GeneralPreferences : IPreference
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class.
    /// </summary>
    public GeneralPreferences()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class with specified values.
    /// </summary>
    /// <param name="pdfDirectories">The directories to search for PDF files.</param>
    /// <param name="allowWildcardSearch">A value indicating whether wildcard search is allowed.</param>
    /// <param name="caseInsensitiveSearch">A value indicating whether the search is case-insensitive.</param>
    /// <param name="searchInSubdirectories">A value indicating whether to search in subdirectories.</param>
    /// <param name="enableCaching">A value indicating whether caching is enabled.</param>
    public GeneralPreferences(List<string> pdfDirectories, bool allowWildcardSearch,
        bool caseInsensitiveSearch, bool searchInSubdirectories, bool enableCaching)
    {
        PdfDirectories = pdfDirectories;
        AllowWildcardSearch = allowWildcardSearch;
        CaseInsensitiveSearch = caseInsensitiveSearch;
        SearchInSubdirectories = searchInSubdirectories;
        EnableCaching = enableCaching;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralPreferences"/> class by copying values from another instance.
    /// </summary>
    /// <param name="other">The instance from which to copy the values.</param>
    public GeneralPreferences(GeneralPreferences other)
    {
        PdfDirectories = [.. other.PdfDirectories];
        AllowWildcardSearch = other.AllowWildcardSearch;
        CaseInsensitiveSearch = other.CaseInsensitiveSearch;
        SearchInSubdirectories = other.SearchInSubdirectories;
        EnableCaching = other.EnableCaching;
    }

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
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the directories to search for PDF files.
    /// </summary>
    public List<string> PdfDirectories { get; set; } = [];

    /// <summary>
    /// Determines whether the current instance is equal to another instance of the <see cref="IPreference"/> interface.
    /// </summary>
    public bool Equals(IPreference? other)
    {
        if (other is not GeneralPreferences otherPreferences)
        {
            return false;
        }

        return AllowWildcardSearch == otherPreferences.AllowWildcardSearch &&
               CaseInsensitiveSearch == otherPreferences.CaseInsensitiveSearch &&
               SearchInSubdirectories == otherPreferences.SearchInSubdirectories &&
               EnableCaching == otherPreferences.EnableCaching &&
               PdfDirectories.SequenceEqual(otherPreferences.PdfDirectories);
    }

    /// <inheritdoc/>
    public IPreference Clone()
    {
        return new GeneralPreferences(this);
    }

    /// <inheritdoc/>
    public Version GetVersion()
    {
        return new Version(1, 0);
    }

    /// <inheritdoc/>
    public IPreference OnDeserialization()
    {
        // Perform necessary post-deserialization tasks
        return this;
    }
}