using IO.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IO
{
    public class EipReader : IEipReader
    {
        // filters control characters but allows only properly-formed surrogate sequences
        private static Regex _invalidXMLChars = new Regex(
            @"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
            RegexOptions.Compiled);


        private string CleanedEipLine(string input, char substringStart)
        {
            var index = input.IndexOf(substringStart);
            if (index > 0)
                return input.Substring(index);

            if (index == -1 && !string.IsNullOrWhiteSpace(input))
                throw new Exception($"Bad eip file: line doesn't have a valid start. Input: '{input}' Pivot: '{substringStart}'");

            return input;
        }

        private async Task<IEnumerable<string>> GetEipContentsByLine(string eipFileName) 
            => await File.ReadAllLinesAsync(eipFileName, Encoding.GetEncoding("iso-8859-13"));//Lithuanian encoding
        public async Task<IEnumerable<I07>> GetParsedEipContents(string filePath)
        {
            var content = await GetEipContentsByLine(filePath);
            var prunedEipFile = string.Join("", PrunedEipFile(content));
            var fileWithEscapedChars = RemoveInvalidXMLChars(EscapeCharacters(prunedEipFile));

            var xmlSerializer = new XmlSerializer(typeof(I06));
            using var stringReader = new StringReader(fileWithEscapedChars);
            return ((I06)xmlSerializer.Deserialize(stringReader)).I07;
        }

        /// <summary>
        /// removes any unusual unicode characters that can't be encoded into XML
        /// </summary>
        private static string RemoveInvalidXMLChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return _invalidXMLChars.Replace(text, "");
        }

        private IEnumerable<string> PrunedEipFile(IEnumerable<string> lines)
        {
            foreach (string line in lines)
                yield return CleanedEipLine(line, substringStart: '<');
        }

        private string EscapeCharacters(string prunedEipFile)
        {
            var charsToEscape = new (string key, string value)[] { ("&", "&amp;") };

            foreach (var (key, value) in charsToEscape)
                prunedEipFile = prunedEipFile.Replace(key, value);

            return prunedEipFile;
        }
    }
}
