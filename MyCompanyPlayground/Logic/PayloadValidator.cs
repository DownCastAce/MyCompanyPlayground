using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Logic
{
    public static class PayloadValidator
    {
        public static IList<string> VerifyMandatoryFields(CompanyPayload company)
        {
            IList<string> MissingMandatoryFields = new List<string>();
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
            
            return MissingMandatoryFields;
        }
        
        public static bool IsValid(string isin)
        {
            Regex regex = new Regex("[a-zA-z]{2}[\\w]{10}");
            return regex.Match(isin).Success;
        }
    }
}