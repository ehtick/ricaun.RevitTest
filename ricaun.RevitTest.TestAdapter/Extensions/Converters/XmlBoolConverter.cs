namespace ricaun.RevitTest.TestAdapter.Extensions.Converters
{
    using ricaun.RevitTest.TestAdapter.Models;
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    internal class XmlBoolConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes => new[] { typeof(XmlBool) };

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var value = (XmlBool)obj;
            var result = new Dictionary<string, object>();
            result["bool"] = (bool)value;
            return result;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (type == typeof(XmlBool))
            {
                var value = (XmlBool)(bool)dictionary["bool"];
                return value;
            }
            return null;
        }
    }
}