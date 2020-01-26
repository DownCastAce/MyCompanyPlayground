using System.Collections.Generic;
using System.Linq;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Logic
{
    public class PayloadValidator
    {
        public IList<string> MissingMandatoryFields = new List<string>();
        
        public bool IsValid(CompanyPayload request)
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                MissingMandatoryFields.Add("CompanyName");
            }
            if (string.IsNullOrWhiteSpace(request.Ticker))
            {
                MissingMandatoryFields.Add("Ticker");
            }
            if (string.IsNullOrWhiteSpace(request.Exchange))
            {
                MissingMandatoryFields.Add("Exchange");
            }
            if (string.IsNullOrWhiteSpace(request.Isin))
            {
                MissingMandatoryFields.Add("Isin");
            }
            
            return !MissingMandatoryFields.Any();
        }
    }
}