using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using MyCompanyPlayground.Models;

namespace MyCompanyPlayground.Repo
{
    public class SqliteDataAccess : IDataBase
    {
        public string ConnectionString { get; set; }

        public SqliteDataAccess(IOptions<ConnectionStrings> settings)
        {
            ConnectionString = settings.Value.Sqlite;
        }
        
        public void AddCompany(Company companyToAdd)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Execute("INSERT INTO CompanyRecords (Name, Exchange, Ticker, Isin, Website) VALUES (@Name, @Exchange, @Ticker, @Isin, @Website)", companyToAdd);
            }
        }

        public Company GetCompanyById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Company GetCompanyByIsin(string Isin)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    var output = cnn.Query<Company>("SELECT * FROM CompanyRecords WHERE Isin = @Isin", new { Isin = Isin });
                    if (!output.Any())
                    {
                        return null;
                    }
                    else
                    {
                        return output.First();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IList<Company> GetAllCompanies()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<Company>("SELECT * FROM CompanyRecords", new DynamicParameters());
                return output.ToList();
            }
        }

        public void UpdateCompany(Company companyToUpdate)
        {
            throw new System.NotImplementedException();
        }
    }
}