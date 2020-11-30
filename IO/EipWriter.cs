using IO.Dto;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IO
{
    public class EipWriter : IEipWriter
    {
        private const string FileDirectory = "Output";

        public EipWriter()
        {
            Directory.CreateDirectory(FileDirectory);
        }
        public async Task WriteAsync(I06Output output)
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
            serializer.Serialize(xmlWriter, output, emptyNamespaces);

            await File.WriteAllTextAsync(
                Path.Combine(FileDirectory, $"Eksportas-{DateTime.Now:yyyy-MM-dd hh_mm_ss}.eip"),
                stringWriter.ToString());
        }
    }
}
