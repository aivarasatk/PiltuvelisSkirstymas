using System;
using System.Xml.Serialization;

namespace IO.Dto
{
    [Serializable]
    [XmlRoot(ElementName = "I06")]
    public class I06Input
    {
        [XmlElement("I07")]
        public I07Input[] I07 { get; set; }
    }
}
