// Copyright (c) Cognizant. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NLog.Targets;

namespace CTS.SmartEngg.Framework
{
    public class DBHelper
    {
        public SqlConnectionStringBuilder GetSqlConnectionStringBuilder(string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionBuilder.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Disabled;

            return connectionBuilder;
        }

        /// <summary>
        /// This Method Is Used To InsertDatatable
        /// </summary>
        /// <param name="dtBulk"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string InsertDatatable(DataTable dtBulk, string tableName, string connectionString)
        {
            string strResult = string.Empty;

            SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString);
            if (dtBulk != null && dtBulk.Rows.Count > 0)
            {
                for (int y = 0; y < dtBulk.Columns.Count; y++)
                {
                    bulkCopy.ColumnMappings.Add(dtBulk.Columns[y].ColumnName, dtBulk.Columns[y].ColumnName);
                }

                strResult = string.Empty;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(dtBulk);
                }
            }

            return strResult;
        }

        /// <summary>
        /// This is CreatePara Common Function
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SqlParameter[] CreatePara<TSource>(TSource data)
        {
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            SqlParameter[] Cusprms = new SqlParameter[props.Length];

            StringBuilder paramName = new StringBuilder();
            StringBuilder strAtSymbol = new StringBuilder("@");
            StringBuilder typeName = new StringBuilder();
            StringBuilder strAVL = new StringBuilder("AVL.");
            StringBuilder strTVP = new StringBuilder("TVP_");

            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(data, null);
                paramName.Clear();
                paramName.Append(strAtSymbol).Append(props[i].Name);
                if (values[i].GetType().Name.ToUpper().Trim() == "DATATABLE")
                {
                    typeName.Clear();
                    typeName.Append(strAVL).Append(strTVP).Append(props[i].Name);
                    Cusprms[i] = new SqlParameter(Convert.ToString(paramName), values[i]);
                    Cusprms[i].SqlDbType = SqlDbType.Structured;
                    Cusprms[i].TypeName = Convert.ToString(typeName);
                }
                else
                {
                    Cusprms[i] = new SqlParameter(Convert.ToString(paramName), values[i]);
                }
            }

            return Cusprms;
        }

        /// <summary>
        /// Method used to create param
        /// </summary>
        /// <param name="ParaNama">Paramenter name</param>
        /// <param name="ParaValue">Paramenter value</param>
        /// <returns>Sql Parameter</returns>
        public SqlParameter[] CreateSinglePara(string ParaNama, string ParaValue)
        {
            SqlParameter[] Cusprms = new SqlParameter[1];

            Cusprms[0] = new SqlParameter("@" + ParaNama, ParaValue);

            return Cusprms;
        }

        /// <summary>
        /// Method used to get result from script
        /// </summary>
        /// <param name="sp">Procedure</param>
        /// <param name="parameter">Parametre</param>
        /// <returns></returns>
        public virtual DataTable GetTableFromSP(string sp, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();
                    command.Parameters.AddRange(parameter);

                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);

                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                }
            }
        }
        /// <summary>
        /// Used to call the gateway server from SP to retrive the Dataset
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual DataSet GetDatasetFromSP(string sp, string connectionString)
        {
            DataSet dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sp, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = connection.ConnectionTimeout;
                        connection.Open();

                        (new SqlDataAdapter(command)).Fill(dataSet);
                        command.Parameters.Clear();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        /// <summary>
        /// used to call the Gatway server SP
        /// </summary>
        /// <param name="sp">Procdure</param>
        /// <returns>dataset values</returns>
        public DataTable GetTableFromSP(string sp, string connectionString)
        {

            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    connection.Open();

                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);
                    command.Parameters.Clear();

                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                }
            }

        }

        /// <summary>
        /// Used to call Development DB SP 
        /// </summary>
        /// <param name="sp">Procedure</param>
        /// <returns>Data Table values</returns>
        public DataTable GetDataTableFromSP(string sp, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    connection.Open();
                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);
                    command.Parameters.Clear();

                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                }
            }
        }
                
        /// <summary>
        /// To Call Dev DB SP
        /// </summary>
        /// <param name="sp">Procedure</param>
        /// <param name="parameter">Parameter</param>
        public void ExecuteNonQuery(string sp, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    connection.Open();
                    command.Parameters.AddRange(parameter);
                    command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// To Call Dev DB SP
        /// </summary>
        /// <param name="sp">procedure</param>
        /// <param name="parameter">parameter</param>
        public void ExecuteNonQuery(string sp, SqlParameter parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    connection.Open();
                    parameter.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(parameter);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// To call dev db
        /// </summary>
        /// <param name="sp">procedure</param>
        /// <param name="parameter">parameter</param>
        /// <param name="parameters">parameters</param>
        public void ExecuteNonQuery(string sp, SqlParameter parameter, SqlParameter[] parameters, string connectionString)
        {

            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    connection.Open();
                    parameter.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(parameter);
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// This is to Get Table From SP
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="prms"></param>
        /// <returns></returns>
        public void ExecuteNonQurey(string sp, SqlParameter[] prms, string connectionString, string outParameterName, out string OutputParameter)
        {
            OutputParameter = string.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    connection.Open();
                    command.Parameters.AddRange(prms);
                    command.ExecuteNonQuery();
                    OutputParameter = (string)command.Parameters[outParameterName].Value;
                }
            }
        }

        /// <summary>
        /// To Call Dev DB's SP
        /// </summary>
        /// <param name="sp">procdure</param>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public DataTable GetTableRow(string sp, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    command.Parameters.AddRange(parameter);
                    connection.Open();
                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);

                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                }
            }
        }

        /// <summary>
        /// To Call Dev DB'S SP
        /// </summary>
        /// <param name="sp">procedure</param>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public DataSet GetDatasetFromSP(string sp, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    connection.Open();
                    command.Parameters.AddRange(parameter);
                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);
                    command.Parameters.Clear();
                    return dataSet;
                }
            }
        }

        /// <summary>
        /// To call Dev DB'S SP
        /// </summary>
        /// <param name="sp">procdure</param>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public int ExecuteNonQueryReturn(string sp, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    connection.Open();
                    command.Parameters.AddRange(parameter);
                    int result = command.ExecuteNonQuery();
                    return result;
                }
            }
        }

        /// <summary>
        /// To Call dev db's sp
        /// </summary>
        /// <param name="sp">procedure</param>
        public void ExecuteNonQuery(string sp, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// GetDataTableAsync
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="parameter"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<DataTable> FetchDataTableFromSpAsync(string spName, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);
            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddRange(parameter);
                    await connection.OpenAsync().ConfigureAwait(false);
                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);

                    return dataSet.Tables.Count > 0 ? dataSet.Tables[0] : null;
                }
            }
        }
        /// <summary>
        /// FetchDatasetFromSP
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="parameter"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<DataSet> FetchDatasetFromSpAsync(string spName, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);
            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = connection.ConnectionTimeout;
                    command.Parameters.AddRange(parameter);
                    await connection.OpenAsync().ConfigureAwait(false);
                    DataSet dataSet = new DataSet();
                    dataSet.Locale = CultureInfo.InvariantCulture;
                    (new SqlDataAdapter(command)).Fill(dataSet);
                    command.Parameters.Clear();
                    return dataSet;
                }
            }
        }
        /// <summary>
        /// ExecuteNonQueryAsync
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameter"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task ExecuteNonQueryAsync(string spName, SqlParameter[] parameter, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(spName, connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = connection.ConnectionTimeout
                })
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    command.Parameters.AddRange(parameter);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// with out paramerter
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="prms"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public int ExecuteNonQueryWithOutParam(string sp, SqlParameter[] prms, string connectionString)
        {
            int returnValue = 0;
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    connection.Open();
                    // Return value as parameter
                    SqlParameter outPutVal = new SqlParameter("@returnValue", SqlDbType.Int);
                    outPutVal.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outPutVal);
                    command.Parameters.AddRange(prms);
                    int result = command.ExecuteNonQuery();
                    if (outPutVal.Value != DBNull.Value)
                    {
                        returnValue = Convert.ToInt32(outPutVal.Value);
                    }
                    return returnValue;
                }

            }

        }
        /// <summary>
        /// Bulk insert for DB table
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="dt"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool BulkInsert(string TableName, DataTable dt, string connectionString)
        {
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionBuilder.ConnectionString))
            {
                bulkCopy.DestinationTableName = TableName;
                // Write from the source to the destination.
                bulkCopy.WriteToServer(dt);
                return true;
            }
        }

        /// <summary>
        /// ExecuteScalar
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="prms"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sp, SqlParameter[] prms, string connectionString)
        {
            object returnValue = null;
            SqlConnectionStringBuilder connectionBuilder = GetSqlConnectionStringBuilder(connectionString);

            using (SqlConnection connection = new SqlConnection(connectionBuilder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout })
                {
                    connection.Open();
                    command.Parameters.AddRange(prms);
                    returnValue = command.ExecuteScalar();
                    return returnValue;
                }
            }
        }

        /// <summary>
        /// Convert type to data table
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="items">List of generic type</param>
        /// <returns>data table</returns>
        public DataTable ToDataTableSingleRow<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            dataTable.Locale = CultureInfo.InvariantCulture;
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() ==
                    typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }


        /// <summary>
        /// This is To DataTable 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="items">List of generic type</param>
        /// <returns>data table</returns>
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            dataTable.Locale = CultureInfo.InvariantCulture;
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                for (int i = 0; i < Props.Length; i++)
                {
                    dataTable.Rows.Add(Props[i].GetValue(item, null));
                }
            }
            return dataTable;
        }        
    }
}
