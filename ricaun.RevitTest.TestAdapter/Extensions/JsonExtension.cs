namespace ricaun.RevitTest.TestAdapter.Extensions
{
    using ricaun.RevitTest.TestAdapter.Extensions.Json;
    /// <summary>
    /// JsonExtension
    /// </summary>
    internal static class JsonExtension
    {
        private static IJsonService jsonService { get; set; } = new JsonService();

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return jsonService.Deserialize<T>(value);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return jsonService.Serialize(value);
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