using ricaun.Aps;
using ricaun.Auth.Aps;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class ApsApplication
    {
        public static string ClientId = "Ko1ABEYpRPL7K8ju1JYUAUzG5pkguxVQ";
        public static int ClientPort = 52000;
        public static string[] ClientScopes = new string[] {
             ApsScope.UserProfileRead,
        };

        public static ApsService ApsService;
        public static bool IsConnected => ApsService?.IsConnected ?? false;
        public static async Task Initialize()
        {
            ApsService = new ApsService(ClientId, ClientScopes) { ClientPort = ClientPort };
            await ApsService.Initialize();
            //System.Console.WriteLine($"ApsService: {ApsService.IsConnected}");
        }
    }
}
