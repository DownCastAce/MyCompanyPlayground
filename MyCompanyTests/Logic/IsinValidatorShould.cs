using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCompanyPlayground.Logic;

namespace MyCompanyTests.Logic
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class IsinValidatorShould
    {
        [TestMethod]
        public void ReturnTrueForValidIsin()
        {
            string testWithCountryCodeAndAllNumbers = "IE123456789a";
            
            string testWithCountryCodeAndAlphaNumberCombination = "IE12345abcde";

            string testWithValidIsin = "DE000PAH0038";
            
            Assert.IsTrue(PayloadValidator.IsValid(testWithCountryCodeAndAllNumbers));
            Assert.IsTrue(PayloadValidator.IsValid(testWithCountryCodeAndAlphaNumberCombination));
            Assert.IsTrue(PayloadValidator.IsValid(testWithValidIsin));

        } 
        
        [TestMethod]
        public void ReturnFalseForInvalidIsin()
        {
            string testOnlyNumbers = "12345";
            
            string testWithOnlyCharaters = "helloWorld";
            
            string testWithIsinMissingLastValue = "US037833100";

            string testWithSpecialCharacter = "IE123456789#";

            Assert.IsFalse(PayloadValidator.IsValid(testOnlyNumbers));
            Assert.IsFalse(PayloadValidator.IsValid(testWithOnlyCharaters));
            Assert.IsFalse(PayloadValidator.IsValid(testWithIsinMissingLastValue));
            Assert.IsFalse(PayloadValidator.IsValid(testWithSpecialCharacter));

        } 
        
    }
}