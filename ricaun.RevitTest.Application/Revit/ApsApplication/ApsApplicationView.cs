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
                var isConnected = ApsApplication.IsConnected;
                Debug.WriteLine($"ApsView: {typeof(ApsView).Assembly}");
                apsView = new ApsView(Autodesk.Windows.ComponentManager.ApplicationWindow);
                apsView.SetApsConfiguration(ApsApplication.ClientId, ApsApplication.ClientPort, ApsApplication.ClientScopes);
                apsView.Closed += (s, e) => { apsView = null; };
                apsView.Closed += async (s, e) =>
                {
                    if (isConnected == false)
                    {
                        var result = await ApsApplicationLogger.Log("Login", $"Login with ApsView {typeof(ApsView).Assembly.GetName().Version.ToString(3)}");
                        Debug.WriteLine($"Login: {result}");
                    }
                };
                apsView.Show();
            }
            apsView?.Activate();
        }
    }

}
