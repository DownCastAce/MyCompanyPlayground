using System.ComponentModel.DataAnnotations;

namespace MyCompanyWebsite.Data
{
	public class UpdateCompany
	{
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Name of the Company
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Stock Exchange
        /// </summary>
        [Required]
        public string Exchange { get; set; }

        /// <summary>
        /// Stock Ticker
        /// </summary>
        [Required]
        public string Ticker { get; set; }

        /// <summary>
        /// International Securities Identification Numbers
        /// </summary>
        [Required]
        public string Isin { get; set; }

        /// <summary>
        /// Company Website
        /// </summary>
        public string Website { get; set; }
       
    }
}
