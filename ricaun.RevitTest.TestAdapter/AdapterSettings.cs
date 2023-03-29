using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using ricaun.RevitTest.TestAdapter.Extensions;
using ricaun.RevitTest.TestAdapter.Models;

namespace ricaun.RevitTest.TestAdapter
{
    public class AdapterSettings : IAdapterSettings
    {
        #region static
        private static AdapterSettings Instance { get; set; }
        public static RunSettingsModel Settings
        {
            get
            {
                if (Instance == null)
                    return new RunSettingsModel();
                return Instance.RunSettings;
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
                RunSettings = runSettings.SettingsXml.DeserializeXml<RunSettingsModel>();
            }
        }
    }

    public interface IAdapterSettings
    {
        void Initialize(IDiscoveryContext discoveryContext);
        RunSettingsModel RunSettings { get; }
    }
}
