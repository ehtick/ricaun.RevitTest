namespace ricaun.RevitTest.Command.Extensions
{
#if NETFRAMEWORK
    using System.Web.Script.Serialization;
    /// <summary>
    /// JsonExtension
    /// <code>Reference Include="System.Web.Extensions"</code>
    /// </summary>
    public static class JsonExtension
    {
        private static JavaScriptSerializer JavaScriptSerializer { get; set; } = new JavaScriptSerializer();

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
#endif

#if NETSTANDARD
    ﻿using Newtonsoft.Json;
    public static class JsonExtension
    {
        /// <summary>
        /// Settings
        /// </summary>
        public static JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
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
#endif

#if NET
    using System.Text.Json;
    public static class JsonExtension
    {
        /// <summary>
        /// Settings
        /// </summary>
        public static JsonSerializerOptions Settings { get; set; } = new JsonSerializerOptions();

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return JsonSerializer.Deserialize<T>(value, Settings);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, Settings);
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
#endif
}