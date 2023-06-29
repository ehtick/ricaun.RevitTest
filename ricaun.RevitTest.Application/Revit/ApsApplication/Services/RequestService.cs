using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication.Services
{

    public class RequestService : IRequestService
    {
        private readonly HttpClient httpClient;
        private readonly IJsonService jsonService;

        public RequestService(HttpClient httpClient, IJsonService jsonService = null)
        {
            this.httpClient = httpClient;
            this.jsonService = jsonService ?? JsonService.Instance;
        }
        public async Task<T> GetAsync<T>(string request)
        {
            var response = await httpClient.GetAsync(request);
            response.EnsureSuccessStatusCode();
            return await ReadAsJsonAsync<T>(response.Content);
        }
        public async Task<T> PostAsync<T>(string request, object content = null)
        {
            var jsonContent = JsonStringContent(content);
            var response = await httpClient.PostAsync(request, jsonContent);
            response.EnsureSuccessStatusCode();
            return await ReadAsJsonAsync<T>(response.Content);
        }

        public async Task<T> PutAsync<T>(string request, object content = null)
        {
            var jsonContent = JsonStringContent(content);
            var response = await httpClient.PutAsync(request, jsonContent);
            response.EnsureSuccessStatusCode();
            return await ReadAsJsonAsync<T>(response.Content);
        }

        public async Task<T> DeleteAsync<T>(string request)
        {
            var response = await httpClient.DeleteAsync(request);
            response.EnsureSuccessStatusCode();
            return await ReadAsJsonAsync<T>(response.Content);
        }

        #region private
        private async Task<T> ReadAsJsonAsync<T>(HttpContent content)
        {
            var result = await content.ReadAsStringAsync();
            return jsonService.FromJson<T>(result);
        }
        private StringContent JsonStringContent(object content)
        {
            var json = jsonService.ToJson(content);
            if (json is null) return null;
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return stringContent;
        }
        #endregion
    }

    public interface IRequestService
    {
        Task<T> GetAsync<T>(string request);
        Task<T> PostAsync<T>(string request, object content = null);
        Task<T> PutAsync<T>(string request, object content = null);
        Task<T> DeleteAsync<T>(string request);
    }
}
