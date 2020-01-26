using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCompanyPlayground.Logic;
using MyCompanyPlayground.Models;
using MyCompanyPlayground.Protos;
using MyCompanyPlayground.Repo;

namespace MyCompanyPlayground.Services
{
	public class CompanyService : CompanyServices.CompanyServicesBase
	{
		private readonly ILogger<CompanyService> _logger;
		private readonly IOptions<ConnectionStrings> _settings;
		private PayloadValidator _validater = new PayloadValidator();

		public CompanyService(ILogger<CompanyService> logger, IOptions<ConnectionStrings> settings)
		{
			_logger = logger;
			_settings = settings;
		}
		
		public override Task<Response> AddCompany(CompanyPayload request, ServerCallContext context)
		{
			if (!_validater.IsValid(request))
			{
				return Task.FromResult(new Response
				{
					Status = "400",
					ErrorMessage = $"Payload is missing Mandatory Fields : {string.Join(", ",_validater.MissingMandatoryFields.Select(l => l))}"
				});
			}

			if (DoesAlreadyExit(request.Isin))
			{
				return Task.FromResult(new Response
				{
					Status = "400",
					ErrorMessage = $"Record for ISIN : ({request.Isin}) already exists"
				});
			}
			
			Company companyToAdd = new Company(request);
			
			SqliteDataAccess database = new SqliteDataAccess(_settings);
			database.AddCompany(companyToAdd);
			
			return Task.FromResult(new Response
			{
				Status = "200",
				ErrorMessage = string.Empty
			});
		}

		
		public override Task<CompaniesResponse> GetAllCompanies(Empty request, ServerCallContext context)
		{
			SqliteDataAccess database = new SqliteDataAccess(_settings);
			var output = database.GetAllCompanies();

			var response = new CompaniesResponse();
			response.Company.AddRange(GenerateCompanyResponse(output));
			
			return Task.FromResult(response);
		}

		public override Task<CompanyPayload> GetCompanyByIsin(CompanyIsin request, ServerCallContext context)
		{
			SqliteDataAccess database = new SqliteDataAccess(_settings);
			var result = database.GetCompanyByIsin(request.Isin);
			
			CompanyPayload response = new CompanyPayload
			{
				CompanyName = result.Name,
				Exchange = result.Exchange,
				Ticker = result.Ticker,
				Isin = result.Isin,
				Website = result.Website
			};

			return Task.FromResult(response);
		}
		
		private bool DoesAlreadyExit(string requestIsin)
		{
			SqliteDataAccess database = new SqliteDataAccess(_settings);
			var result = database.GetCompanyByIsin(requestIsin);

			return result != null && !string.IsNullOrWhiteSpace(result.Isin);
		}

		private IEnumerable<CompanyPayload> GenerateCompanyResponse(IList<Company> output)
		{
			foreach (var company in output)
			{
				yield return new CompanyPayload
				{
					CompanyName = company.Name,
					Exchange = company.Exchange,
					Isin = company.Isin,
					Ticker = company.Ticker,
					Website = company.Website
				};
			}
		}
	}
}
