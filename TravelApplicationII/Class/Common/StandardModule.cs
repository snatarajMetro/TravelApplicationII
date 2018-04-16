using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace TravelApplication.Class.Common
{
    public class StandardModule
    {
        public void LoadDropDownList(DropDownList dd, string key, Boolean displayValue)
        {
            String sql = "SELECT * FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.AppendDataBoundItems = true;
            dd.Items.Add(new ListItem("", ""));
            dd.DataSource = dr;
            if (displayValue)
            {
                dd.DataValueField = "picklist_value";
            }
            else
            {
                dd.DataValueField = "picklist_display";
            }
            dd.DataTextField = "picklist_display";

            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public void LoadDropDownList(DropDownList dd, string key)
        {
            String sql = "SELECT DISTINCT * FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.AppendDataBoundItems = true;
            dd.Items.Add(new ListItem("", ""));
            dd.DataSource = dr;
            dd.DataTextField = "picklist_display";
            dd.DataValueField = "picklist_value";
            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public void LoadDropDownList(DropDownList dd, string key, string defaultText)
        {
            String sql = "SELECT * FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.AppendDataBoundItems = true;
            if (defaultText != null)
            {
                dd.Items.Add(new ListItem("{" + defaultText + "}", ""));
            }
            else
            {
                dd.Items.Add(new ListItem("", ""));
            }

            dd.DataSource = dr;
            dd.DataTextField = "picklist_display";
            dd.DataValueField = "picklist_value";
            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public void LoadRadioButtonList(RadioButtonList dd, string key)
        {
            String sql = "SELECT * FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.DataSource = dr;
            dd.DataTextField = "picklist_display";
            dd.DataValueField = "picklist_value";
            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public void loadCheckBoxList(CheckBoxList dd, string key)
        {
            String sql = "SELECT * FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.DataSource = dr;
            dd.DataTextField = "picklist_display";
            dd.DataValueField = "picklist_value";
            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public void loadCheckBoxList(CheckBoxList dd, string key, bool useDisplayValue)
        {
            String sql = "";
            sql = "SELECT DISTINCT picklist_display,picklist_value,picklist_order FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order";
            if (useDisplayValue) sql = "SELECT DISTINCT picklist_display,picklist_value,picklist_order FROM (SELECT DISTINCT picklist_display,picklist_display as picklist_value,picklist_order FROM GTS.PICKLIST WHERE lower(picklist_code) = lower(:picklist_code) ORDER BY picklist_order) ORDER BY picklist_order";
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add("picklist_code", OracleDbType.Varchar2).Value = key;

            OracleDataReader dr = cmd.ExecuteReader();
            dd.Items.Clear();
            dd.DataSource = dr;
            dd.DataTextField = "picklist_display";
            dd.DataValueField = "picklist_value";
            dd.DataBind();
            conn.Close();
            cmd.Dispose();
        }

        public object CheckNullNumber(object value, bool returnZero)
        {
            if (value == null)
            {
                return 0;
            }
            if (object.ReferenceEquals(value, ""))
            {
                return 0;
            }
            if (value == System.DBNull.Value)
            {
                if (returnZero)
                {
                    return 0;
                }
                else
                {
                    return null;
                }
            }
            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    return 0;
                }
            }
            return value;
        }

        public object CheckNullDate(object value)
        {
            if ((value == null) || (value == System.DBNull.Value))
            {
                return null;
            }
            return value;
        }

        public string CheckNullVarchar(object value)
        {
            if ((value == null) || (value == System.DBNull.Value))
            {
                return "";
            }
            else
            {
                return Convert.ToString(value);
            }
        }

        public bool CheckNullBoolean(object Expression)
        {
            if (Expression == System.DBNull.Value)
            {
                return false;
            }
            else
            {
                string value = (string)Expression;
                if ((value == "T"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // function to execute scalar SQL
        public string OracleExecuteScalar(string pSQL, OracleConnection conn)
        {
            string ret_val = "";
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = pSQL;
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            }

            ret_val = CheckNullVarchar(cmd.ExecuteScalar());
            if (System.DBNull.Value.Equals(ret_val))
            {
                ret_val = "";
            }
            return ret_val;
        }

        public string OracleExecuteScalar(string pSQL)
        {
            OracleConnection oConn = MakeOracleConnection();
            string ret_val = OracleExecuteScalar(pSQL, oConn);
            oConn.Close();
            oConn.Dispose();
            return ret_val;
        }

        public string OracleExecuteScalar(string pSQL, OracleParameter[] oParams)
        {
            OracleConnection conn = MakeOracleConnection();

            string ret_val = "";
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = pSQL;
            foreach (OracleParameter p in oParams)
            {
                cmd.Parameters.Add(p);
            }

            cmd.Connection.Open();
            ret_val = CheckNullVarchar(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            conn.Dispose();

            if (System.DBNull.Value.Equals(ret_val))
            {
                ret_val = "";
            }


            return ret_val;
        }

        // number version of execute scalar
        public object OracleExecuteScalarObject(string pSQL, OracleConnection conn)
        {
            object ret_val = 0;
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = pSQL;
            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
            }

            ret_val = CheckNullNumber(cmd.ExecuteScalar(), true);
            if (System.DBNull.Value.Equals(ret_val))
            {
                ret_val = 0;
            }
            return ret_val;
        }

        public object OracleExecuteScalarObject(string pSQL)
        {
            OracleConnection oConn = MakeOracleConnection();
            object ret_val = OracleExecuteScalarObject(pSQL, oConn);
            oConn.Close();
            oConn.Dispose();
            return ret_val;
        }

        // function to execute other SQL
        public int OracleExecuteSQL(string pSQL, OracleConnection conn)
        {
            int ret_val = -1;
            Boolean connectionIsClosed = (conn.State != ConnectionState.Open);
            OracleCommand cmd = new OracleCommand();
            if (!string.IsNullOrEmpty(pSQL))
            {
                try
                {
                    if (connectionIsClosed)
                    {
                        conn.Open();
                    }
                    cmd.Connection = conn;
                    cmd.CommandText = pSQL;
                    ret_val = cmd.ExecuteNonQuery();
                    if (connectionIsClosed)
                    {
                        conn.Close();
                    }
                }
                catch (Exception E)
                {
                    string msg = E.Message;
                    ret_val = -1;
                }
            }
            else { return -1; }
            return ret_val;
        }

        public int OracleExecuteSQL(string pSQL)
        {
            OracleConnection oConn = MakeOracleConnection();
            oConn.Open();
            int ret_val = OracleExecuteSQL(pSQL, oConn);
            oConn.Close();
            oConn.Dispose();
            return ret_val;
        }

        // DB Read function
        public OracleDataReader OracleDataReader(string sqlStatement, OracleConnection conn)
        {
            if (conn == null)
            {
                conn = MakeOracleConnection();
            }
            OracleCommand cmd = new OracleCommand(sqlStatement, conn);
            if (!(conn.State == ConnectionState.Open))
            {
                conn.Open();
            }
            return cmd.ExecuteReader();
        }

        public OracleDataReader OracleDataReader(string sqlStatement, OracleParameter[] oPs, OracleConnection conn)
        {
            if (conn == null)
            {
                conn = MakeOracleConnection();
            }
            OracleCommand cmd = new OracleCommand(sqlStatement, conn);
            foreach (OracleParameter op in oPs)
            {
                cmd.Parameters.Add(op);
            }

            if (!(conn.State == ConnectionState.Open))
            {
                conn.Open();
            }
            return cmd.ExecuteReader();
        }

        // DB Fill function
        public void OracleFillDataSet(ref DataSet ds, string sql, string tableName, OracleConnection conn)
        {
            bool CloseConn = false;
            if (conn == null)
            {
                conn = MakeOracleConnection();
                CloseConn = true;
                conn.Open();
            }
            OracleDataAdapter da = new OracleDataAdapter(sql, conn);
            da.Fill(ds, tableName);
            if (CloseConn)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public void OracleFillDataSet(ref DataSet ds, string sql, string tableName)
        {
            OracleConnection conn = MakeOracleConnection();
            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(sql, conn);
            da.Fill(ds, tableName);
            conn.Close();
            conn.Dispose();
        }

        private OracleConnection MakeOracleConnection(string connString) // oracle connection function
        {
            string conn = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
            OracleConnection oOracleConn = new OracleConnection();
            oOracleConn.ConnectionString = conn;
            return oOracleConn;
        }

        private OracleConnection MakeOracleConnection()
        {
            return MakeOracleConnection("DefaultDBContext");
        }
    }
}