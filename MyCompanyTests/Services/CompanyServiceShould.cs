using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyCompanyPlayground.Database;
using MyCompanyPlayground.Models;
using MyCompanyPlayground.Protos;
using MyCompanyPlayground.Services;

namespace MyCompanyTests.Services
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CompanyServiceShould
    {
        private const string ValidIsin = "IE1234567890";
        private const string InvalidIsin = "IE12345";
        private CompanyPayload validTestCompanyPayload;
        private Company validTestCompany;
        private IList<Company> ValidCompanys = new List<Company>();
        
        private Mock<ILogger<CompanyService>> logger;
        private Mock<IOptions<ConnectionStrings>> settings;
        private Mock<IDataBase> database;
        
        
        [TestInitialize]
        public void Setup()
        {
            validTestCompanyPayload = new CompanyPayload 
                {
                    CompanyName = "MyCompany",
                    Exchange = "MyCompany Exchange",
                    Ticker = "MyCo",
                    Isin = ValidIsin,
                    Website = "http://zombo.com/"
                };
            
            validTestCompany = new Company
            {
                Name = validTestCompanyPayload.CompanyName,
                Exchange = validTestCompanyPayload.Exchange,
                Ticker = validTestCompanyPayload.Ticker,
                Isin = validTestCompanyPayload.Isin,
                Website = validTestCompanyPayload.Website
            };
            
            ValidCompanys.Add(validTestCompany);
            database = new Mock<IDataBase>();
            settings = new Mock<IOptions<ConnectionStrings>>();
            logger = new Mock<ILogger<CompanyService>>();
        }
        
        [TestMethod]
        public void AddValidCompany_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>())).Returns(1);
            database.Setup(x => x.GetCompanyByIsin(ValidIsin)).Returns(ValidCompanys);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            testPayload.Company.Isin = "US1234567890";

            var testResponse = tester.AddCompany(testPayload, null);
            Assert.IsNotNull(testResponse.Result);
            Assert.AreEqual("Company Successfully Added", testResponse.Result.Message);
        }
        
        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void SendErrorMessageForMissingCompanyName_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            testPayload.Company.CompanyName = string.Empty;
            
            tester.AddCompany(testPayload, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void SendErrorMessageForInvalidIsin_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            testPayload.Company.Isin = InvalidIsin;

            tester.AddCompany(testPayload, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void SendErrorMessageForDuplicateIsin_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            database.Setup(x => x.GetCompanyByIsin(ValidIsin)).Returns(ValidCompanys);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            tester.AddCompany(testPayload, null);
        }

        [TestMethod]
        public void GetAllCompaniesInDatabase_GetAllCompanies()
        {
            database.Setup(x => x.GetAllCompanies()).Returns(new List<Company>{ validTestCompany });
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
            var testResponse = tester.GetAllCompanies(new GetAllCompaniesRequest(), null);
            
            Assert.IsNotNull(testResponse.Result.Company);
            Assert.AreEqual(1, testResponse.Result.Company.Count);
            Assert.AreEqual(string.Empty, testResponse.Result.Message);
        }

        [TestMethod]
        public void GetCompanyByIsinWithValidIsin_GetCompanyByIsin()
        {
            database.Setup(x => x.GetCompanyByIsin(validTestCompanyPayload.Isin)).Returns(ValidCompanys);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new GetCompanyByIsinRequest
            {
                Isin = ValidIsin
            };

            var testResponse = tester.GetCompanyByIsin(testRequest, null);
            
            Assert.IsNotNull(testResponse.Result.Company);
            Assert.AreEqual(validTestCompany.Name, testResponse.Result.Company.CompanyName);
            Assert.AreEqual(validTestCompany.Exchange, testResponse.Result.Company.Exchange);
            Assert.AreEqual(validTestCompany.Ticker, testResponse.Result.Company.Ticker);
            Assert.AreEqual(validTestCompany.Isin, testResponse.Result.Company.Isin);
            Assert.AreEqual(validTestCompany.Website, testResponse.Result.Company.Website);
            Assert.AreEqual(string.Empty, testResponse.Result.Message);
        }
        
        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void GetErrorMessageWithInvalidIsin_GetCompanyByIsin()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new GetCompanyByIsinRequest
            {
                Isin = InvalidIsin
            };

            tester.GetCompanyByIsin(testRequest, null);
        }
        
        [TestMethod]
        public void GetCompanyByIdWithValidId_GetCompanyById()
        {
            database.Setup(x => x.GetCompanyById(1)).Returns(ValidCompanys);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new GetCompanyByIdRequest
            {
                Id = 1
            };

            var testResponse = tester.GetCompanyById(testRequest, null);
            
            Assert.IsNotNull(testResponse.Result.Company);
            Assert.AreEqual(validTestCompany.Name, testResponse.Result.Company.CompanyName);
            Assert.AreEqual(validTestCompany.Exchange, testResponse.Result.Company.Exchange);
            Assert.AreEqual(validTestCompany.Ticker, testResponse.Result.Company.Ticker);
            Assert.AreEqual(validTestCompany.Isin, testResponse.Result.Company.Isin);
            Assert.AreEqual(validTestCompany.Website, testResponse.Result.Company.Website);
            Assert.AreEqual(string.Empty, testResponse.Result.Message);
        }

        [TestMethod]
        public void UpdateExistingCompanyWithNewDetails_UpdateCompanyDetails()
        {
            database.Setup(x => x.UpdateCompany(1, It.IsAny<Company>())).Returns(1);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new UpdateCompanyDetailsRequest
            {
                Company = validTestCompanyPayload,
                Id = 1
            };

            var testResponse = tester.UpdateCompanyDetails(testRequest, null);
            
            Assert.IsNotNull(testResponse.Result.Message);
            Assert.AreEqual("Company (1) was successfully updated", testResponse.Result.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void ReturnErrorMessageForInvalidPayload_UpdateCompanyDetails()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new UpdateCompanyDetailsRequest
            {
                Company = new CompanyPayload(),
                Id = 1
            };

            tester.UpdateCompanyDetails(testRequest, null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(RpcException))]
        public void ReturnErrorMessageForInvalidIsin_UpdateCompanyDetails()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new UpdateCompanyDetailsRequest
            {
                Company = validTestCompanyPayload,
                Id = 1
            };

            testRequest.Company.Isin = InvalidIsin;
            
            tester.UpdateCompanyDetails(testRequest, null);
        }
    }
}