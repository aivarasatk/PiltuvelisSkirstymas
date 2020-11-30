using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace IO.Dto
{
    [Serializable()]
    public class I07Input
    {
        [XmlElement("I07_EIL_NR")]
        public int LineNr { get; set; }

        private string[] _code;
        [XmlElement("I07_KODAS")]
        public string[] Code
        {
            get => _code;
            set => _code = value.Select(v => v.Trim()).ToArray();
        }

        [XmlElement("I07_PAV")]
        public string Name { get; set; }

        private string _maker;
        [XmlElement("I07_KODAS_IS")]
        public string Maker
        {
            get => _maker;
            set => _maker = value.Trim(); 
        }

        public DateTime DateDateTime { get; set; }

        [XmlElement("I07_GALIOJA_IKI")]
        public string DateString
        {
            get => DateDateTime.ToString("yyyy-MM-dd"); 
            set => DateDateTime = DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture); 
        }

        [XmlElement("I07_APRASYMAS1")]
        public string Details1 { get; set; }

        [XmlElement("I07_APRASYMAS2")]
        public string Details2 { get; set; }

        [XmlElement("I07_APRASYMAS3")]
        public string Details3 { get; set; }

        [XmlElement("I07_KIEKIS")]
        public int Amount { get; set; }

        [XmlElement("PAP_2")]
        public string _pap2 { get; set; }

        public int Pap2 
        {
            get 
            {
                int.TryParse(_pap2, out var res);
                return res;
            }
        }

        public DateTime DimDateDateTime { get; set; }

        [XmlElement("DIM_01")]
        public string DimDate
        {
            get => DimDateDateTime.ToString("yyyy.MM.dd");
            set 
            { 
                DateTime.TryParseExact(value, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
                DimDateDateTime = date;
            }
        }
    }
}
