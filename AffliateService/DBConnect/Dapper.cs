using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Telemedicine.Data
{
    public class AppDapper : IDapper
    {
        private readonly IConfiguration _config;
        private string Connectionstring;
        private string MarketPlaceConnectionString;

        public AppDapper(IConfiguration config)
        {
            _config = config;
            Connectionstring = "DefaultConnection";
            MarketPlaceConnectionString = "MarketPlaceConnection";
        }

        public DbConnection GetDbconnection()
        {
            return new SqlConnection(_config.GetConnectionString(Connectionstring));
        }

        public async Task<int> Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await Task.Run(() => db.Execute(sp, parms, commandType: commandType));
        }

        public async Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text, string _connectionString = "")
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                Connectionstring = MarketPlaceConnectionString;
            }
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));

            return await Task.Run(() => db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault());
        }

        public async Task<List<T>> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure, string _connectionString = "")
        {
            //Timeout in 4mins = 240 secs
            if(!string.IsNullOrEmpty(_connectionString))
            {
                Connectionstring = MarketPlaceConnectionString;
            }
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await Task.Run(() =>     db.Query<T>(sp, parms, commandType: commandType, commandTimeout: 240).ToList());
        }

        public async Task<IEnumerable<T>> GetAllAsyncFast<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            //Timeout in 2mins = 120 secs
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            return await Task.Run(() => db.Query<T>(sp, parms, commandType: commandType, commandTimeout:240 ));
        }

        public async Task<T> Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            if (db.State == ConnectionState.Closed)
                db.Open();
            using var tran = db.BeginTransaction();
            try
            {
                try
                {
                    result = await Task.Run(() => db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault());
                    
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                tran.Commit();
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public async Task<T> Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = await Task.Run(() => db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault());
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public async Task<string> GetJSONStringAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(Connectionstring));

            var reader = await Task.Run(() => db.ExecuteReader(sp, parms, commandType: commandType));
            while (reader.Read())
            {
                sb.Append(reader.GetString(0));
            }
            string result = sb.ToString();
            return result;
        }

        public void Dispose()
        {

        }
    }
}