using System.ComponentModel.DataAnnotations;

namespace MyCompanyWebsite.Data
{
	public class CompanyId
	{
		[Required]
		public string Value { get; set; }
	}
}
