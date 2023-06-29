using Newtonsoft.Json;

namespace ricaun.RevitTest.Application.Revit.ApsApplication.Services
{
    public class JsonService : IJsonService
    {
        /// <summary>
        /// Instance
        /// </summary>
        public static IJsonService Instance { get; set; } = new JsonService();
        public T ToJson<T>(string content)
        {
            if (content is null) return default(T);
            if (content is T contentT) return contentT;
            return JsonConvert.DeserializeObject<T>(content);
        }
        public string FromJson<T>(T content)
        {
            if (content is null) return null;
            if (content is string contentS) return contentS;
            return JsonConvert.SerializeObject(content);
        }
    }

    public interface IJsonService
    {
        T ToJson<T>(string content);
        string FromJson<T>(T content);
    }
}
