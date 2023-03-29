namespace ricaun.RevitTest.TestAdapter.Models
{
    using System.Xml.Serialization;

    [XmlRoot("RunSettings")]
    public class RunSettingsModel
    {
        [XmlElement("NUnit")]
        public NUnitModel NUnit { get; set; } = new NUnitModel();

        public class NUnitModel
        {
            [XmlElement("RevitVersion")]
            public int RevitVersion { get; set; }

            [XmlElement("RevitOpen")]
            public bool RevitOpen { get; set; }

            [XmlElement("RevitClose")]
            public bool RevitClose { get; set; }
        }
    }
}
