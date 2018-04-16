using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WebAppTest.Class.SQLBuilder
{
    /// <summary>
    /// SelectQueryBuilder class
    /// To build Update SQL query
    /// </summary>
    public class SelectQueryBuilder
    {
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public string PrimaryKey { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }

        private readonly List<string> selectColumns = new List<string>();
        private string whereClause = "";
        private IList<string> orderByList = new List<string>();

        /// <summary>
        /// To add main table selected columns for display
        /// </summary>
        /// <param name="columnName">column name</param>
        /// <returns>this SelectQueryBuilder instance</returns>
        public SelectQueryBuilder AddSelectColumn(string columnName)
        {
            selectColumns.Add(columnName);
            return this;
        }

        /// <summary>
        /// Builds where clause from JObject instance
        /// </summary>
        /// <param name="jObjectFilter">JObject instance that contains where clause filter information</param>
        /// <returns>current SelectQueryBuilder instance</returns>
        public SelectQueryBuilder SetWhereClause(JObject jObjectFilter)
        {

            if (jObjectFilter != null)
            {
                // set the filter information
                string tmpWhere = "";

                foreach (var result in jObjectFilter)
                {
                    if (string.IsNullOrEmpty(whereClause))
                    {
                        whereClause += "WHERE ";
                    }
                    else
                    {
                        whereClause += " AND ";
                    }
                    if (result.Value.Count() == 0)
                    {
                        if (result.Value.ToString().StartsWith("~~"))
                        {
                            whereClause += string.Format("({0}.{1} IN ('{2}'))", TableAlias, result.Key, result.Value.ToString().Replace("~~", "").Replace(",", "','"));
                        }
                        else
                        {
                            whereClause += string.Format("({0}.{1} LIKE '%{2}%')", TableAlias, result.Key, result.Value);
                            //whereClause += string.Format("(LOWER({0}.{1}) LIKE '%{2}%')", TableAlias, result.Key, result.Value.ToString().ToLower()); // case-insensitive with LOWER() function
                        }
                    }
                    else
                    {
                        switch (result.Value["TYPE"].ToString())
                        {
                            case "date":
                                if (!string.IsNullOrEmpty(result.Value["FROM"].ToString()) && !string.IsNullOrEmpty(result.Value["TO"].ToString()))
                                {
                                    if (result.Value["FROM"].ToObject<DateTime>() > result.Value["TO"].ToObject<DateTime>())
                                    {
                                        JToken tmp = result.Value["FROM"];
                                        result.Value["FROM"] = result.Value["TO"];
                                        result.Value["TO"] = tmp;
                                    }
                                }

                                tmpWhere = "(";
                                if (!string.IsNullOrEmpty(result.Value["FROM"].ToString()))
                                {
                                    tmpWhere += string.Format("(TRUNC({0}.{1}) >= TO_DATE('{2}','mm/dd/yyyy'))", TableAlias, result.Key, result.Value["FROM"]);
                                }
                                if (!string.IsNullOrEmpty(result.Value["TO"].ToString()))
                                {
                                    if (tmpWhere != "(") tmpWhere += " AND ";
                                    tmpWhere += string.Format("(TRUNC({0}.{1}) <= TO_DATE('{2}','mm/dd/yyyy'))", TableAlias, result.Key, result.Value["TO"]);
                                }
                                tmpWhere += ")";
                                whereClause += tmpWhere;
                                break;
                            case "number":
                                if (!string.IsNullOrEmpty(result.Value["FROM"].ToString()) && !string.IsNullOrEmpty(result.Value["TO"].ToString()))
                                {
                                    if (result.Value["FROM"].ToObject<double>() > result.Value["TO"].ToObject<double>())
                                    {
                                        JToken tmp = result.Value["FROM"];
                                        result.Value["FROM"] = result.Value["TO"];
                                        result.Value["TO"] = tmp;
                                    }
                                }

                                tmpWhere = "(";
                                if (!string.IsNullOrEmpty(result.Value["FROM"].ToString()))
                                {
                                    tmpWhere += string.Format("({0}.{1} >= {2})", TableAlias, result.Key, result.Value["FROM"]);
                                }
                                if (!string.IsNullOrEmpty(result.Value["TO"].ToString()))
                                {
                                    if (tmpWhere != "(") tmpWhere += " AND ";
                                    tmpWhere += string.Format("({0}.{1} <= {2})", TableAlias, result.Key, result.Value["TO"]);
                                }
                                tmpWhere += ")";
                                whereClause += tmpWhere;
                                break;
                            default:
                                tmpWhere = "(";
                                if (!string.IsNullOrEmpty(result.Value["FROM"].ToString()))
                                {
                                    tmpWhere += string.Format("({0}.{1} >= {2})", TableAlias, result.Key, result.Value["FROM"]);
                                }
                                if (!string.IsNullOrEmpty(result.Value["TO"].ToString()))
                                {
                                    if (tmpWhere == "(") tmpWhere += " AND ";
                                    tmpWhere += string.Format("({0}.{1} <= {2})", TableAlias, result.Key, result.Value["TO"]);
                                }
                                tmpWhere += ")";
                                whereClause += tmpWhere;
                                break;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Add an Order By keyword to the query
        /// </summary>
        /// <param name="sortField">Field name</param>
        /// <param name="sortDirection">Sort direction to use ("ASC"[default] / "DESC")</param>
        public void SetOrderByClause(string sortField, string sortDirection = "ASC")
        {
            if (!string.IsNullOrEmpty(sortField))
            {
                orderByList.Add(sortField.ToUpper() + " " + sortDirection.ToUpper());
            }
        }

        /// <summary>
        /// SELECT query string for finding the number of total selected rows
        /// </summary>
        /// <returns>SELECT query string</returns>
        public string GetAllCoutQuery()
        {
            return string.Format("SELECT count(1) FROM {0} {1} {2}", TableName, TableAlias, whereClause);
        }

        /// <summary>
        /// Returns final completed SELECT query string
        /// </summary>
        /// <returns>SELECT query string</returns>
        public string GetSelectQuery()
        {
            // To build select columns string
            var selectColumnsBuilder = new StringBuilder();
            selectColumnsBuilder.Append(TableAlias + "." + PrimaryKey);
            foreach (var column in selectColumns)
            {
                selectColumnsBuilder.Append(", " + TableAlias + "." + column);
            }

            // To set order by clause
            var orderByBuilder = new StringBuilder();
            for (int i = 0; i < orderByList.Count; i++)
            {
                var orderByString = orderByList[i];

                if (i == 0)
                {
                    orderByBuilder.Append("ORDER BY " + orderByString);
                }
                orderByBuilder.Append(", " + orderByString);
            }

            // This is for Oracle 12 version and above
            return string.Format("SELECT {0} FROM {1} {2} {3} {4} {5} {6}",
                                selectColumnsBuilder.ToString(),
                                TableName,
                                TableAlias,
                                whereClause,
                                orderByBuilder.ToString(),
                                string.IsNullOrEmpty(Offset.ToString()) ? "" : "OFFSET " + Offset + " ROWS",
                                string.IsNullOrEmpty(Limit.ToString()) ? "" : "FETCH NEXT " + Limit + " ROWS ONLY");

            /*
            // This query format supports Oracle 12 as well as old version, but not using anymore.
            // You can still using this block if your application is connecting to old Oracle database
            return string.Format("SELECT T1.* "
                                + "FROM (SELECT T2.*, ROWNUM AS RNUM "
                                + "      FROM (SELECT {0} FROM {1} {2} {3} {4}) T2 "
                                + "      WHERE ROWNUM <= {5}) T1 "
                                + "WHERE RNUM > {6} ",
                                selectColumnsBuilder.ToString(),
                                TableName,
                                TableAlias,
                                whereClause,
                                orderByBuilder.ToString(),
                                Convert.ToString(Convert.ToInt32(Offset) + Convert.ToInt32(Limit)),
                                Convert.ToString(Convert.ToInt32(Offset)));
            */
        }
    }
}