using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IO.Dto
{
    [SerializableAttribute()]
    public class I06
    {
        [XmlElementAttribute("I07")]
        public IEnumerable<I07> I07 { get; set; }
    }
}
