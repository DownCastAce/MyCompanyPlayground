using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Models
{
    public class Company
    {
        public Company() { }

        public Company(CompanyPayload payload)
        {
            Name = payload.CompanyName;
            Exchange = payload.Exchange;
            Ticker = payload.Ticker;
            Isin = payload.Isin;
            Website = payload.Website;
        }
        
        /// <summary>
        /// Name of the Company
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Stock Exchange
        /// </summary>
        public string Exchange { get; set; }
        
        /// <summary>
        /// Stock Ticker
        /// </summary>
        public string Ticker { get; set; }
        
        /// <summary>
        /// International Securities Identification Numbers
        /// </summary>
        public string Isin { get; set; }
        
        /// <summary>
        /// Company Website
        /// </summary>
        public string Website { get; set; }
    }
}