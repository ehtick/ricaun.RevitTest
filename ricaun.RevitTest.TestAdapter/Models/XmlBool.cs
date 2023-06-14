namespace ricaun.RevitTest.TestAdapter.Models
{
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// XmlBool
    /// <code>https://stackoverflow.com/questions/1155227/xml-serialization-vs-true-and-false</code>
    /// </summary>
    internal struct XmlBool : IXmlSerializable
    {
        private bool _value;

        /// <summary>
        /// Allow implicit cast to a real bool
        /// </summary>
        /// <param name="value">Value to cast to bool</param>
        public static implicit operator bool(
            XmlBool value)
        {
            return value._value;
        }

        /// <summary>
        /// Allow implicit cast from a real bool
        /// </summary>
        /// <param name="value">Value to cash to y/n</param>
        public static implicit operator XmlBool(
            bool value)
        {
            return new XmlBool { _value = value };
        }

        /// <summary>
        /// This is not used
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads a value from XML
        /// </summary>
        /// <param name="reader">XML reader to read</param>
        public void ReadXml(
            XmlReader reader)
        {
            var s = reader.ReadElementContentAsString().ToLowerInvariant();
            _value = s == "true" || s == "yes" || s == "y";
        }

        /// <summary>
        /// Writes the value to XML
        /// </summary>
        /// <param name="writer">XML writer to write to</param>
        public void WriteXml(
            XmlWriter writer)
        {
            writer.WriteString(_value ? "true" : "false");
        }

        /// <summary>
        /// To string the value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
