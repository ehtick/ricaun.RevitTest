using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Metadatas;
using ricaun.RevitTest.TestAdapter.Models;

namespace ricaun.RevitTest.TestAdapter
{
    internal class AdapterSettings : IAdapterSettings
    {
        #region static
        internal static AdapterSettings Instance { get; set; }
        public static RunSettingsModel Settings
        {
            get
            {
                if (Instance == null)
                    return new RunSettingsModel();

                return Instance.RunSettings ?? new RunSettingsModel();
            }
        }

        public static void Create(IDiscoveryContext discoveryContext)
        {
            if (Instance == null)
            {
                Instance = new AdapterSettings();
                Instance.Initialize(discoveryContext);
            }
        }
        #endregion

        private IDiscoveryContext discoveryContext;
        public RunSettingsModel RunSettings { get; private set; }
        public void Initialize(IDiscoveryContext discoveryContext)
        {
            this.discoveryContext = discoveryContext;

            if (discoveryContext.RunSettings is IRunSettings runSettings)
            {
                //RunSettings = runSettings.SettingsXml.DeserializeXml<RunSettingsModel>();
                try
                {
                    MapperKey.Map(this, XmlUtils.ParseKeyValues(runSettings.SettingsXml));
                }
                catch { }
            }
        }
    }

    internal interface IAdapterSettings
    {
        void Initialize(IDiscoveryContext discoveryContext);
        RunSettingsModel RunSettings { get; }
    }
}
