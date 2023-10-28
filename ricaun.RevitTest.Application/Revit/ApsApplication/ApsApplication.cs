using ricaun.Aps;
using ricaun.Aps.Extensions;
using ricaun.Auth.Aps;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class ApsApplication
    {
        public static string ClientId = "Ko1ABEYpRPL7K8ju1JYUAUzG5pkguxVQ";
        public static int ClientPort = 52000;
        public static string[] ClientScopes = new string[] {
             ApsScope.Openid,
        };

        public static ApsService ApsService;
        public static bool IsConnected => ApsService?.IsConnected ?? false;
        public static string LoginUserId => GetLoginUserId();
        public static string GetLoginUserId()
        {
            if (IsConnected == false)
                return string.Empty;

            var data = ApsService.ApsClient?.GetAccessToken()?.GetDataBase();

            if (data is null)
                return string.Empty;

            return data.UserId;
        }
        public static async Task Initialize()
        {
            ApsService = new ApsService(ClientId, ClientScopes) { ClientPort = ClientPort };
            await ApsService.Initialize();
            //System.Console.WriteLine($"ApsService: {ApsService.IsConnected}");
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> Login()
        {
            return await ApsService.Login();
        }
        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public static async Task Logout()
        {
            await ApsService.Logout();
        }
    }
}
