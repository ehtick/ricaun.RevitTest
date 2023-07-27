using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class ApsApplicationCheck
    {
        //private const string requestUri = "http://localhost:5000/api/v1/aps/check/{0}/{1}";
        private const string requestUri = "https://ricaun-aps-application.web.app/api/v1/aps/check/{0}/{1}";

        public static async Task<ApsResponse> Check()
        {
            ApsResponse result = null;
            Debug.WriteLine($"Check[{ApsApplication.IsConnected}]: check/{AppApsUtils.AppId}/{AppApsUtils.AppVersion}");
            if (ApsApplication.IsConnected)
            {
                try
                {
                    var service = await ApsApplication.ApsService.ApsClient.GetRequestServiceAsync();
                    using (service)
                    {
                        var request = string.Format(requestUri, AppApsUtils.AppId, AppApsUtils.AppVersion);
                        result = await service.GetAsync<ApsResponse>(request);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return result;
        }
    }
}