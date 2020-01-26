using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Options;
using MyCompanyPlayground.Models;

namespace MyCompanyPlayground.Database
{
    public class SqliteDataAccess : IDataBase
    {
        private string _connectionString { get; set; }

        public SqliteDataAccess(IOptions<ConnectionStrings> settings)
        {
            _connectionString = settings.Value.Sqlite;
        }
        
        
        /// <summary>
        /// Adds a Company Record into the CompanyRecords Table
        /// </summary>
        /// <param name="companyToAdd"></param>
        public int AddCompany(Company companyToAdd)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return cnn.Execute("INSERT INTO CompanyRecords (Name, Exchange, Ticker, Isin, Website) VALUES (@Name, @Exchange, @Ticker, @Isin, @Website)", companyToAdd);
            }
        }

        /// <summary>
        /// Retrieves a Company Record based on the Id from the CompanyRecords Table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Company> GetCompanyById(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return cnn.Query<Company>("SELECT * FROM CompanyRecords WHERE Id = @Id", new { Id = id });
            }
        }

        /// <summary>
        /// Retrieves a Company Record based on the ISIN from the CompanyRecords Table
        /// </summary>
        /// <param name="Isin"></param>
        /// <returns></returns>
        public IEnumerable<Company> GetCompanyByIsin(string Isin)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return cnn.Query<Company>("SELECT * FROM CompanyRecords WHERE Isin = @Isin", new { Isin = Isin });
            }
        }

        /// <summary>
        /// Retrieves All Company Records From the CompanyRecords Table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Company> GetAllCompanies()
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return cnn.Query<Company>("SELECT * FROM CompanyRecords", new DynamicParameters());
            }
        }

        /// <summary>
        /// Updates a specific Company Record for the given Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="companyToUpdate"></param>
        /// <returns></returns>
        public int UpdateCompany(int id, Company companyToUpdate)
        {
            int rowAffected = 0;
            
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                rowAffected = cnn.Execute("UPDATE CompanyRecords SET Name = @Name, Exchange = @Exchange, Ticker = @Ticker, Isin = @Isin, Website = @Website WHERE  Id = @id", new {companyToUpdate.Name, companyToUpdate.Exchange, companyToUpdate.Ticker, companyToUpdate.Isin, companyToUpdate.Website, id } );
            }

            return rowAffected;
        }
    }
}