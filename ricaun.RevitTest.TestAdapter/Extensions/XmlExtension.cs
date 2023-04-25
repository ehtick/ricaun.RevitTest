namespace ricaun.RevitTest.TestAdapter.Extensions
{
    using System.IO;
    using System.Xml.Serialization;

    internal static class XmlExtension
    {
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(this string value)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader xmlStream = new StringReader(value))
                {
                    return (T)serializer.Deserialize(xmlStream);
                }
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// SerializeXml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeXml<T>(this T value)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter xmlStream = new StringWriter())
                {
                    serializer.Serialize(xmlStream, value);
                    return xmlStream.ToString();
                }
            }
            catch { }
            return null;
        }
    }
}
