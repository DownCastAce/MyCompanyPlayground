using System.Collections.Generic;
using MyCompanyPlayground.Models;

namespace MyCompanyPlayground.Repo
{
    public interface IDataBase
    {
        void AddCompany (Company companyToAdd);
        Company GetCompanyById (int id);
        Company GetCompanyByIsin (string Isin);
        IList<Company> GetAllCompanies();
        int UpdateCompany(int id, Company companyToUpdate);
    }
}