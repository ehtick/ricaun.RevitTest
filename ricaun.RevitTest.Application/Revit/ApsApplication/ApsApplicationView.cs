using ricaun.Auth.Aps;
using ricaun.Auth.Aps.UI;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    /// <summary>
    /// ApsApplicationView
    /// </summary>
    public static class ApsApplicationView
    {
        private const int MillisecondsDelayCloseAfterConnected = 2000;
        private const int MillisecondsDelayAutoClose = 90000;

        private static ApsView apsView;
        /// <summary>
        /// OpenApsView
        /// </summary>
        public static void OpenApsView(bool showDialog = false)
        {
            try
            {
                if (apsView is null)
                {
                    var apsService = new ApsService(ApsApplication.ClientId, ApsApplication.ClientScopes)
                    {
                        ClientPort = ApsApplication.ClientPort,
                    };
                    var isConnected = ApsApplication.IsConnected;
                    Debug.WriteLine($"ApsView: {typeof(ApsView).Assembly}");

                    apsView = new ApsView(Autodesk.Windows.ComponentManager.ApplicationWindow);
                    apsView.SetAppName("RevitTest");
                    apsView.SetApsConfiguration(apsService);
                    apsView.Closed += (s, e) => { apsService?.Dispose(); };
                    apsView.Closed += (s, e) => { apsView = null; };
                    apsView.Closed += async (s, e) =>
                    {
                        if (isConnected == false)
                        {
                            var result = await ApsApplicationLogger.Log("Login", $"Login with ApsView {typeof(ApsView).Assembly.GetName().Version.ToString(3)}");
                            Debug.WriteLine($"Login: {result}");
                        }
                    };

                    if (showDialog)
                    {
                        apsService.Connected += async (client) =>
                        {
                            Debug.WriteLine($"ApsView: [Connected] was conencted = {isConnected}");
                            if (isConnected == false)
                            {
                                await Task.Delay(MillisecondsDelayCloseAfterConnected);
                                apsView?.Close();
                            }
                        };

                        Task.Run(async () =>
                        {
                            await Task.Delay(MillisecondsDelayAutoClose);
                            if (apsView != null)
                            {
                                Debug.WriteLine($"ApsView: [AutoClose] {MillisecondsDelayAutoClose}");
                                try
                                {
                                    apsView?.Dispatcher?.Invoke(() => { apsView?.Close(); });
                                }
                                catch (System.Exception ex)
                                {
                                    System.Console.WriteLine(ex);
                                }
                            }
                        });

                        apsView.ShowDialog();
                    }
                    apsView?.Show();
                }
                apsView?.Activate();
            }
            catch (System.Exception)
            {
                Task.Run(async () =>
                {
                    if (ApsApplication.IsConnected)
                    {
                        await ApsApplication.Logout();
                    }
                    else
                    {
                        await ApsApplication.Login();
                    }
                });
            }
        }
    }
}