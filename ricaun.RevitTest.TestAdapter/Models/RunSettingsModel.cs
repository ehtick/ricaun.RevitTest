namespace ricaun.RevitTest.TestAdapter.Models
{
    using ricaun.RevitTest.TestAdapter.Extensions;
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
            public XmlBool RevitOpen { get; set; }

            [XmlElement("RevitClose")]
            public XmlBool RevitClose { get; set; }

            [XmlElement("Verbosity")]
            public int Verbosity { get; set; }

            [XmlElement("Application")]
            public string Application { get; set; }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
