using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Logic
{
    public static class PayloadValidator
    {
        public static IList<string> VerifyMandatoryFields(CompanyPayload company)
        {
            IList<string> missingMandatoryFields = new List<string>();
            if (string.IsNullOrWhiteSpace(company.CompanyName))
            {
                missingMandatoryFields.Add("CompanyName");
            }
            if (string.IsNullOrWhiteSpace(company.Ticker))
            {
                missingMandatoryFields.Add("Ticker");
            }
            if (string.IsNullOrWhiteSpace(company.Exchange))
            {
                missingMandatoryFields.Add("Exchange");
            }
            if (string.IsNullOrWhiteSpace(company.Isin))
            {
                missingMandatoryFields.Add("Isin");
            }
            
            return missingMandatoryFields;
        }
        
        public static bool IsValid(string isin)
        {
            Regex regex = new Regex("[a-zA-Z]{2}[\\w]{10}");
            return regex.Match(isin).Success;
        }
    }
}