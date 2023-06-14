namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    internal static class XmlUtils
    {
        public static IDictionary<string, string> ParseKeyValues(string xmlText)
        {
            var dictionary = new Dictionary<string, string>();

            XElement xmlElement = XElement.Parse(xmlText);
            ExtractElements(xmlElement, "", dictionary);

            return dictionary;
        }

        private static void ExtractElements(XElement element, string prefix, Dictionary<string, string> dictionary)
        {
            var separator = '.';
            string currentPrefix = prefix + separator + element.Name.LocalName;
            currentPrefix = currentPrefix.Trim(separator);

            if (element.HasElements)
            {
                foreach (var subelement in element.Elements())
                {
                    ExtractElements(subelement, currentPrefix, dictionary);
                }
            }
            else
            {
                string key = currentPrefix;
                string value = element.Value;
                dictionary[key] = value;
            }
        }
    }
}
