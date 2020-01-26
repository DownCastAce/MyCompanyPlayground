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
		private readonly IDataBase _dataBase;
		private PayloadValidator _validater = new PayloadValidator();

		public CompanyService(ILogger<CompanyService> logger, IOptions<ConnectionStrings> settings, IDataBase dataBase)
		{
			_logger = logger;
			_settings = settings;
			_dataBase = dataBase;
		}
		
		public override Task<AddCompanyResponse> AddCompany (AddCompanyRequest request, ServerCallContext context)
		{
			if (!_validater.IsValid(request.Company))
			{
				return Task.FromResult(new AddCompanyResponse
				{
					Message = $"Payload is missing Mandatory Fields : {string.Join(", ",_validater.MissingMandatoryFields.Select(l => l))}"
				});
			}
			
			IsinValidator test = new IsinValidator();
			if (!test.IsValid(request.Company.Isin))
			{
				return Task.FromResult(new AddCompanyResponse
				{
					Message = $"Invalid ISIN : ({request.Company.Isin})"
				});
			}
			
			if (DoesAlreadyExit(request.Company.Isin))
			{
				return Task.FromResult(new AddCompanyResponse
				{
					Message = $"Record for ISIN : ({request.Company.Isin}) already exists"
				});
			}
			
			Company companyToAdd = new Company(request.Company);
			
			_dataBase.AddCompany(companyToAdd);
			
			return Task.FromResult(new AddCompanyResponse
			{
				Message = "Company Successfully Added"
			});
		}
		
		public override Task<GetAllCompaniesResponse> GetAllCompanies (Empty request, ServerCallContext context)
		{
			var output = _dataBase.GetAllCompanies();

			var response = new GetAllCompaniesResponse();
			response.Company.AddRange(GenerateCompanyResponse(output));
			
			return Task.FromResult(response);
		}

		public override Task<GetCompanyByIsinResponse> GetCompanyByIsin (GetCompanyByIsinRequest request, ServerCallContext context)
		{
			IsinValidator test = new IsinValidator();
			if (!test.IsValid(request.Isin))
			{
				return Task.FromResult(new GetCompanyByIsinResponse
				{
					Company = null,
					Message = $"Invalid ISIN : {request.Isin}"
				});
			}
			
			var result = _dataBase.GetCompanyByIsin(request.Isin);
			
			GetCompanyByIsinResponse response = new GetCompanyByIsinResponse
			{
				Company = new CompanyPayload{
					CompanyName = result.Name,
					Exchange = result.Exchange,
					Ticker = result.Ticker,
					Isin = result.Isin,
					Website = result.Website
				},
				Message = string.Empty
			};

			return Task.FromResult(response);
		}

		public override Task<GetCompanyByIdResponse> GetCompanyById (GetCompanyByIdRequest request, ServerCallContext context)
		{
			var result = _dataBase.GetCompanyById(request.Id);
			
			CompanyPayload company = null;
			if(result != null)
			{
				company = new CompanyPayload
				{
					CompanyName = result?.Name,
					Exchange = result?.Exchange,
					Ticker = result?.Ticker,
					Isin = result?.Isin,
					Website = result?.Website
				};
			}
			
			return Task.FromResult(new GetCompanyByIdResponse
			{
				Company = company,
				Message = result == null ? $"No company found for ID : ({request.Id})" : string.Empty
			});
		}

		public override Task<UpdateCompanyDetailsResponse> UpdateCompanyDetails(UpdateCompanyDetailsRequest request, ServerCallContext context)
		{
			if (!_validater.IsValid(request.Company))
			{
				return Task.FromResult(new UpdateCompanyDetailsResponse
				{
					Message= $"Payload is missing Mandatory Fields : {string.Join(", ",_validater.MissingMandatoryFields.Select(l => l))}"
				});
			}
			
			IsinValidator test = new IsinValidator();
			if (!test.IsValid(request.Company.Isin))
			{
				return Task.FromResult(new UpdateCompanyDetailsResponse
				{
					Message = $"Invalid ISIN : ({request.Company.Isin})"
				});
			}
			
			Company company = new Company
			{
				Name = request.Company.CompanyName,
				Exchange = request.Company.Exchange,
				Ticker = request.Company.Ticker,
				Isin = request.Company.Isin,
				Website = request.Company.Website
			};
			
			int rowsAffected = _dataBase.UpdateCompany(request.Id, company);
			
			return Task.FromResult(new UpdateCompanyDetailsResponse
			{
				Message = rowsAffected == 1 ? $"Company ({request.Id}) was successfully updated" : "Update not successful"
			});
		}
		
		private bool DoesAlreadyExit(string requestIsin)
		{
			var result = _dataBase.GetCompanyByIsin(requestIsin);

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
