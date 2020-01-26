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
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<Company>("SELECT * FROM CompanyRecords WHERE Id = @Id", new { Id = id });
                
                if (!output.Any())
                {
                    return null;
                }

                return output.First();
            }
        }

        public Company GetCompanyByIsin(string Isin)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<Company>("SELECT * FROM CompanyRecords WHERE Isin = @Isin", new { Isin = Isin });
                
                if (!output.Any())
                {
                    return null;
                }

                return output.First();
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

        public int UpdateCompany(int id, Company companyToUpdate)
        {
            int rowAffected = 0;
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    rowAffected = cnn.Execute("UPDATE CompanyRecords SET Name = @Name, Exchange = @Exchange, Ticker = @Ticker, Isin = @Isin, Website = @Website WHERE  Id = @id", new {companyToUpdate.Name, companyToUpdate.Exchange, companyToUpdate.Ticker, companyToUpdate.Isin, companyToUpdate.Website, id } );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return rowAffected;
        }
    }
}