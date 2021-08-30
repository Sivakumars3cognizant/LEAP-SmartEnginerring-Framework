using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTS.SmartEngg.Framework
{
    public interface IMySqlHelper   {

        IList<T> Query<T>(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null);
        T QuerySingle<T>(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null);
        T QueryFirst<T>(string query, string connectionString, DynamicParameters parameters = null);
        /*For Insert, Update & Delete*/
        void Execute(string query, string connectionString, CommandType cmdType, Dictionary<string, object> parameters = null);
        T Insert<T>(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null);
        T Update<T>(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null);
        int ExecuteQuery(string query, string connectionString, CommandType commandType, Dictionary<string, object> parameters = null);
        
    }
}
