namespace ricaun.RevitTest.TestAdapter.Extensions
{
    using System.IO;
    using System.Xml.Serialization;

    public static class XmlExtension
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
    }
}
