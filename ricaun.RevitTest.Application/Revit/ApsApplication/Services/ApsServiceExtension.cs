using ricaun.Auth.Aps;
using System.Net.Http;

namespace ricaun.RevitTest.Application.Revit.ApsApplication.Services
{
    /// <summary>
    /// ApsServiceExtension
    /// </summary>
    public static class ApsServiceExtension
    {
        /// <summary>
        /// GetHttpClient
        /// </summary>
        /// <param name="apsService"></param>
        /// <returns></returns>
        public static HttpClient GetHttpClient(this ApsService apsService)
        {
            if (apsService == null) return null;
            if (apsService.IsConnected == false) return null;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apsService.ApsClient.GetAccessToken().AccessToken}");
            return httpClient;
        }
    }

}
