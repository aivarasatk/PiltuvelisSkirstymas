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
            var fileContents = output.XmlSerialize();

            await File.WriteAllTextAsync(
                Path.Combine(FileDirectory, $"Dukteriniu importas - {DateTime.Now:yyyy-MM-dd hh_mm_ss}.eip"),
                fileContents);
        }
    }
}
