using System.ComponentModel.DataAnnotations;

namespace MyCompanyWebsite.Data
{
	public class CompanyIsin
	{
		[Required]
		public string Value { get; set; }
	}
}
