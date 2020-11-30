using System.Collections.Generic;
using System.Xml.Serialization;

namespace IO.Dto
{
    [XmlRoot(ElementName = "I06")]
    public class I06Output
    {
        public I06Output()
        {
        }

        public I06Output(I07Output[] output)
        {
            Output = output;   
        }

        [XmlElement(ElementName = "I07")]
        public I07Output[] Output { get; init; }
    }
}
