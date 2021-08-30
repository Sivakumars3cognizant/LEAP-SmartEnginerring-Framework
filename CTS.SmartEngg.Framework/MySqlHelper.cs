using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
[assembly: System.Runtime.InteropServices.ComVisible(false)]

namespace CTS.SmartEngg.Framework
{
    public class MySqlHelper  : IMySqlHelper
    {

        public IList<T> Query<T>(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                DynamicParameters dbArgs = new DynamicParameters();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pair in parameters) 
                    {
                        dbArgs.Add(pair.Key, pair.Value); 
                    }
                }
                return objMySQLCon.Query<T>(query, dbArgs, null, false, null, cmdType).ToList();
            }
        }

        public T QuerySingle<T>(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                DynamicParameters dbArgs = new DynamicParameters();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pair in parameters) 
                    {
                        dbArgs.Add(pair.Key, pair.Value);
                    }
                }
                return objMySQLCon.QuerySingle<T>(query, parameters, null, null, cmdType);
            }
        }

        public T QuerySingleOrDefault<T>(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                DynamicParameters dbArgs = new DynamicParameters();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pair in parameters) 
                    { 
                        dbArgs.Add(pair.Key, pair.Value); 
                    }
                }
                return objMySQLCon.QuerySingleOrDefault<T>(query, parameters, null, null, cmdType);
            }
        }

        public T QueryFirst<T>(string query, string connectionString, DynamicParameters parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                return objMySQLCon.QueryFirst<T>(query, parameters);
            }
        }

        public T QueryFirstOrDefault<T>(string query, string connectionString, DynamicParameters parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                return objMySQLCon.QueryFirstOrDefault<T>(query, parameters);
            }
        }

        /*For Insert, Update & Delete*/
        public void Execute(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                DynamicParameters dbArgs = new DynamicParameters();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pair in parameters) 
                    { 
                        dbArgs.Add(pair.Key, pair.Value);
                    }
                }
                if (query == "Sample_upload")
                {
                    objMySQLCon.Execute(query, parameters, null, 100, cmdType);
                }
                else
                {
                    objMySQLCon.Execute(query, parameters, null, null, cmdType);
                }
            }
        }
                
        public T Insert<T>(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null)
        {
            T result;
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            result = db.Query<T>(query, parameters, commandType: commandType, transaction: tran).FirstOrDefault();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                db.Close();
            }
            return result;
        }
                
        public T Update<T>(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null)
        {
            T result;
            using (IDbConnection db = new MySqlConnection(connectionString)) 
            {
                db.Open();
                using (var tran = db.BeginTransaction())
                {
                    try
                    {
                        result = db.Query<T>(query, parameters, commandType: commandType , transaction: tran).FirstOrDefault();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
                db.Close();
            }
            return result;
        }

        public string ExecuteScalar(List<string> queries, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (MySqlConnection objMySQLCon = new MySqlConnection(connectionString))
            {
                DynamicParameters dbArgs = new DynamicParameters();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var pair in parameters)
                    {
                        dbArgs.Add(pair.Key, pair.Value);
                    }
                    parameters.Add("@Scope_Identity", null);
                }
                objMySQLCon.Open();
                var transaction = objMySQLCon.BeginTransaction();
                foreach (string qry in queries)
                {
                    parameters["@Scope_Identity"] = objMySQLCon.ExecuteScalar(qry, parameters, transaction, null, cmdType);
                }
                transaction.Commit();
                objMySQLCon.Close();
                return Convert.ToString(parameters["@Scope_Identity"]);
            }
        }

        public int ExecuteQuery(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null)
        {
            int result;
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                result = db.Query<int>(query, parameters, commandType: commandType).FirstOrDefault();
            }
            return result;
        }
    }
}
