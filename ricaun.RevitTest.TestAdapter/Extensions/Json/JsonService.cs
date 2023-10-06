namespace ricaun.RevitTest.TestAdapter.Extensions.Json
{
#if NETFRAMEWORK
    using System.Web.Script.Serialization;
    /// <summary>
    /// JsonService
    /// <code>Reference Include="System.Web.Extensions"</code>
    /// </summary>
    internal class JsonService : IJsonService
    {
        private static JavaScriptSerializer JavaScriptSerializer { get; set; } = new JavaScriptSerializer();
        public T Deserialize<T>(string value)
        {
            return JavaScriptSerializer.Deserialize<T>(value);
        }

        public string Serialize(object value)
        {
            return JavaScriptSerializer.Serialize(value);
        }
    }
#else
    using System.Text.Json;
    internal class JsonService : IJsonService
    {
        public T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        public string Serialize(object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
#endif
}