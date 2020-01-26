using System.Text.RegularExpressions;

namespace MyCompanyPlayground.Logic
{
    public class IsinValidator
    {
        public bool IsValid(string isin)
        {
            Regex regex = new Regex("[a-zA-z]{2}[\\w]{10}");
            return regex.Match(isin).Success;
        }
    }
}