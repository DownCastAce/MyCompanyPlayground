using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyCompanyPlayground.Models;
using MyCompanyPlayground.Protos;
using MyCompanyPlayground.Repo;
using MyCompanyPlayground.Services;

namespace MyCompanyTests.Services
{
    [TestClass]
    public class CompanyServiceShould
    {
        private const string ValidIsin = "IE1234567890";
        private const string InvalidIsin = "IE12345";
        private CompanyPayload validTestCompanyPayload;
        private Company validTestCompany;
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
            
            database = new Mock<IDataBase>();
            settings = new Mock<IOptions<ConnectionStrings>>();
            logger = new Mock<ILogger<CompanyService>>();
        }
        
        [TestMethod]
        public void AddValidCompany_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            var testResponse = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(testResponse.Result);
            Assert.AreEqual("Company Successfully Added", testResponse.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForMissingCompanyName_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            testPayload.Company.CompanyName = string.Empty;
            
            var testResponse = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(testResponse.Result);
            Assert.AreEqual("Payload is missing Mandatory Fields : CompanyName", testResponse.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForInvalidIsin_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            testPayload.Company.Isin = InvalidIsin;

            var testResponse = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(testResponse.Result);
            Assert.AreEqual($"Invalid ISIN : ({InvalidIsin})", testResponse.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForDuplicateIsin_AddCompany()
        {
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            database.Setup(x => x.GetCompanyByIsin(ValidIsin)).Returns(validTestCompany);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompanyPayload
            };

            var testResponse = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(testResponse.Result);
            Assert.AreEqual($"Record for ISIN : ({validTestCompanyPayload.Isin}) already exists", testResponse.Result.Message);
        }

        [TestMethod]
        public void GetAllCompaniesInDatabase_GetAllCompanies()
        {
            database.Setup(x => x.GetAllCompanies()).Returns(new List<Company>{ validTestCompany });
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
            var testResponse = tester.GetAllCompanies(new Empty(), null);
            
            Assert.IsNotNull(testResponse.Result.Company);
            Assert.AreEqual(1, testResponse.Result.Company.Count);
            Assert.AreEqual(string.Empty, testResponse.Result.Message);
        }

        [TestMethod]
        public void GetCompanyByIsinWithValidIsin_GetCompanyById()
        {
            database.Setup(x => x.GetCompanyByIsin(validTestCompanyPayload.Isin)).Returns(validTestCompany);
            
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
        public void GetErrorMessageWithInvalidIsin_GetCompanyByIsin()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new GetCompanyByIsinRequest
            {
                Isin = InvalidIsin
            };

            var testResponse = tester.GetCompanyByIsin(testRequest, null);
            
            Assert.IsNull(testResponse.Result.Company);
            Assert.AreEqual($"Invalid ISIN : {InvalidIsin}", testResponse.Result.Message);
        }

        [TestMethod]
        public void GetCompanyByIdWithValidId_GetCompanyById()
        {
            database.Setup(x => x.GetCompanyById(1)).Returns(validTestCompany);
            
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
        public void ReturnErrorMessageForInvalidPayload_UpdateCompanyDetails()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new UpdateCompanyDetailsRequest
            {
                Company = new CompanyPayload(),
                Id = 1
            };

            var testResponse = tester.UpdateCompanyDetails(testRequest, null);
            
            Assert.IsNotNull(testResponse.Result.Message);
            Assert.AreEqual($"Payload is missing Mandatory Fields : CompanyName, Ticker, Exchange, Isin", testResponse.Result.Message);
        }
        
        [TestMethod]
        public void ReturnErrorMessageForInvalidIsin_UpdateCompanyDetails()
        {
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);

            var testRequest = new UpdateCompanyDetailsRequest
            {
                Company = validTestCompanyPayload,
                Id = 1
            };

            testRequest.Company.Isin = InvalidIsin;
            
            var testResponse = tester.UpdateCompanyDetails(testRequest, null);
            
            Assert.IsNotNull(testResponse.Result.Message);
            Assert.AreEqual($"Invalid ISIN : ({InvalidIsin})", testResponse.Result.Message);
        }
    }
}