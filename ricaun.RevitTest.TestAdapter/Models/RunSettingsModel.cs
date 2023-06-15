namespace ricaun.RevitTest.TestAdapter.Models
{
    using ricaun.RevitTest.TestAdapter.Extensions;
    using System.Xml.Serialization;

    [XmlRoot("RunSettings")]
    internal class RunSettingsModel
    {
        [XmlElement("NUnit")]
        public NUnitModel NUnit { get; set; } = new NUnitModel();

        public class NUnitModel
        {
            [XmlElement("Version")]
            public int Version { get; set; }

            [XmlElement("Open")]
            public bool Open { get; set; }

            [XmlElement("Close")]
            public bool Close { get; set; }

            [XmlElement("Verbosity")]
            public int Verbosity { get; set; }

            [XmlElement("Application")]
            public string Application { get; set; }

            [XmlElement("Metadata")]
            public bool Metadata { get; set; } = true;
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
