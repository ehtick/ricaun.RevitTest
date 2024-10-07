using ricaun.RevitTest.Command.Extensions.Json;

namespace ricaun.RevitTest.Command.Extensions
{
    /// <summary>
    /// JsonExtension
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// IJsonService
        /// </summary>
        public static IJsonService JsonService { get; set; } = CreateJsonService();

        /// <summary>
        /// Create JsonService
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// If NewtonsoftJsonService is available, use it.
        /// </remarks>
        private static IJsonService CreateJsonService()
        {
            try
            {
                return new NewtonsoftJsonService();
            }
            catch { };
            return new JsonService();
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return JsonService.Deserialize<T>(value);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return JsonService.Serialize(value);
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