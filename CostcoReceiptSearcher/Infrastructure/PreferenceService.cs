using System.Diagnostics;
using System.IO;
using Ncl.Common.Core.Preferences;
using Ncl.Common.Core.Xml;

namespace CostcoReceiptSearcher.Infrastructure;

public sealed class PreferenceService : PreferenceServiceBase
{
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

    public PreferenceService(IXmlSerializationService xmlSerializationService) : this()
    {
        XmlSerializationService =
            xmlSerializationService ?? throw new ArgumentNullException(nameof(xmlSerializationService));
    }

    protected override IXmlSerializationService XmlSerializationService { get; }
    public override string DefaultDirectory { get; }
    public override string? FallbackDirectory { get; }
}