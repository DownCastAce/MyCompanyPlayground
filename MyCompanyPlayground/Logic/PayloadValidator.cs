using System.Collections.Generic;
using System.Linq;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Logic
{
    public class PayloadValidator
    {
        public IList<string> MissingMandatoryFields = new List<string>();
        
        public bool IsValid(CompanyPayload company)
        {
            if (string.IsNullOrWhiteSpace(company.CompanyName))
            {
                MissingMandatoryFields.Add("CompanyName");
            }
            if (string.IsNullOrWhiteSpace(company.Ticker))
            {
                MissingMandatoryFields.Add("Ticker");
            }
            if (string.IsNullOrWhiteSpace(company.Exchange))
            {
                MissingMandatoryFields.Add("Exchange");
            }
            if (string.IsNullOrWhiteSpace(company.Isin))
            {
                MissingMandatoryFields.Add("Isin");
            }
            
            return !MissingMandatoryFields.Any();
        }
    }
}