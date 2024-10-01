using System.Diagnostics;
using System.IO;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Core.Xml;

namespace CostcoReceiptSearcher.Infrastructure;

/// <summary>
/// Represents a service for managing preferences.
/// </summary>
public sealed class PreferenceService : PreferenceServiceBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreferenceService"/> class.
    /// </summary>
    public PreferenceService()
    {
        XmlSerializationService = new XmlSerializationService();
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        FallbackDirectory = null;
        DefaultDirectory = Path.Combine(appDataPath, "CostcoReceiptSearcher", "Preferences");
        //Create the directory if it doesn't exist in debug release mode only
        if (Debugger.IsAttached && !Directory.Exists(DefaultDirectory))
        {
            Directory.CreateDirectory(DefaultDirectory);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreferenceService"/> class with the specified XML serialization service.
    /// </summary>
    /// <param name="xmlSerializationService">The XML serialization service.</param>
    public PreferenceService(IXmlSerializationService xmlSerializationService) : this()
    {
        XmlSerializationService =
            xmlSerializationService ?? throw new ArgumentNullException(nameof(xmlSerializationService));
    }

    /// <summary>
    /// Gets the XML serialization service.
    /// </summary>
    protected override IXmlSerializationService XmlSerializationService { get; }

    /// <summary>
    /// Gets the default directory for preferences.
    /// </summary>
    public override string DefaultDirectory { get; }

    /// <summary>
    /// Gets the fallback directory for preferences.
    /// </summary>
    public override string? FallbackDirectory { get; }
}
