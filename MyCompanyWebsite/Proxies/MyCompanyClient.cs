using System;
using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using MyCompanyPlayground.Protos;
using MyCompanyWebsite.Data;

namespace MyCompanyWebsite.Proxies
{
	public class MyCompanyClient
	{
		private CompanyServices.CompanyServicesClient _client; 
		
		public MyCompanyClient()
		{
			var channel = GrpcChannel.ForAddress("https://localhost:5001");
			_client = new CompanyServices.CompanyServicesClient(channel);
		}

		public IEnumerable<Company> GetAllCompanies()
		{
			var response = _client.GetAllCompanies(new GetAllCompaniesRequest());

			foreach(var company in response.Company)
			{
				yield return new Company
				{
					Name = company.CompanyName,
					Exchange = company.Exchange,
					Ticker = company.Ticker,
					Isin = company.Isin,
					Website = company.Website
				};
			}
		}

		public Company FindCompanyByIsin(string isin)
		{
			var request = new GetCompanyByIsinRequest 
			{
				Isin = isin
			};
			
			var response = _client.GetCompanyByIsin(request);

			return new Company 
			{
				Name = response.Company.CompanyName,
				Exchange = response.Company.CompanyName,
				Isin = response.Company.Isin,
				Ticker = response.Company.Ticker,
				Website = response.Company.Website
			};

		}

		public Company FindCompanyById(int id)
		{
			var request = new GetCompanyByIdRequest
			{
				Id = id
			};

			var response = _client.GetCompanyById(request);

			return new Company
			{
				Name = response.Company.CompanyName,
				Exchange = response.Company.CompanyName,
				Isin = response.Company.Isin,
				Ticker = response.Company.Ticker,
				Website = response.Company.Website
			};

		}

		public string AddCompany(Company company)
		{
			AddCompanyRequest request = new AddCompanyRequest 
			{
				Company = new CompanyPayload
				{
					CompanyName = company.Name,
					Exchange = company.Exchange,
					Ticker = company.Ticker,
					Isin = company.Isin,
					Website = company.Website ?? string.Empty
				}
			};
			try
			{
				var response = _client.AddCompany(request);
				return response.Message;
			}
			catch (RpcException exception)
			{
				return exception.Status.Detail;
			}
		}

		public string UpdateCompany(UpdateCompany updateCompany)
		{
			UpdateCompanyDetailsRequest request = new UpdateCompanyDetailsRequest
			{
				Id = int.Parse(updateCompany.Id),
				Company = new CompanyPayload
				{
					CompanyName = updateCompany.Name,
					Exchange = updateCompany.Exchange,
					Ticker = updateCompany.Ticker,
					Isin = updateCompany.Isin,
					Website = updateCompany.Website,
				}
			};
			try
			{
				var response = _client.UpdateCompanyDetails(request);
				return response.Message;
			}
			catch (RpcException exception)
			{
				return exception.Status.Detail;
			}
		}
	}
}
