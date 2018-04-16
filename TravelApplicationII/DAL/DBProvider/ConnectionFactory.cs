using System.Configuration;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace TravelApplication.DAL.DBProvider
{
    /// <summary>
    /// ConnectionFactory class
    /// Returns default database connection instance which is OracleConnection.
    /// </summary>
    public class ConnectionFactory
    {
        private static string defaultConnectionString = ConfigurationManager.ConnectionStrings["DefaultDBContext"].ConnectionString;
        //private static string secondConnectionString = ConfigurationManager.ConnectionStrings["SecondDBContext"].ConnectionString;

        /// <summary>
        /// Returns an open connection for default oracle database
        /// </summary>
        /// <returns>IDbConnection type instance for database connection</returns>
        public static DbConnection GetOpenDefaultConnection()
        {
            var connection = new OracleConnection(defaultConnectionString);
            connection.Open();

            // Set session state to allow case insensitive searches
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "alter session set NLS_COMP=LINGUISTIC";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "alter session set NLS_SORT=BINARY_CI";
            cmd.ExecuteNonQuery();

            return connection;
        }

        /// <summary>
        /// Returns an open connection for another oracle database
        /// </summary>
        /// <returns></returns>
        //public static IDbConnection GetOpenSecondConnection()
        //{
        //    var connection = new OracleConnection(secondConnectionString);
        //    connection.Open();

        //    return connection;
        //}
    }
}