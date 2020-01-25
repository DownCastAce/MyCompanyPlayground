using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Services
{
	public class CompanyService : CompanyServices.CompanyServicesBase
	{
		private readonly ILogger<CompanyService> _logger;

		public CompanyService(ILogger<CompanyService> logger)
		{
			_logger = logger;
		}
		
		public override Task<Response> AddCompany(Company request, ServerCallContext context)
		{
			return Task.FromResult(new Response
			{
				Status = "Hello",
				ErrorMessage = string.Empty
			});
		}
	}
}
