using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace TravelApplication.Class.SQLBuilder
{
    /// <summary>
    /// InsertQueryBuilder class
    /// To build INSERT SQL query
    /// </summary>
    public class InsertQueryBuilder
    {
        private string insertQuery;
        private StringBuilder columnNameList;
        private StringBuilder columnValueList;
        private DbCommand dbCommand;

        /// <summary>
        /// Constructor with database table name in string
        /// </summary>
        /// <param name="tableName">database table name in string</param>
        public InsertQueryBuilder(string tableName)
        {
            insertQuery = "INSERT INTO " + tableName + " (";
            columnNameList = new StringBuilder();
            columnValueList = new StringBuilder();
            dbCommand = new OracleCommand();
        }

        /// <summary>
        /// This method will add a single column name in string and value object to INSERT query
        /// </summary>
        /// <param name="columnName">column name string</param>
        /// <param name="columnValue">column value object</param>
        public void AddColumnNameAndValue(string columnName, object columnValue)
        {
            if (!string.IsNullOrEmpty(columnNameList.ToString()))
            {
                columnNameList.Append(",");
                columnValueList.Append(",");
            }

            // To set column name
            columnNameList.Append(columnName);

            // To set column value
            columnValueList.Append(":").Append(columnName);
            if (ReferenceEquals(columnValue.GetType(), typeof(string)))
            {
                dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Varchar2, columnValue.ToString(), ParameterDirection.Input));
            }
            else if (ReferenceEquals(columnValue.GetType(), typeof(int)) || ReferenceEquals(columnValue.GetType(), typeof(long)))
            {
                dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Int32, Convert.ToInt32(columnValue), ParameterDirection.Input));
            }
            else if (ReferenceEquals(columnValue.GetType(), typeof(decimal)))
            {
                dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Double, Convert.ToDouble(columnValue), ParameterDirection.Input));
            }
            else if (ReferenceEquals(columnValue.GetType(), typeof(bool))
                        || ReferenceEquals(columnValue.GetType(), typeof(byte)))
            {
                dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Byte, Convert.ToByte(columnValue), ParameterDirection.Input));
            }
            else if (ReferenceEquals(columnValue.GetType(), typeof(DateTime)))
            {
                if (Convert.ToDateTime(columnValue) < Convert.ToDateTime("01/01/1950"))
                {
                    dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Date, null, ParameterDirection.Input));
                }
                else
                {
                    dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Date, Convert.ToDateTime(columnValue), ParameterDirection.Input));
                }
            }
            else
            {
                dbCommand.Parameters.Add(new OracleParameter(columnName, OracleDbType.Varchar2, columnValue.ToString(), ParameterDirection.Input));
            }
        }

        /// <summary>
        /// Returns final completed INSERT query string
        /// </summary>
        /// <returns>return INSERT query string</returns>
        public string GetCompletedSQL()
        {
            return insertQuery + columnNameList.ToString() + ") VALUES (" + columnValueList.ToString() + ")";
        }

        /// <summary>
        /// Returns OracleCommand instance that is used for this INSERT query
        /// </summary>
        /// <returns>returns OracleCommand instance</returns>
        public DbCommand GetSQLCommand()
        {
            return dbCommand;
        }
    }
}