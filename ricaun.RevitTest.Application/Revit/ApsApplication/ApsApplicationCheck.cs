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
            Debug.WriteLine($"Aps Check[{ApsApplication.IsConnected}]: check/{AppApsUtils.AppId}/{AppApsUtils.AppVersion}");
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
                    Debug.WriteLine($"Aps Check: {ex.GetType()}");
                }
            }
            return result;
        }
    }

    public class ApsResponse
    {
        public string userId { get; set; }
        public string appId { get; set; }
        public bool isValid { get; set; }
        public string message { get; set; }

        public OtherResponse[] Other { get; set; }
        public class OtherResponse
        {
            public string Url { get; set; }
            public string Text { get; set; }
        }
        public override string ToString()
        {
            return $"[{userId}] {appId} {isValid} {message}";
        }
    }
}