namespace ricaun.RevitTest.TestAdapter.Extensions
{
    using System.Web.Script.Serialization;
    /// <summary>
    /// JsonExtension
    /// <code>Reference Include="System.Web.Extensions"</code>
    /// </summary>
    public static class JsonExtension
    {
        private static JavaScriptSerializer JavaScriptSerializer { get; set; } = Create();

        private static JavaScriptSerializer Create()
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.RegisterConverters(new[] { new Converters.XmlBoolConverter() });
            return javaScriptSerializer;
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return JavaScriptSerializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return JavaScriptSerializer.Serialize(value);
        }

        /// <summary>
        /// ToJson
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson(this object value)
        {
            return Serialize(value);
        }
    }
}