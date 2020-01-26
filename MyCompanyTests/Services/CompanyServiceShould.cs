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
        private CompanyPayload validTestCompany;

        [TestInitialize]
        public void Setup()
        {
            validTestCompany = new CompanyPayload 
                {
                    CompanyName = "MyCompany",
                    Exchange = "MyCompany Exchange",
                    Ticker = "MyCo",
                    Isin = "IE1234567890",
                    Website = "http://zombo.com/"
                };
        }
        
        [TestMethod]
        public void AddValidCompany()
        {
            Mock<ILogger<CompanyService>> logger = new Mock<ILogger<CompanyService>>();
            Mock<IOptions<ConnectionStrings>> settings = new Mock<IOptions<ConnectionStrings>>();
            
            Mock<IDataBase> database = new Mock<IDataBase>();
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompany
            };

            var response = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(response);
            Assert.AreEqual("Company Successfully Added", response.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForMissingCompanyName()
        {
            Mock<ILogger<CompanyService>> logger = new Mock<ILogger<CompanyService>>();
            Mock<IOptions<ConnectionStrings>> settings = new Mock<IOptions<ConnectionStrings>>();
            
            Mock<IDataBase> database = new Mock<IDataBase>();
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompany
            };

            testPayload.Company.CompanyName = string.Empty;
            
            var response = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(response);
            Assert.AreEqual("Payload is missing Mandatory Fields : CompanyName", response.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForInvalidIsin()
        {
            Mock<ILogger<CompanyService>> logger = new Mock<ILogger<CompanyService>>();
            Mock<IOptions<ConnectionStrings>> settings = new Mock<IOptions<ConnectionStrings>>();
            
            Mock<IDataBase> database = new Mock<IDataBase>();
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompany
            };

            string invalidIsin = "IE12345";
            testPayload.Company.Isin = invalidIsin;

            var response = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(response);
            Assert.AreEqual($"Invalid ISIN : ({invalidIsin})", response.Result.Message);
        }
        
        [TestMethod]
        public void SendErrorMessageForDuplicateIsin()
        {
            Mock<ILogger<CompanyService>> logger = new Mock<ILogger<CompanyService>>();
            Mock<IOptions<ConnectionStrings>> settings = new Mock<IOptions<ConnectionStrings>>();
            
            Company testCompany = new Company
            {
                Name = validTestCompany.CompanyName,
                Exchange = validTestCompany.Exchange,
                Ticker = validTestCompany.Ticker,
                Isin = validTestCompany.Isin,
                Website = validTestCompany.Website
            };
            
            Mock<IDataBase> database = new Mock<IDataBase>();
            database.Setup(x => x.AddCompany(It.IsAny<Company>()));
            database.Setup(x => x.GetCompanyByIsin("IE1234567890")).Returns(testCompany);
            
            CompanyService tester = new CompanyService(logger.Object, settings.Object, database.Object);
        
            AddCompanyRequest testPayload = new AddCompanyRequest
            {
                Company = validTestCompany
            };

            var response = tester.AddCompany(testPayload, null);
            
            Assert.IsNotNull(response);
            Assert.AreEqual($"Record for ISIN : ({validTestCompany.Isin}) already exists", response.Result.Message);
        }
        
    }
}