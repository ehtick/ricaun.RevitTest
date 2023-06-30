using ricaun.Auth.Aps.UI;
using System.Diagnostics;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    /// <summary>
    /// ApsApplicationView
    /// </summary>
    public static class ApsApplicationView
    {
        private static ApsView apsView;
        /// <summary>
        /// OpenApsView
        /// </summary>
        public static void OpenApsView()
        {
            if (apsView is null)
            {
                Debug.WriteLine($"{typeof(ApsView).Assembly}");
                apsView = new ApsView(Autodesk.Windows.ComponentManager.ApplicationWindow);
                apsView.SetApsConfiguration(ApsApplication.ClientId, ApsApplication.ClientPort, ApsApplication.ClientScopes);
                apsView.Closed += (s, e) => { apsView = null; };
                apsView.Show();
            }
            apsView?.Activate();
        }
    }

}
