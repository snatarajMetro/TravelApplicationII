using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.Class.Common;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public class FISRepository : IFISRepository
    {
        private DbConnection dbConn;
        public async Task<List<CostCenter>> GetCostCenters()
        {
            List<CostCenter> costCenters = new List<CostCenter>();
            var client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["fisServiceUrl"].ToString()+ string.Format("/CostCenters");
                HttpResponseMessage response = await client.GetAsync(endpointUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {

                    costCenters = await response.Content.ReadAsAsync<List<CostCenter>>();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("FIS repository : GetCostCenters : " + ex.Message);
                throw new Exception("Unable to get the cost center information from FIS ");
            }
            finally
            {
                client.Dispose();
            }
            return costCenters;
        }

        public async Task<List<Project>> GetProjectsByCostCenterName(string costCenterName)
        {
            List<Project> projects = new List<Project>();
            List<string> projectIds = null;

            var client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["fisServiceUrl"].ToString() + string.Format("/ProjectsByCostCenterId/{0}",costCenterName);
                HttpResponseMessage response = await client.GetAsync(endpointUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {

                    projectIds = await response.Content.ReadAsAsync<List<string>>();

                    foreach (var item in projectIds)
                    {
                        projects.Add(new Project() { Id = item, Name = item });
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("FIS repository : GetProjectsByCostCenterName : " + ex.Message);
                throw new Exception("Unable to get the project  information from FIS service");
            }
            finally
            {
                client.Dispose();
            }
            return projects;
        }

        public FIS GetFISdetails(DbConnection dbConn, string travelRequestId)
        {
            try
            {

            FIS response = new FIS();
            List<FISDetails> fisDetails = new List<FISDetails>();
            string query = string.Format(@"Select * from travelRequest_Fis where travelRequestid = {0}", travelRequestId);
            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    fisDetails.Add(new FISDetails()
                    {
                        CostCenterId = dataReader["COSTCENTERID"].ToString(),
                        LineItem = dataReader["LINEITEM"].ToString(),
                        ProjectId = dataReader["PROJECTID"].ToString(),
                        Task = dataReader["TASK"].ToString(),
                        Amount = Convert.ToInt32(dataReader["AMOUNT"]),
                        TravelRequestId = travelRequestId                         
                    });
                    response.TotalAmount = Convert.ToInt32(dataReader["TOTALAMOUNT"]);
                }
                response.FISDetails = fisDetails;
            }
            return response;

            }
            catch (Exception ex )
            {

                LogMessage.Log("FIS repository : GetFISdetails : " + ex.Message);
                throw new Exception("Unable to get the FIS details" + ex.Message);
            }
        }

        public FIS GetFISdetailsForReimburse(DbConnection dbConn, string travelRequestId)
        {
            try
            {

                FIS response = new FIS();
                List<FISDetails> fisDetails = new List<FISDetails>();
                string query = string.Format(@"Select * from Reimburse_Fis where travelRequestid = {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        fisDetails.Add(new FISDetails()
                        {
                            CostCenterId = dataReader["COSTCENTERID"].ToString(),
                            LineItem = dataReader["LINEITEM"].ToString(),
                            ProjectId = dataReader["PROJECTID"].ToString(),
                            Task = dataReader["TASK"].ToString(),
                            Amount = Convert.ToInt32(dataReader["AMOUNT"]),
                            TravelRequestId = travelRequestId
                        });
                        response.TotalAmount = Convert.ToInt32(dataReader["TOTALAMOUNT"]);
                    }
                    response.FISDetails = fisDetails;
                }
                return response;

            }
            catch (Exception ex)
            {

                LogMessage.Log("FIS repository : GetFISdetailsForReimburse : " + ex.Message);
                throw new Exception("Unable to get the FIS details" + ex.Message);
            }
        }
    }
}