using System;
using System.Data;
using System.Data.Common;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace TravelApplication.Class.SQLBuilder
{
    /// <summary>
    /// UpdateQueryBuilder class
    /// To build Update SQL query
    /// </summary>
    public class UpdateQueryBuilder
    {
        private string updateQuery;
        private StringBuilder setNameValueList;
        private string whereCaluse;
        private DbCommand dbCommand;

        /// <summary>
        /// Constructor with database table name, primary key and value
        /// </summary>
        /// <param name="tableName">database table name</param>
        /// <param name="primaryKey">primary key for where clause</param>
        /// <param name="primaryKeyValue">primary key value for where clause</param>
        public UpdateQueryBuilder(string tableName, string primaryKey, string primaryKeyValue)
        {
            updateQuery = "UPDATE " + tableName + " SET ";
            setNameValueList = new StringBuilder();
            whereCaluse = "WHERE " + primaryKey + " = " + primaryKeyValue;
            dbCommand = new OracleCommand();
        }

        /// <summary>
        /// This method will add a single column name in string and value object to UPDATE query
        /// </summary>
        /// <param name="columnName">column name string</param>
        /// <param name="columnValue">column value object</param>
        public void AddSetNameAndValue(string columnName, object columnValue)
        {
            if (!string.IsNullOrEmpty(setNameValueList.ToString()))
            {
                setNameValueList.Append(",");
            }

            setNameValueList.Append(columnName + " = :" + columnName);
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
        /// Returns final completed UPDATE query string
        /// </summary>
        /// <returns>return UPDATE query string</returns>
        public string GetCompletedSQL()
        {
            return updateQuery + setNameValueList.ToString() + " " + whereCaluse;
        }

        /// <summary>
        /// Returns OracleCommand instance that is used for this UPDATE query
        /// </summary>
        /// <returns>returns OracleCommand instance</returns>
        public DbCommand GetSQLCommand()
        {
            return dbCommand;
        }
    }
}