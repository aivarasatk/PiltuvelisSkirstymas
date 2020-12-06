using IO.Dto;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IO
{
    public class EipWriter : IEipWriter
    {
        private const string FileDirectory = "Output";
        private readonly IFileSystem _fileSystem;

        public EipWriter(IFileSystem fileSystem)
        {
            Directory.CreateDirectory(FileDirectory);
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
        public async Task WriteAsync(I06Output output)
        {
            var fileContents = output.XmlSerialize();

            await _fileSystem.File.WriteAllTextAsync(
                Path.Combine(FileDirectory, $"Dukteriniu importas - {DateTime.Now:yyyy-MM-dd hh_mm_ss}.eip"),
                fileContents);
        }
    }
}
