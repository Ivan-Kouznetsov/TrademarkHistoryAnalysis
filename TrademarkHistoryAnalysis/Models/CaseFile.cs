using System;
using System.Collections.Generic;
using System.Text;

namespace TrademarkHistoryAnalysis.Models
{
    public class CaseFile
    {
        public DateTime FilingDate { get; private set; }
        public int SerialNumber { get; private set; }
        public DateTime? RegistrationDate { get; private set; }
        public int? RegistrationNumber { get; private set; }
        public string Owner { get; private set; }
        public int OwnerTypeId { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string Attorney { get; private set; }
        public int StatusCode { get; private set; }
        public string MarkLiteralElements { get; private set; }
        public string GoodsAndServices { get; private set; }
        public List<int> Classes { get; private set; }

        public CaseFile(DateTime filingDate, int serialNumber, DateTime? registrationDate, int? registrationNumber, string owner,
            int ownerTypeId, string state, string country, string attorney, int statusCode, string markLiteralElements, string goodsAndServices,
            List<int> classes)
        {
            FilingDate = filingDate;
            SerialNumber = serialNumber;
            RegistrationDate = registrationDate;
            RegistrationNumber = registrationNumber;
            Owner = owner;
            OwnerTypeId = ownerTypeId;
            State = state;
            Country = country;
            Attorney = attorney;
            StatusCode = statusCode;
            MarkLiteralElements = markLiteralElements;
            GoodsAndServices = goodsAndServices;
            Classes = classes;
        }
    }
}
