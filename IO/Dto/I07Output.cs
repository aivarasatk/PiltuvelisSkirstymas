﻿using System;
using System.Linq;
using System.Xml.Serialization;

namespace IO.Dto
{
    [Serializable]
    public class I07Output
    {
        public I07Output()
        {
        }

        public I07Output(I07Input input)
        {
            Code = input.Code.First();
            Maker = input.Maker;
            DateString = input.DateString;
            Details1 = !string.IsNullOrWhiteSpace(input.Details1.Trim()) ? input.Details1.Trim() : " ";//Space is needed to render open/closing tags in xml
            Details2 = !string.IsNullOrWhiteSpace(input.Details2.Trim()) ? input.Details2.Trim() : " ";
            Details3 = !string.IsNullOrWhiteSpace(input.Details3.Trim()) ? input.Details3.Trim() : " ";
            Amount = input.Amount;
            Pap2 = input.LineNr;
            DimDate = input.DimDateDateTime != default ? input.DimDate : " ";
        }

        [XmlElement("I07_KODAS_PO")]
        public string ReferenceNumber { get; set; }

        [XmlElement("I07_KODAS")]
        public string Code { get; set; }

        [XmlElement("I07_KODAS_IS")]
        public string Maker { get; set; }

        [XmlElement("I07_GALIOJA_IKI")]
        public string DateString { get; set; }

        [XmlElement("I07_APRASYMAS1")]
        public string Details1 { get; set; }

        [XmlElement("I07_APRASYMAS2")]
        public string Details2 { get; set; }

        [XmlElement("I07_APRASYMAS3")]
        public string Details3 { get; set; }

        [XmlElement("I07_KIEKIS")]
        public int Amount { get; set; }

        [XmlElement("PAP_2")]
        public int Pap2 { get; set; }

        [XmlElement("DIM_01")]
        public string DimDate { get; set; }
    }
}
