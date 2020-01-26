using System.Collections.Generic;
using MyCompanyPlayground.Models;

namespace MyCompanyPlayground.Database
{
    public interface IDataBase
    {
        int AddCompany (Company companyToAdd);
        IEnumerable<Company> GetCompanyById (int id);
        IEnumerable<Company> GetCompanyByIsin (string Isin);
        IEnumerable<Company> GetAllCompanies();
        int UpdateCompany(int id, Company companyToUpdate);
    }
}