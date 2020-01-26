using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCompanyPlayground.Database;
using MyCompanyPlayground.Logic;
using MyCompanyPlayground.Models;
using MyCompanyPlayground.Protos;

namespace MyCompanyPlayground.Services
{
	public class CompanyService : CompanyServices.CompanyServicesBase
	{
		private readonly ILogger<CompanyService> _logger;
		private readonly IOptions<ConnectionStrings> _settings;
		private readonly IDataBase _dataBase;
		
		public CompanyService(ILogger<CompanyService> logger, IOptions<ConnectionStrings> settings, IDataBase dataBase)
		{
			_logger = logger;
			_settings = settings;
			_dataBase = dataBase;
		}
		
		public override Task<AddCompanyResponse> AddCompany (AddCompanyRequest request, ServerCallContext context)
		{
			IList<string> mandatoryMissingFields = PayloadValidator.VerifyMandatoryFields(request.Company);
			
			if (mandatoryMissingFields.Any())
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Payload is missing Mandatory Fields"), $"Payload is missing Mandatory Fields : {string.Join(", ", mandatoryMissingFields.Select(l => l))}");
			}
			
			if (!PayloadValidator.IsValid(request.Company.Isin))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ISIN"), $"Invalid ISIN : ({request.Company.Isin})");
			}
			
			if (DoesAlreadyExit(request.Company.Isin))
			{
				throw new RpcException(new Status(StatusCode.AlreadyExists, "Company already exists with this ISIN"), $"Record for ISIN : ({request.Company.Isin}) already exists");
			}
			
			Company companyToAdd = new Company(request.Company);
			
			int rowsAffected = _dataBase.AddCompany(companyToAdd);
			
			return Task.FromResult(new AddCompanyResponse
			{
				Message = rowsAffected == 1 ? "Company Successfully Added" : "Failed to add Company"
			});
		}
		
		public override Task<GetAllCompaniesResponse> GetAllCompanies (GetAllCompaniesRequest request, ServerCallContext context)
		{
			IEnumerable<Company> output = _dataBase.GetAllCompanies();

			GetAllCompaniesResponse response = new GetAllCompaniesResponse();
			response.Company.AddRange(GenerateCompanyResponse(output));
			
			return Task.FromResult(response);
		}

		public override Task<GetCompanyByIsinResponse> GetCompanyByIsin (GetCompanyByIsinRequest request, ServerCallContext context)
		{
			if (!PayloadValidator.IsValid(request.Isin))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ISIN"), $"Invalid ISIN : ({request.Isin})");
			}
			
			IEnumerable<Company> result = _dataBase.GetCompanyByIsin(request.Isin);

			if (!result.Any())
			{
				throw new RpcException(new Status(StatusCode.NotFound, $"No Company matches search criteria"), $"Company for ISIN ({request.Isin}) doesn't exist.");
			}
			
			GetCompanyByIsinResponse response = new GetCompanyByIsinResponse
			{
				Company = new CompanyPayload{
					CompanyName = result.First().Name,
					Exchange = result.First().Exchange,
					Ticker = result.First().Ticker,
					Isin = result.First().Isin,
					Website = result.First().Website
				}
			};

			return Task.FromResult(response);
		}

		public override Task<GetCompanyByIdResponse> GetCompanyById (GetCompanyByIdRequest request, ServerCallContext context)
		{
			IEnumerable<Company> result = _dataBase.GetCompanyById(request.Id);
			
			if (!result.Any())
			{
				throw new RpcException(new Status(StatusCode.NotFound, $"No Company matches search criteria"), $"Company for Id ({request.Id}) doesn't exist.");
			}
			
			CompanyPayload company = null;
			if(result != null)
			{
				company = new CompanyPayload
				{
					CompanyName = result.First().Name,
					Exchange = result.First().Exchange,
					Ticker = result.First().Ticker,
					Isin = result.First().Isin,
					Website = result.First().Website
				};
			}
			
			return Task.FromResult(new GetCompanyByIdResponse
			{
				Company = company
			});
		}

		public override Task<UpdateCompanyDetailsResponse> UpdateCompanyDetails(UpdateCompanyDetailsRequest request, ServerCallContext context)
		{
			IList<string> mandatoryMissingFields = PayloadValidator.VerifyMandatoryFields(request.Company);
			
			if (mandatoryMissingFields.Any())
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Payload is missing Mandatory Fields"), $"Payload is missing Mandatory Fields : {string.Join(", ", mandatoryMissingFields.Select(l => l))}");
			}
			
			if (!PayloadValidator.IsValid(request.Company.Isin))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ISIN"), $"Invalid ISIN : ({request.Company.Isin})");
			}
			
			Company company = new Company(request.Company);
			
			int rowsAffected = _dataBase.UpdateCompany(request.Id, company);
			
			return Task.FromResult(new UpdateCompanyDetailsResponse
			{
				Message = rowsAffected == 1 ? $"Company ({request.Id}) was successfully updated" : "Update not successful"
			});
		}
		
		private bool DoesAlreadyExit(string requestIsin)
		{
			IEnumerable<Company> result = _dataBase.GetCompanyByIsin(requestIsin);
			
			return result.Any();
		}

		private IEnumerable<CompanyPayload> GenerateCompanyResponse(IEnumerable<Company> output)
		{
			foreach (Company company in output)
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
