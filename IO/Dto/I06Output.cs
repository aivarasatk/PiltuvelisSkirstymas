using System.Collections.Generic;
using System.IO;
using System.Xml;
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

    public static class I06OutputExtensions
    {
        public static string XmlSerialize(this I06Output data)
        {
            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            var serializer = new XmlSerializer(typeof(I06Output));
            serializer.Serialize(xmlWriter, data, emptyNamespaces);
            return stringWriter.ToString();
        }
    }
}
