using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCompanyPlayground.Logic;

namespace MyCompanyTests.Logic
{
    [TestClass]
    public class IsinValidatorShould
    {
        [TestMethod]
        public void ReturnTrueForValidIsin()
        {
            IsinValidator tester = new IsinValidator();
            
            string testWithCountryCodeAndAllNumbers = "IE1234567890";
            
            string testWithCountryCodeAndAlphaNumberCombination = "IE12345abcde";

            string testWithValidIsin = "DE000PAH0038";
            
            Assert.IsTrue(tester.IsValid(testWithCountryCodeAndAllNumbers));
            Assert.IsTrue(tester.IsValid(testWithCountryCodeAndAlphaNumberCombination));
            Assert.IsTrue(tester.IsValid(testWithValidIsin));

        } 
        
        [TestMethod]
        public void ReturnFalseForInvalidIsin()
        {
            IsinValidator tester = new IsinValidator();
            
            string testOnlyNumbers = "12345";
            
            string testWithOnlyCharaters = "helloWorld";
            
            string testWithIsinMissingLastValue = "US037833100";
            
            Assert.IsFalse(tester.IsValid(testOnlyNumbers));
            Assert.IsFalse(tester.IsValid(testWithOnlyCharaters));
            Assert.IsFalse(tester.IsValid(testWithIsinMissingLastValue));

        } 
        
    }
}