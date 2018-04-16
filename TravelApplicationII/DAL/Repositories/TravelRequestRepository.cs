using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TravelApplication.DAL.DBProvider;
using TravelApplication.Models;
using System.Collections.Generic;
using TravelApplication.Common;
using TravelApplication.DAL.Repositories;
using TravelApplication.Class.Common;

namespace TravelApplication.Services
{
    public class TravelRequestRepository :  ApprovalRepository, ITravelRequestRepository
    {
        private DbConnection dbConn;
        IEstimatedExpenseRepository estimatedExpenseRepository = new EstimatedExpenseRepository();
        IFISRepository fisRepository = new FISRepository();
        TravelRequestReportService travelRequestReportService = new TravelRequestReportService();

        public async Task<EmployeeDetails> GetEmployeeDetails(int badgeNumber)
        {
            EmployeeDetails employeeDetails = null;
            var client = new HttpClient();
            try

            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["fisServiceUrl"].ToString()+ string.Format("/EmployeeInfo/{0}", badgeNumber);

                HttpResponseMessage response = await client.GetAsync(endpointUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {

                    employeeDetails = await response.Content.ReadAsAsync<EmployeeDetails>();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("Could not get badge information from FIS : " + ex.Message);
                throw new Exception("Unable to get the badge information from FIS service");
            }
            finally
            {
                client.Dispose();
            }
            return employeeDetails;

        }
        public async Task<string> GetVendorNumber(int badgeNumber)
        {
             
            var client = new HttpClient();
            var result = string.Empty;
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var endpointUrl = System.Configuration.ConfigurationManager.AppSettings["fisServiceUrl"].ToString() + string.Format("/VendorId/{0}", badgeNumber);

                HttpResponseMessage response = await client.GetAsync(endpointUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {

                    result = await response.Content.ReadAsAsync<string>();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("Could not get Vendor  information from FIS : " + ex.Message);
                throw new Exception("Unable to get the Vendor information from FIS service");
            }
            finally
            {
                client.Dispose();
            }
            return result;

        }

        public async Task<int> SaveTravelRequest(TravelRequest request)
        {
            int travelRequestId = 0;
            try
            {
                ValidateTravelRequestInfo(request);              
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    var loginId = getUserName(dbConn,request.UserId);
                    request.LoginId = loginId;
                    if (request.TravelRequestId == 0)
                    {                           
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = @"INSERT INTO TRAVELREQUEST (                                                  
                                                    BADGENUMBER,
                                                    NAME,
                                                    DIVISION,
                                                    SECTION,
                                                    ORGANIZATION,
                                                    MEETINGLOCATION,
                                                    MEETINGBEGINDATETIME,
                                                    DEPARTUREDATETIME,
                                                    MEETINGENDDATETIME,
                                                    RETURNDATETIME,
                                                    CREATIONDATETIME,
                                                    SELECTEDROLEID,
                                                    STATUS,
                                                    SUBMITTEDBYBADGENUMBER,
                                                    PURPOSE
                                                )
                                                VALUES
                                                    (:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13,:p14,:P15,:p16) returning TRAVELREQUESTID into :travelRequestId";
                        cmd.Parameters.Add(new OracleParameter("p2", request.BadgeNumber));
                        cmd.Parameters.Add(new OracleParameter("p3", request.Name));
                        cmd.Parameters.Add(new OracleParameter("p4", request.Division));
                        cmd.Parameters.Add(new OracleParameter("p5", request.Section));
                        cmd.Parameters.Add(new OracleParameter("p6", request.Organization));
                        cmd.Parameters.Add(new OracleParameter("p7", request.MeetingLocation));
                        cmd.Parameters.Add(new OracleParameter("p8", request.MeetingBeginDateTime));
                        cmd.Parameters.Add(new OracleParameter("p9", request.DepartureDateTime));
                        cmd.Parameters.Add(new OracleParameter("p10", request.MeetingEndDateTime));
                        cmd.Parameters.Add(new OracleParameter("p11", request.ReturnDateTime));
                        cmd.Parameters.Add(new OracleParameter("p12", DateTime.Now));
                        cmd.Parameters.Add(new OracleParameter("p13", request.SelectedRoleId));
                        cmd.Parameters.Add(new OracleParameter("p14", ApprovalStatus.New.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p15", request.SubmittedByBadgeNumber));
                        cmd.Parameters.Add(new OracleParameter("p16", request.SubmittedByBadgeNumber));
                        cmd.Parameters.Add("travelRequestId", OracleDbType.Int32, ParameterDirection.ReturnValue);
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        travelRequestId = Decimal.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)(cmd.Parameters["travelRequestId"].Value)).Value);
                        cmd.Dispose();
                    }                 
                else        
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = (OracleConnection)dbConn;
                    cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST SET                                                  
                                                BADGENUMBER = :p2,
                                                NAME = :p3,
                                                DIVISION  = :p4,
                                                SECTION  = :p5,
                                                ORGANIZATION  = :p6,
                                                MEETINGLOCATION  = :p7,
                                                MEETINGBEGINDATETIME  = :p8,
                                                DEPARTUREDATETIME  = :p9,
                                                MEETINGENDDATETIME  = :p10,
                                                RETURNDATETIME  = :p11,
                                                LASTUPDATEDDATETIME  = :p12,
                                                PURPOSE = :p13
                                                WHERE TRAVELREQUESTID = {0}",request.TravelRequestId);
                    cmd.Parameters.Add(new OracleParameter("p2", request.BadgeNumber));
                    cmd.Parameters.Add(new OracleParameter("p3", request.Name));
                    cmd.Parameters.Add(new OracleParameter("p4", request.Division));
                    cmd.Parameters.Add(new OracleParameter("p5", request.Section));
                    cmd.Parameters.Add(new OracleParameter("p6", request.Organization));
                    cmd.Parameters.Add(new OracleParameter("p7", request.MeetingLocation));
                    cmd.Parameters.Add(new OracleParameter("p8", request.MeetingBeginDateTime));
                    cmd.Parameters.Add(new OracleParameter("p9", request.DepartureDateTime));
                    cmd.Parameters.Add(new OracleParameter("p10", request.MeetingEndDateTime));
                    cmd.Parameters.Add(new OracleParameter("p11", request.ReturnDateTime));
                    cmd.Parameters.Add(new OracleParameter("p12", DateTime.Now));
                    cmd.Parameters.Add(new OracleParameter("p13", request.Purpose));
                    var rowsUpdated = cmd.ExecuteNonQuery();
                    travelRequestId = request.TravelRequestId;
                    cmd.Dispose();
                    
                }
                    dbConn.Close();
                    dbConn.Dispose();
                }
            }
            catch (Exception ex)
            {

                LogMessage.Log("Couldn't insert/update record into Travel Request - " + ex.Message);
                 throw new Exception("Couldn't insert/update record into Travel Request - " );
            }

            return travelRequestId;
        }

        public string getUserName(DbConnection dbConn, int id)
        {
               string query = string.Format("Select loginId from users where Id = {0}", id);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                string userName = string.Empty;
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        userName = dataReader["LoginId"].ToString();
                    }
                }
                command.Dispose();
                dataReader.Close();
                return userName;
    
        }

        public void ValidateTravelRequestInfo(TravelRequest request)
        {
            try
            {
                if (request.BadgeNumber <= 0)
                {
                    throw new Exception("Invalid Badge Number");
                }
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new Exception("Invalid Name");
                }

                if (string.IsNullOrWhiteSpace(request.Division))
                {
                    throw new Exception("Invalid Division");
                }
                if (string.IsNullOrWhiteSpace(request.Section))
                {
                    throw new Exception("Invalid Section");
                }
                if (string.IsNullOrWhiteSpace(request.Organization))
                {
                    throw new Exception("Invalid Organization");
                }
                if (string.IsNullOrWhiteSpace(request.MeetingLocation))
                {
                    throw new Exception("Invalid Meeting Location");
                }

                if (request.MeetingBeginDateTime == DateTime.MinValue)
                {
                    throw new Exception("Invalid Meeting Begin Date");
                }
                if (request.MeetingEndDateTime == DateTime.MinValue)
                {
                    throw new Exception("Invalid Meeting End Date");
                }
                //if (request.DepartureDateTime == DateTime.MinValue)
                //{
                //    throw new Exception("Invalid Departure Date");
                //}
                //if (request.ReturnDateTime == DateTime.MinValue)
                //{
                //    throw new Exception("Invalid Return Date");
                //}
            }
            catch (Exception ex)
            {
                LogMessage.Log("Validate travel request info "+ex.Message);

                throw new Exception(ex.Message);
            }
        }

        public TravelRequest GetTravelRequestDetail(int travelRequestId)
        {
            try
            {
            TravelRequest response = null;
            using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
            {
                string query = string.Format("Select * from TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        response = new TravelRequest()
                        {
                            BadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]),
                            Name = dataReader["NAME"].ToString(),
                            Division = dataReader["DIVISION"].ToString(),
                            Section = dataReader["SECTION"].ToString(),
                            Organization = dataReader["ORGANIZATION"].ToString(),
                            MeetingLocation = dataReader["MEETINGLOCATION"].ToString(),
                            MeetingBeginDateTime = Convert.ToDateTime(dataReader["MEETINGBEGINDATETIME"]),
                            MeetingEndDateTime = Convert.ToDateTime(dataReader["MEETINGENDDATETIME"]),
                            DepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]),
                            ReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"]),
                            Purpose = dataReader["Purpose"].ToString()
                        };
                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }
                command.Dispose();
                dataReader.Close();
                dbConn.Close();
                dbConn.Dispose();
            }
            return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
                throw;
            }
        }

        public TravelRequest GetTravelRequestDetail(DbConnection dbConn, string travelRequestId)
        {
            try
            {

            TravelRequest response = null;
                string query = string.Format("Select * from TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        response = new TravelRequest()
                        {
                            BadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]),
                            Name = dataReader["NAME"].ToString(),
                            Division = dataReader["DIVISION"].ToString(),
                            Section = dataReader["SECTION"].ToString(),
                            Organization = dataReader["ORGANIZATION"].ToString(),
                            MeetingLocation = dataReader["MEETINGLOCATION"].ToString(),
                            MeetingBeginDateTime = Convert.ToDateTime(dataReader["MEETINGBEGINDATETIME"]),
                            MeetingEndDateTime = Convert.ToDateTime(dataReader["MEETINGENDDATETIME"]),
                            DepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]),
                            ReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"]),
                            Purpose = dataReader["PURPOSE"].ToString(),
                            StrMeetingBeginDateTime = Convert.ToDateTime(dataReader["MEETINGBEGINDATETIME"]).ToShortDateString(),
                            StrMeetingEndDateTime = Convert.ToDateTime(dataReader["MEETINGENDDATETIME"]).ToShortDateString(),
                            StrDepartureDateTime = (Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]).ToShortDateString()== "1/1/0001") ? string.Empty : Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]).ToShortDateString(),
                            StrReturnDateTime = (Convert.ToDateTime(dataReader["RETURNDATETIME"]).ToShortDateString() == "1/1/0001") ? string.Empty : Convert.ToDateTime(dataReader["RETURNDATETIME"]).ToShortDateString(),
                        };
                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }
                command.Dispose();
                dataReader.Close();
                return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
                throw;
            }
        }
        public List<TravelRequestDetails> GetTravelRequestList(int submittedBadgeNumber, int selectedRoleId)
        {
            try
            {
                List<TravelRequestDetails> response = new List<TravelRequestDetails>();

                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    if (selectedRoleId == 1 || selectedRoleId == 2)
                    {
               
                        string query = string.Format("Select * from TRAVELREQUEST where BADGENUMBER= {0} order by CREATIONDATETIME desc", submittedBadgeNumber, selectedRoleId);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new TravelRequestDetails()
                                {
                                    TravelRequestId = Convert.ToInt32(dataReader["TravelRequestId"]),
                                    //Description = dataReader["PURPOSE"].ToString(),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = EditActionEligible(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["PURPOSE"].ToString(),
                                    CancelActionVisible = CancelActionEligible(dbConn, dataReader["TravelRequestId"].ToString()) ? true : false,
                                    BadgeNumber = dataReader["BADGENUMBER"].ToString()

                                });
                            }
                        }
                        command.Dispose();
                        dataReader.Close();                  
                    }  
                    if (selectedRoleId == 3)                    
                    {   
                        string query = string.Format(@"SELECT
	                                                        *
                                                        FROM
	                                                        TRAVELREQUEST
                                                        WHERE
	                                                        TRAVELREQUESTID IN (
		                                                        SELECT
			                                                        TRAVELREQUESTId
		                                                        FROM
			                                                        TRAVELREQUEST_APPROVAL
		                                                        WHERE
			                                                        BADGENUMBER = {0} OR APPROVEROTHERBADGENUMBER ={0}
	                                                        )   order by CREATIONDATETIME desc", submittedBadgeNumber);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new TravelRequestDetails()
                                {
                                    TravelRequestId = Convert.ToInt32(dataReader["TravelRequestId"]),
                                    Description = dataReader["Purpose"].ToString(),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn,Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = getApprovalSatus(dbConn,Convert.ToInt32(dataReader["TravelRequestId"]), submittedBadgeNumber) ? true : false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["Purpose"].ToString(),
                                    CancelActionVisible = false,
                                    ShowApproverAlert = (Convert.ToDateTime(dataReader["DEPARTUREDATETIME"])- DateTime.Today).TotalDays <= 30  ? true : false,
                                    BadgeNumber = dataReader["BADGENUMBER"].ToString()

                                });
                            }
                        }
                        command.Dispose();
                        dataReader.Close();                   
                    }

                    if (selectedRoleId == 4)
                    {
                        string query = string.Format("Select * from TRAVELREQUEST order by CREATIONDATETIME desc", submittedBadgeNumber, selectedRoleId);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new TravelRequestDetails()
                                {
                                    TravelRequestId = Convert.ToInt32(dataReader["TravelRequestId"]),
                                    //Description = dataReader["PURPOSE"].ToString(),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = EditActionEligibleForTravelAdimin(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = getApprovalSatus(dbConn, Convert.ToInt32(dataReader["TravelRequestId"]), submittedBadgeNumber) ? true : false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["PURPOSE"].ToString(),
                                    CancelActionVisible = false,
                                    ShowApproverAlert = (Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]) - DateTime.Today).TotalDays <= 30 ? true : false,
                                    BadgeNumber = dataReader["BADGENUMBER"].ToString()
                                });
                            }
                        }
                        command.Dispose();
                        dataReader.Close();
                    }
                    dbConn.Close();
                    dbConn.Dispose();
                    return response;
                }

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestList : " + ex.Message);
                throw;
            }
        }

        private bool CancelActionEligible(DbConnection dbConn, string travelRequestId)
        {
            string response = "";
            string query = string.Format(@"SELECT
	                                        STATUS
                                        FROM
	                                        TRAVELREQUEST 
                                        WHERE
	                                        TRAVELREQUESTID = {0}  ", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = dataReader["STATUS"].ToString();
                }
            }
            command.Dispose();
            dataReader.Close();

            if (response != ApprovalStatus.Cancelled.ToString())
            {
                return true;
            }
            return false;
        }

        public bool Approve(int approverBadgeNumber, string travelRequestId, string comments)
        {
            try
            {
                bool response = false;
                string approvalOrderResult = string.Empty;
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    // check if its already approved by website or Email 
                    string query2 = string.Format("Select * from  TRAVELREQUEST_APPROVAL WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1} OR APPROVEROTHERBADGENUMBER ={1} ", travelRequestId, approverBadgeNumber);
                    OracleCommand command2 = new OracleCommand(query2, (OracleConnection)dbConn);
                    command2.CommandText = query2;
                    DbDataReader dataReader2 = command2.ExecuteReader();

                    if (dataReader2.HasRows)
                    {                                            

                        //Update travel request _approval
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST_APPROVAL SET                                                  
                                                            APPROVERCOMMENTS = :p1,
                                                            APPROVALSTATUS = :p2 ,
                                                            APPROVALDATETIME = :p3
                                                            WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1} OR APPROVEROTHERBADGENUMBER ={1}  ", travelRequestId, approverBadgeNumber);
                        cmd.Parameters.Add(new OracleParameter("p1", comments));
                        cmd.Parameters.Add(new OracleParameter("p2", ApprovalStatus.Approved.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p3", DateTime.Now));             
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        // Get the approval badgeNumber 
                        var result = getNextApproverBadgeNumber(dbConn, travelRequestId);
                    
                        // Update the Status in Travel request
                        OracleCommand cmd2 = new OracleCommand();
                        cmd2.Connection = (OracleConnection)dbConn;
                        string query = string.Format(@"UPDATE  TRAVELREQUEST SET                                                  
                                                             STATUS = :p1,
                                                             LASTUPDATEDDATETIME = :p2,
                                                             LASTUPDATEDBYLOGINID = :p3 
                                                            WHERE TRAVELREQUESTID = {0}", travelRequestId);                    
                        cmd2.CommandText = query;
                        if (result != 0)
                            {
                             
                                cmd2.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Pending.ToString()));
                                cmd2.Parameters.Add(new OracleParameter("p2", DateTime.Now));
                                cmd2.Parameters.Add(new OracleParameter("p3", approverBadgeNumber));
                            var rowsUpdated1 = cmd2.ExecuteNonQuery();

                            var dateTime = System.DateTime.Now.Ticks;

                            travelRequestReportService.RunReport("Travel_Request.rpt", "TravelRequest_" + travelRequestId+"_"+dateTime, travelRequestId);
                            //Send Email for next approver
                            string subject = string.Format(@"Travel Request Approval for Id - {0} ", travelRequestId);                            
                            sendEmail(result, subject,travelRequestId,"Form1", "TravelRequest_"+travelRequestId+"_"+dateTime);
                           
                            }                    
                        else
                        {
                            cmd2.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Complete.ToString()));
                            cmd2.Parameters.Add(new OracleParameter("p2", DateTime.Now));
                            cmd2.Parameters.Add(new OracleParameter("p3", approverBadgeNumber));
                            var rowsUpdated1 = cmd2.ExecuteNonQuery();
                        }
              
                        cmd2.Dispose();
                        dbConn.Close();
                        dbConn.Dispose();
                        response = true;
                    }
                    else
                    {
                        response = false;
                    }
                    
                    command2.Dispose();
                }                
                return response;
            }
            catch (Exception ex)
            {
                LogMessage.Log("Approve :" + ex.Message);
                throw;
            }
        }

        public bool Reject(ApproveRequest approveRequest)
        {
            try
            {
                bool response = false;
                int approvalOrder = 0;
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    // check if its already approved by website or Email 
                    string query2 = string.Format("Select * from  TRAVELREQUEST_APPROVAL WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1}  ", approveRequest.TravelRequestId, approveRequest.ApproverBadgeNumber);
                    OracleCommand command2 = new OracleCommand(query2, (OracleConnection)dbConn);
                    command2.CommandText = query2;
                    DbDataReader dataReader2 = command2.ExecuteReader();

                    if (dataReader2.HasRows)
                    {

                        //Update travel request _approval
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST_APPROVAL SET                                                  
                                                            APPROVERCOMMENTS = :p1,
                                                            APPROVALSTATUS = :p2 ,
                                                            REJECTEDDATETIME = :p3,
                                                            REJECTREASON = :p4
                                                            WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1}   ", approveRequest.TravelRequestId, approveRequest.ApproverBadgeNumber);
                        cmd.Parameters.Add(new OracleParameter("p1", approveRequest.Comments));
                        cmd.Parameters.Add(new OracleParameter("p2", ApprovalStatus.Rejected.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p3", DateTime.Now));
                        cmd.Parameters.Add(new OracleParameter("p4", approveRequest.RejectReason));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    
                        var rejectTravelRequest = string.Empty;
                        if (approveRequest.ApproverBadgeNumber == 85163)
                        {
                            rejectTravelRequest = "true";

                            //OracleCommand cmd3 = new OracleCommand();
                            //cmd3.Connection = (OracleConnection)dbConn;
                            //cmd3.CommandText = string.Format(@"Delete from TRAVELREQUEST_APPROVAL where TravelRequestId = {0}", approveRequest.TravelRequestId);
                            //cmd3.ExecuteNonQuery();

                            if ((!string.IsNullOrEmpty(approveRequest.DepartmentHeadBadgeNumber)) && approveRequest.DepartmentHeadBadgeNumber != "?")
                            {
                                InsertApprovalList(approveRequest.TravelRequestId, approveRequest.DepartmentHeadBadgeNumber, approveRequest.DepartmentHeadName, 1);
                            }

                            if ((!string.IsNullOrEmpty(approveRequest.ExecutiveOfficerBadgeNumber)) && approveRequest.ExecutiveOfficerBadgeNumber != "?")
                            {
                                InsertApprovalList(approveRequest.TravelRequestId, approveRequest.ExecutiveOfficerBadgeNumber, approveRequest.ExecutiveOfficerName, 2);
                            }

                            if ((!string.IsNullOrEmpty(approveRequest.CEOForInternationalBadgeNumber)) && approveRequest.CEOForInternationalBadgeNumber != "?")
                            {
                                InsertApprovalList(approveRequest.TravelRequestId, approveRequest.CEOForInternationalBadgeNumber, approveRequest.CEOForInternationalName, 3);
                            }

                            if ((!string.IsNullOrEmpty(approveRequest.CEOForAPTABadgeNumber)) && approveRequest.CEOForAPTABadgeNumber != "?")
                            {
                                InsertApprovalList(approveRequest.TravelRequestId, approveRequest.CEOForAPTABadgeNumber, approveRequest.CEOForAPTAName, 4);
                            }
                            if ((!string.IsNullOrEmpty(approveRequest.TravelCoordinatorBadgeNumber)) && approveRequest.TravelCoordinatorBadgeNumber != "?")
                            {
                                InsertApprovalList(approveRequest.TravelRequestId, approveRequest.TravelCoordinatorBadgeNumber, approveRequest.TravelCoordinatorName, 5);
                            }
                        }
                        else
                        {
                            OracleCommand cmd3 = new OracleCommand();
                            cmd3.Connection = (OracleConnection)dbConn;
                            cmd3.CommandText = string.Format(@"Delete from TRAVELREQUEST_APPROVAL where TravelRequestId = {0}", approveRequest.TravelRequestId);
                            cmd3.ExecuteNonQuery();
                        }
                        //Update travel request _approval
                        OracleCommand cmd1 = new OracleCommand();
                        cmd1.Connection = (OracleConnection)dbConn;
                        // update travel request for the latest status 
                        cmd1.CommandText = string.Format(@"UPDATE  TRAVELREQUEST SET                                                  
                                                             STATUS = :p1,
                                                             REJECTTRAVELREQUEST = :p2 
                                                            WHERE TRAVELREQUESTID = {0}", approveRequest.TravelRequestId);
 
                        cmd1.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Rejected.ToString()));
                        cmd1.Parameters.Add(new OracleParameter("p2", rejectTravelRequest));

                        var rowsUpdated1 = cmd1.ExecuteNonQuery();

                        cmd1.Dispose();


                        dbConn.Close();
                        dbConn.Dispose();
 
                        //Send Email submitter and traveller 
                        string subject = string.Format(@"Travel Rejection  for Id - {0} ", approveRequest.TravelRequestId);
                        sendRejectionEmail(approveRequest.TravelRequestBadgeNumber,  subject,approveRequest.TravelRequestId, approveRequest.Comments, approveRequest.RejectReason);
                        response = true;
                    }
                    else
                    {
                        response = false;
                    }

                    command2.Dispose();
                }
                return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("Reject : " + ex.Message);
                throw;
            }
        }

        private void InsertApprovalList(string travelRequestId, string badgeNumber, string name, int count)
        {
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = (OracleConnection)dbConn;
            cmd2.CommandText = @"INSERT INTO TRAVELREQUEST_APPROVAL (                                                  
                                                            TRAVELREQUESTID,
                                                            BADGENUMBER,
                                                            APPROVERNAME,
                                                            APPROVALSTATUS,
                                                            APPROVALORDER,
                                                            TAUPDATEDAPPROVAL
                                                        )
                                                        VALUES
                                                            (:p1,:p2,:p3,:p4,:p5,:p6)";
            cmd2.Parameters.Add(new OracleParameter("p1", travelRequestId));
            cmd2.Parameters.Add(new OracleParameter("p2", badgeNumber));
            cmd2.Parameters.Add(new OracleParameter("p3", name));
            cmd2.Parameters.Add(new OracleParameter("p4", Common.ApprovalStatus.Pending.ToString()));
            cmd2.Parameters.Add(new OracleParameter("p5", count));
            cmd2.Parameters.Add(new OracleParameter("p6", "Y"));
            var rowsUpdated2 = cmd2.ExecuteNonQuery();
            cmd2.Dispose();
        }

        public TravelRequestInputResponse SaveTravelRequestInput(TravelRequestInput travelRequest)
        {
           
            string travelRequestId = string.Empty;
            int estimatedExpenseId = 0;
            TravelRequestInputResponse response = null;
            try
            {
                //Validate basic information
                ValidateTravelRequestInfo(travelRequest.TravelRequestData);

                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    // Insert or update travel request
                    travelRequestId = SaveTravelRequestNew(dbConn, travelRequest.TravelRequestData);                  
                    if (string.IsNullOrEmpty(travelRequestId))
                    {
                        throw new Exception("Couldn't save/update travel request information");
                    }

                    // Insert or update estimated expense
                    estimatedExpenseId = SaveEstimatedExpenseRequestNew(dbConn, travelRequest.EstimatedExpenseData, travelRequestId);
                    if(estimatedExpenseId == 0)
                    {
                        throw new Exception("Couldn't save/update estimated expense information");
                    }
                    // Insert or update FIS expense
                    SaveFISData(dbConn, travelRequest.FISData, travelRequestId);

                    response = new TravelRequestInputResponse() { TravelRequestId = travelRequestId, BadgeNumber = travelRequest.TravelRequestData.BadgeNumber };

                    return response;

                }
                dbConn.Close();
                dbConn.Dispose();
            }
            catch (Exception ex)
            {
                LogMessage.Log("SaveTravelRequestInput : " + ex.Message);
               throw new Exception("Couldn't save record into Travel Request - " );
            }
        }

        public  string SaveTravelRequestNew(DbConnection dbConn , TravelRequest travelRequest)
        {
            string travelRequestIdNew = string.Empty;
            try
            {
                var loginId = getUserName(dbConn, travelRequest.UserId);
                travelRequest.LoginId = loginId;
                if (travelRequest.TravelRequestId != 0)
                {
                    travelRequest.TravelRequestIdNew = travelRequest.TravelRequestId.ToString();
                }
                    if (string.IsNullOrEmpty(travelRequest.TravelRequestIdNew))
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = @"INSERT INTO TRAVELREQUEST (                                                  
                                                    BADGENUMBER,
                                                    NAME,
                                                    DIVISION,
                                                    SECTION,
                                                    ORGANIZATION,
                                                    MEETINGLOCATION,
                                                    MEETINGBEGINDATETIME,
                                                    DEPARTUREDATETIME,
                                                    MEETINGENDDATETIME,
                                                    RETURNDATETIME,
                                                    CREATIONDATETIME,
                                                    SELECTEDROLEID,
                                                    STATUS,
                                                    SUBMITTEDBYBADGENUMBER,
                                                    PURPOSE
                                                )
                                                VALUES
                                                    (:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13,:p14,:P15,:p16) returning TRAVELREQUESTID into :travelRequestId";
                        cmd.Parameters.Add(new OracleParameter("p2", travelRequest.BadgeNumber));
                        cmd.Parameters.Add(new OracleParameter("p3", travelRequest.Name));
                        cmd.Parameters.Add(new OracleParameter("p4", travelRequest.Division));
                        cmd.Parameters.Add(new OracleParameter("p5", travelRequest.Section));
                        cmd.Parameters.Add(new OracleParameter("p6", travelRequest.Organization));
                        cmd.Parameters.Add(new OracleParameter("p7", travelRequest.MeetingLocation));
                        cmd.Parameters.Add(new OracleParameter("p8", travelRequest.MeetingBeginDateTime));
                        cmd.Parameters.Add(new OracleParameter("p9", travelRequest.DepartureDateTime));
                        cmd.Parameters.Add(new OracleParameter("p10", travelRequest.MeetingEndDateTime));
                        cmd.Parameters.Add(new OracleParameter("p11", travelRequest.ReturnDateTime));
                        cmd.Parameters.Add(new OracleParameter("p12", DateTime.Now));
                        cmd.Parameters.Add(new OracleParameter("p13", travelRequest.SelectedRoleId));
                        cmd.Parameters.Add(new OracleParameter("p14", ApprovalStatus.New.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p15", travelRequest.SubmittedByBadgeNumber));
                        cmd.Parameters.Add(new OracleParameter("p16", travelRequest.Purpose));
                        cmd.Parameters.Add("travelRequestId", OracleDbType.Int32, ParameterDirection.ReturnValue);
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        travelRequestIdNew = cmd.Parameters["travelRequestId"].Value.ToString();
                        cmd.Dispose();
                    }
                    else
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST SET                                                  
                                                BADGENUMBER = :p2,
                                                NAME = :p3,
                                                DIVISION  = :p4,
                                                SECTION  = :p5,
                                                ORGANIZATION  = :p6,
                                                MEETINGLOCATION  = :p7,
                                                MEETINGBEGINDATETIME  = :p8,
                                                DEPARTUREDATETIME  = :p9,
                                                MEETINGENDDATETIME  = :p10,
                                                RETURNDATETIME  = :p11,
                                                LASTUPDATEDDATETIME  = :p12,
                                                PURPOSE = :p13
                                                WHERE TRAVELREQUESTID = {0}", travelRequest.TravelRequestId);
                        cmd.Parameters.Add(new OracleParameter("p2", travelRequest.BadgeNumber));
                        cmd.Parameters.Add(new OracleParameter("p3", travelRequest.Name));
                        cmd.Parameters.Add(new OracleParameter("p4", travelRequest.Division));
                        cmd.Parameters.Add(new OracleParameter("p5", travelRequest.Section));
                        cmd.Parameters.Add(new OracleParameter("p6", travelRequest.Organization));
                        cmd.Parameters.Add(new OracleParameter("p7", travelRequest.MeetingLocation));
                        cmd.Parameters.Add(new OracleParameter("p8", travelRequest.MeetingBeginDateTime));
                        cmd.Parameters.Add(new OracleParameter("p9", travelRequest.DepartureDateTime));
                        cmd.Parameters.Add(new OracleParameter("p10", travelRequest.MeetingEndDateTime));
                        cmd.Parameters.Add(new OracleParameter("p11", travelRequest.ReturnDateTime));
                        cmd.Parameters.Add(new OracleParameter("p12", DateTime.Now));
                        cmd.Parameters.Add(new OracleParameter("p13", travelRequest.Purpose));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        travelRequestIdNew = travelRequest.TravelRequestId.ToString();
                        cmd.Dispose();
                    }
            }
            catch (Exception ex)
            {

                LogMessage.Log("SaveTravelRequestNew : " + ex.Message);
                throw new Exception("Couldn't insert/update record into Travel Request - " );
            }

            return travelRequestIdNew;
        }

        public int SaveEstimatedExpenseRequestNew(DbConnection dbConn, EstimatedExpense request, string travelRequestId)
        {
            int estimatedExpenseId = 0;
            try
            {
                if (request.EstimatedExpenseId == 0)
                {
                    using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = @"INSERT INTO TRAVELREQUEST_ESTIMATEDEXPENSE(
                        TRAVELREQUESTID,
                        ADVANCELODGING,
                        ADVANCEAIRFARE,
						ADVANCEREGISTRATION,
						ADVANCEMEALS,
						ADVANCECARRENTAL,
						ADVANCEMISCELLANEOUS,
						ADVANCETOTAL,
                        TOTALESTIMATEDLODGE,
                        TOTALESTIMATEDAIRFARE,
						TOTALESTIMATEDREGISTRATION,
						TOTALESTIMATEDMEALS,
						TOTALESTIMATEDCARRENTAL,
						TOTALESTIMATEDMISCELLANEOUS,
						TOTALESTIMATEDTOTAL,
                        HOTELNAMEANDADDRESS,
						SCHEDULE,
						PAYABLETOANDADDRESS,
						NOTE,
						AGENCYNAMEANDRESERVATION,
						SHUTTLE,
						CASHADVANCE,
						DATENEEDEDBY,
				        APPROVEDLODGE,
                        APPROVEDAIRFARE,
                        APPROVEDMEALS,
                        ACTUALLODGE,
                        ACTUALAIRFARE,
                        ACTUALMEALS,
                        PERSONALTRAVELEXPENSE,
                        PERSONALTRAVELINCLUDED,
                        SPECIALINSTRUCTIONS,
                        LASTUPDATEDDATETIME
						 ) VALUES (:p1 ,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13,:p14,:p15,:p16,:p17,:p18,:p19,:p20,:p21,:p22,:p23,:p24,:p25,:p26, :p27, :p28, :p29,:p30,:p31,:p32, :p33 ) returning ESTIMATEDEXPENSEID into : estimatedExpenseId ";
                        cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
                        cmd.Parameters.Add(new OracleParameter("p2", request.AdvanceLodging));
                        cmd.Parameters.Add(new OracleParameter("p3", request.AdvanceAirFare));
                        cmd.Parameters.Add(new OracleParameter("p4", request.AdvanceRegistration));
                        cmd.Parameters.Add(new OracleParameter("p5", request.AdvanceMeals));
                        cmd.Parameters.Add(new OracleParameter("p6", request.AdvanceCarRental));
                        cmd.Parameters.Add(new OracleParameter("p7", request.AdvanceMiscellaneous));
                        cmd.Parameters.Add(new OracleParameter("p8", request.AdvanceTotal));
                        cmd.Parameters.Add(new OracleParameter("p9", request.TotalEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p10", request.TotalEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p11", request.TotalEstimatedRegistration));
                        cmd.Parameters.Add(new OracleParameter("p12", request.TotalEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p13", request.TotalEstimatedCarRental));
                        cmd.Parameters.Add(new OracleParameter("p14", request.TotalEstimatedMiscellaneous));
                        cmd.Parameters.Add(new OracleParameter("p15", request.TotalEstimatedTotal));
                        cmd.Parameters.Add(new OracleParameter("p16", request.HotelNameAndAddress));
                        cmd.Parameters.Add(new OracleParameter("p17", request.Schedule));
                        cmd.Parameters.Add(new OracleParameter("p18", request.PayableToAndAddress));
                        cmd.Parameters.Add(new OracleParameter("p19", request.Note));
                        cmd.Parameters.Add(new OracleParameter("p20", request.AgencyNameAndReservation));
                        cmd.Parameters.Add(new OracleParameter("p21", request.Shuttle));
                        cmd.Parameters.Add(new OracleParameter("p22", request.CashAdvance));
                        cmd.Parameters.Add(new OracleParameter("p23", request.DateNeededBy));
                        cmd.Parameters.Add(new OracleParameter("p24", request.TotalOtherEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p25", request.TotalOtherEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p26", request.TotalOtherEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p27", request.TotalActualEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p28", request.TotalActualEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p29", request.TotalActualEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p30", request.PersonalTravelExpense));
                        cmd.Parameters.Add(new OracleParameter("p31", request.PersonalTravelIncluded.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p32", request.SpecialInstruction));
                        cmd.Parameters.Add(new OracleParameter("p33", DateTime.Now));
                        cmd.Parameters.Add("estimatedExpenseId", OracleDbType.Int32, ParameterDirection.ReturnValue);
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        estimatedExpenseId = Decimal.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)(cmd.Parameters["estimatedExpenseId"].Value)).Value);

                        cmd.Dispose();
                        dbConn.Close();
                        dbConn.Dispose();
                    }
                }
                else
                {
                    using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST_ESTIMATEDEXPENSE SET
                        TRAVELREQUESTID = :p1,
                        ADVANCELODGING = :p2,
                        ADVANCEAIRFARE = :p3,
						ADVANCEREGISTRATION = :p4,
						ADVANCEMEALS = :p5,
						ADVANCECARRENTAL = :p6,
						ADVANCEMISCELLANEOUS = :p7,
						ADVANCETOTAL = :p8,
                        TOTALESTIMATEDLODGE = :p9,
                        TOTALESTIMATEDAIRFARE = :p10,
						TOTALESTIMATEDREGISTRATION = :p11,
						TOTALESTIMATEDMEALS = :p12,
						TOTALESTIMATEDCARRENTAL = :p13,
						TOTALESTIMATEDMISCELLANEOUS = :p14,
						TOTALESTIMATEDTOTAL = :p15,
                        HOTELNAMEANDADDRESS = :p16,
						SCHEDULE = :p17,
						PAYABLETOANDADDRESS = :p18,
						NOTE = :p19,
						AGENCYNAMEANDRESERVATION = :p20,
						SHUTTLE = :p21,
						CASHADVANCE = :p22,
						DATENEEDEDBY  = :p23,
                        APPROVEDLODGE = :p24,
                        APPROVEDAIRFARE = :p25,
                        APPROVEDMEALS = :26,
                        ACTUALLODGE = :p27,
                        ACTUALAIRFARE = :p28,
                        ACTUALMEALS = :p29,
                        PERSONALTRAVELEXPENSE = :p30,
                        PERSONALTRAVELINCLUDED = :p31,
                        SPECIALINSTRUCTIONS =:p32,
                        LASTUPDATEDDATETIME = :p33
                        WHERE TRAVELREQUESTID = {0}", request.TravelRequestId);
                        cmd.Parameters.Add(new OracleParameter("p1", request.TravelRequestId));
                        cmd.Parameters.Add(new OracleParameter("p2", request.AdvanceLodging));
                        cmd.Parameters.Add(new OracleParameter("p3", request.AdvanceAirFare));
                        cmd.Parameters.Add(new OracleParameter("p4", request.AdvanceRegistration));
                        cmd.Parameters.Add(new OracleParameter("p5", request.AdvanceMeals));
                        cmd.Parameters.Add(new OracleParameter("p6", request.AdvanceCarRental));
                        cmd.Parameters.Add(new OracleParameter("p7", request.AdvanceMiscellaneous));
                        cmd.Parameters.Add(new OracleParameter("p8", request.AdvanceTotal));
                        cmd.Parameters.Add(new OracleParameter("p9", request.TotalEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p10", request.TotalEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p11", request.TotalEstimatedRegistration));
                        cmd.Parameters.Add(new OracleParameter("p12", request.TotalEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p13", request.TotalEstimatedCarRental));
                        cmd.Parameters.Add(new OracleParameter("p14", request.TotalEstimatedMiscellaneous));
                        cmd.Parameters.Add(new OracleParameter("p15", request.TotalEstimatedTotal));
                        cmd.Parameters.Add(new OracleParameter("p16", request.HotelNameAndAddress));
                        cmd.Parameters.Add(new OracleParameter("p17", request.Schedule));
                        cmd.Parameters.Add(new OracleParameter("p18", request.PayableToAndAddress));
                        cmd.Parameters.Add(new OracleParameter("p19", request.Note));
                        cmd.Parameters.Add(new OracleParameter("p20", request.AgencyNameAndReservation));
                        cmd.Parameters.Add(new OracleParameter("p21", request.Shuttle));
                        cmd.Parameters.Add(new OracleParameter("p22", request.CashAdvance));
                        cmd.Parameters.Add(new OracleParameter("p23", request.DateNeededBy));
                        cmd.Parameters.Add(new OracleParameter("p24", request.TotalOtherEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p25", request.TotalOtherEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p26", request.TotalOtherEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p27", request.TotalActualEstimatedLodge));
                        cmd.Parameters.Add(new OracleParameter("p28", request.TotalActualEstimatedAirFare));
                        cmd.Parameters.Add(new OracleParameter("p29", request.TotalActualEstimatedMeals));
                        cmd.Parameters.Add(new OracleParameter("p30", request.PersonalTravelExpense));
                        cmd.Parameters.Add(new OracleParameter("p31", request.PersonalTravelIncluded.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p32", request.SpecialInstruction));
                        cmd.Parameters.Add(new OracleParameter("p33", DateTime.Now));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        estimatedExpenseId = request.EstimatedExpenseId;
                        cmd.Dispose();
                        dbConn.Close();
                        dbConn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {

                LogMessage.Log("SaveEstimatedExpenseRequestNew :" + ex.Message);
                throw new Exception("Could not save estimated expense successfully");
            }

            return estimatedExpenseId;
        }

        public TravelRequestInput GetTravelRequestDetailNew(string travelRequestId)
        {
            TravelRequest travelRequest = null;
            EstimatedExpense estimatedExpense = null;
            FIS fisDetails = null;
            TravelRequestInput travelRequestInput = null;
            try
            {
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {                    
                    travelRequest = GetTravelRequestDetail(dbConn, travelRequestId);
                    estimatedExpense = estimatedExpenseRepository.GetTravelRequestDetailNew(dbConn, travelRequestId);
                    fisDetails = fisRepository.GetFISdetails(dbConn, travelRequestId);
                    dbConn.Close();
                    dbConn.Dispose();
                }
                travelRequestInput = new TravelRequestInput()
                {
                    TravelRequestData = travelRequest,
                    EstimatedExpenseData = estimatedExpense,
                    FISData = fisDetails
                };
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error while getting travel request details - "+ex.Message);
                throw;
            }
            return travelRequestInput;
        }

        public void SaveFISData(DbConnection dbConn, FIS request, string travelRequestId)
        {
            try
            {
                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = (OracleConnection)dbConn;
                cmd1.CommandText = string.Format(@" DELETE FROM TRAVELREQUEST_FIS WHERE TRAVELREQUESTID = {0}", travelRequestId);
                cmd1.ExecuteNonQuery();

                foreach (var fis in request.FISDetails)
                {
                    if (!string.IsNullOrEmpty(fis.CostCenterId) && fis.CostCenterId != "?")
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"INSERT INTO TRAVELREQUEST_FIS (                                                  
                                                            TRAVELREQUESTID,
                                                            COSTCENTERID ,
                                                            LINEITEM ,
                                                            PROJECTID ,
                                                            TASK ,
                                                            AMOUNT ,
                                                            TOTALAMOUNT 
                                                        )
                                                        VALUES
                                                            (:p1,:p2,:p3,:p4,:p5,:p6,:p7)");
                        cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
                        cmd.Parameters.Add(new OracleParameter("p2", fis.CostCenterId));
                        cmd.Parameters.Add(new OracleParameter("p3", fis.LineItem));
                        cmd.Parameters.Add(new OracleParameter("p4", fis.ProjectId));
                        cmd.Parameters.Add(new OracleParameter("p5", fis.Task));
                        cmd.Parameters.Add(new OracleParameter("p6", fis.Amount));
                        cmd.Parameters.Add(new OracleParameter("p7", request.TotalAmount));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }

                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("SaveFISData : " + ex.Message);
                throw new Exception("Couldn't insert/update fis record  ");
            }
            //try
            //{                
            //    if (!CheckFISDataExists(dbConn, travelRequestId))
            //    {
            //        foreach (var fis in request.FISDetails)
            //        {
            //            if (!string.IsNullOrEmpty(fis.CostCenterId))
            //            {
            //                OracleCommand cmd = new OracleCommand();
            //                cmd.Connection = (OracleConnection)dbConn;
            //                cmd.CommandText = string.Format(@"INSERT INTO TRAVELREQUEST_FIS (                                                  
            //                                            TRAVELREQUESTID,
            //                                            COSTCENTERID ,
            //                                            LINEITEM ,
            //                                            PROJECTID ,
            //                                            TASK ,
            //                                            AMOUNT ,
            //                                            TOTALAMOUNT 
            //                                        )
            //                                        VALUES
            //                                            (:p1,:p2,:p3,:p4,:p5,:p6,:p7)");
            //                cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
            //                cmd.Parameters.Add(new OracleParameter("p2", fis.CostCenterId));
            //                cmd.Parameters.Add(new OracleParameter("p3", fis.LineItem));
            //                cmd.Parameters.Add(new OracleParameter("p4", fis.ProjectId));
            //                cmd.Parameters.Add(new OracleParameter("p5", fis.Task));
            //                cmd.Parameters.Add(new OracleParameter("p6", fis.Amount));
            //                cmd.Parameters.Add(new OracleParameter("p7", request.TotalAmount));
            //                var rowsUpdated = cmd.ExecuteNonQuery();
            //                cmd.Dispose();
            //            }
            //        }
            //    }

            //    else
            //    {

            //        foreach (var fis in request.FISDetails)
            //        {
            //            if (!string.IsNullOrEmpty(fis.CostCenterId))
            //            {
            //                OracleCommand cmd = new OracleCommand();
            //                cmd.Connection = (OracleConnection)dbConn;
            //                cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST_FIS SET                                                  
            //                                    TRAVELREQUESTID = :p1,
            //                                            COSTCENTERID =:p2,
            //                                            LINEITEM =:p3,
            //                                            PROJECTID =:p4,
            //                                            TASK =:p5,
            //                                            AMOUNT =:p6,
            //                                            TOTALAMOUNT =:p7
            //                                    WHERE TRAVELREQUESTID = {0}", travelRequestId);
            //                cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
            //                cmd.Parameters.Add(new OracleParameter("p2", fis.CostCenterId));
            //                cmd.Parameters.Add(new OracleParameter("p3", fis.LineItem));
            //                cmd.Parameters.Add(new OracleParameter("p4", fis.ProjectId));
            //                cmd.Parameters.Add(new OracleParameter("p5", fis.Task));
            //                cmd.Parameters.Add(new OracleParameter("p6", fis.Amount));
            //                cmd.Parameters.Add(new OracleParameter("p7", request.TotalAmount));
            //                var rowsUpdated = cmd.ExecuteNonQuery();
            //                cmd.Dispose();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogMessage.Log("SaveFISData : "+ex.Message);
            //    throw new Exception("Couldn't insert/update record into Travel Request "  );
            //}


        }


        public TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId)
        {
            TravelRequestSubmitDetailResponse response = null;
            TravelRequestSubmitDetail travelRequestSubmitDetail = null;
            try
            {
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    string query = string.Format("select TRAVELREQUESTID,BADGENUMBER,APPROVALORDER,APPROVEROTHERBADGENUMBER, APPROVERNAME from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID='{0}' ", travelRequestId, ApprovalStatus.Pending);
                    travelRequestSubmitDetail = new TravelRequestSubmitDetail();
                    if (GetRejectedTravelRequestStatus(dbConn, travelRequestId.ToString()))
                    {
                        query = string.Format("select TRAVELREQUESTID,BADGENUMBER,APPROVALORDER,APPROVEROTHERBADGENUMBER, APPROVERNAME from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID='{0}' and APPROVALSTATUS='{1}' ", travelRequestId, ApprovalStatus.Pending.ToString());
                    }
                    OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                       
                    if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                            switch (Convert.ToInt32(dataReader["APPROVALORDER"]))
                            {
                                case 1:
                                   
                                    travelRequestSubmitDetail.DepartmentHeadBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.DepartmentHeadOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.DepartmentHeadOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 2:
                                    travelRequestSubmitDetail.ExecutiveOfficerBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.ExecutiveOfficerOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.ExecutiveOfficerOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 3:
                                    travelRequestSubmitDetail.CEOInternationalBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOInternationalOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 4:
                                    travelRequestSubmitDetail.CEOAPTABadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 5:
                                    travelRequestSubmitDetail.TravelCoordinatorBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                   
                                    break;
                            }
                            travelRequestSubmitDetail.TravelRequestId = dataReader["TRAVELREQUESTID"].ToString();
                            travelRequestSubmitDetail.RejectedTravelRequest = GetRejectedTravelRequestStatus(dbConn, dataReader["TRAVELREQUESTID"].ToString());

                            string agree = string.Empty;
                            string submitter = string.Empty;
                            //travelRequestSubmitDetail.Agree = GetAgeedAcknowledgement(dbConn, travelRequestId);
                            GetSubmitterName(dbConn, travelRequestId,out agree,out submitter);
                            travelRequestSubmitDetail.SubmitterName = submitter;
                            travelRequestSubmitDetail.Agree = (agree == "Y")? true : false;

                            }
                        }
                        command.Dispose();
                        dataReader.Close();
                    response = new TravelRequestSubmitDetailResponse();
                    response.TravelRequestSubmitDetail = travelRequestSubmitDetail;
                    response.CEOApprovalRequired = GetCEOApprovalStatus(dbConn, travelRequestId);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            //response = new TravelRequestSubmitDetailResponse();
            //response.TravelRequestSubmitDetail = travelRequestSubmitDetail;
            //response.CEOApprovalRequired = GetCEOApprovalStatus(dbConn, travelRequestId);

            return response;
        }

        private bool GetCEOApprovalStatus(DbConnection dbconn, int travelRequestId)
        {
            var result = false;
            try
            {
                string query = string.Format(@"select CASHADVANCE FROM TRAVELREQUEST_ESTIMATEDEXPENSE where TRAVELREQUESTID= '{0}' and CASHADVANCE > 0 ", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    result =true ;
                }
                command.Dispose();
                dataReader.Close();
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetRejectedTravelRequestStatus : " + ex.Message);
                throw;
            }
            return result;

        }

        private bool GetRejectedTravelRequestStatus(DbConnection dbconn, string travelRequestId)
        {
            var result = false;
            try
            {
                string query = string.Format(@"select REJECTTRAVELREQUEST FROM TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        result = (dataReader["REJECTTRAVELREQUEST"].ToString()) == "true" ? true : false;

                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve rejected travel request status");
                }
              //  command.Dispose();
               // dataReader.Close();
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetRejectedTravelRequestStatus : " + ex.Message);
                throw;
            }
            return result;

        }

        private void GetSubmitterName(DbConnection dbConn, int travelRequestId,out string agree, out string submitter)
        {
            try
            {

                string response = string.Empty;
                agree = string.Empty;
                submitter = string.Empty;
                string query = string.Format("Select * from TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        submitter = dataReader["SUBMITTEDBYUSERNAME"].ToString();
                        agree = dataReader["AGREE"].ToString();
                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve submitter name");
                }
                command.Dispose();
                dataReader.Close();
              //  return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetSubmitterName : " + ex.Message);
                throw;
            }
        }

        private bool GetAgeedAcknowledgement(DbConnection dbConn, int travelRequestId)
        {
            throw new NotImplementedException();
        }

        public bool Cancel(string travelRequestId, int travelRequestBadgeNumber, string comments)
        {
            using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
            {
                //Update travel request _approval
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = (OracleConnection)dbConn;
                cmd.CommandText = string.Format(@"UPDATE  TRAVELREQUEST SET                                                  
                                                         STATUS = :p1,
                                                         COMMENTS = :p2 
                                                        WHERE TRAVELREQUESTID = {0}", travelRequestId);
                cmd.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Cancelled.ToString()));
                cmd.Parameters.Add(new OracleParameter("p2", comments));
                cmd.ExecuteNonQuery();
            }

            return true;
        }



        #region   REIMBURSE Section

        //public List<TravelRequestDetails> GetApprovedTravelRequestList(int submittedBadgeNumber, int selectedRoleId)
        //{
        //    try
        //    {
        //        List<TravelRequestDetails> response = new List<TravelRequestDetails>();

        //        using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
        //        {
        //            if (selectedRoleId == 1 || selectedRoleId == 2)
        //            {

        //                string query = string.Format("Select * from TRAVELREQUEST where BADGENUMBER= {0} AND SELECTEDROLEID ={1} AND STATUS = '{2}' order by CREATIONDATETIME desc", submittedBadgeNumber, selectedRoleId, ApprovalStatus.Complete);
        //                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //                command.CommandText = query;
        //                DbDataReader dataReader = command.ExecuteReader();
        //                if (dataReader.HasRows)
        //                {
        //                    while (dataReader.Read())
        //                    {
        //                        response.Add(new TravelRequestDetails()
        //                        {
        //                            TravelRequestId = Convert.ToInt32(dataReader["TravelRequestId"]),
        //                            //Description = dataReader["PURPOSE"].ToString(),
        //                            SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
        //                            SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
        //                            RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            EditActionVisible = EditActionEligible(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])) ? true : false,
        //                            ViewActionVisible = true,
        //                            ApproveActionVisible = false,
        //                            Status = dataReader["STATUS"].ToString(),
        //                            Purpose = dataReader["PURPOSE"].ToString()
        //                        });
        //                    }
        //                }
        //                command.Dispose();
        //                dataReader.Close();
        //            }
        //            else
        //            {
        //                string query = string.Format(@"SELECT
        //                                                 *
        //                                                FROM
        //                                                 TRAVELREQUEST
        //                                                WHERE
        //                                                 TRAVELREQUESTID IN (
        //                                                  SELECT
        //                                                   TRAVELREQUESTId
        //                                                  FROM
        //                                                   TRAVELREQUEST_APPROVAL
        //                                                  WHERE
        //                                                   BADGENUMBER = {0}
        //                                                 )   order by CREATIONDATETIME desc", submittedBadgeNumber);
        //                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //                command.CommandText = query;
        //                DbDataReader dataReader = command.ExecuteReader();
        //                if (dataReader.HasRows)
        //                {
        //                    while (dataReader.Read())
        //                    {
        //                        response.Add(new TravelRequestDetails()
        //                        {
        //                            TravelRequestId = Convert.ToInt32(dataReader["TravelRequestId"]),
        //                            Description = dataReader["Purpose"].ToString(),
        //                            SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
        //                            SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
        //                            RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
        //                            EditActionVisible = false,
        //                            ViewActionVisible = true,
        //                            ApproveActionVisible = getApprovalSatus(dbConn, Convert.ToInt32(dataReader["TravelRequestId"]), submittedBadgeNumber) ? true : false,
        //                            Status = dataReader["STATUS"].ToString(),
        //                            Purpose = dataReader["Purpose"].ToString()
        //                        });
        //                    }
        //                }
        //                command.Dispose();
        //                dataReader.Close();
        //            }
        //            dbConn.Close();
        //            dbConn.Dispose();
        //            return response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage.Log("GetTravelRequestList : " + ex.Message);
        //        throw;
        //    }
        //}

        //public ReimbursementAllTravelInformation GetTravelRequestInfoForReimbursement(string travelRequestId)
        //{
        //    ReimbursementTravelRequestDetails travelReimbursementDetails = null;
        //    EstimatedExpense estimatedExpense = null;
        //    FIS fisDetails = null;
        //    ReimbursementAllTravelInformation travelRequestReimbursementDetails = null ;

        //    try
        //    {
        //        using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
        //        {
        //            travelReimbursementDetails = GetTravelReimbursementDetails(dbConn, travelRequestId);
        //            estimatedExpense = estimatedExpenseRepository.GetTravelRequestDetailNew(dbConn, travelRequestId);
        //            fisDetails = fisRepository.GetFISdetails(dbConn, travelRequestId);
        //            dbConn.Close();
        //            dbConn.Dispose();
        //        }
        //        travelRequestReimbursementDetails = new ReimbursementAllTravelInformation()
        //        {
        //             TravelReimbursementDetails = travelReimbursementDetails,
        //              Fis = fisDetails,
        //              CashAdvance = estimatedExpense.CashAdvance
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage.Log("Error while getting travel request details - " + ex.Message);
        //        throw;
        //    }

        //    return travelRequestReimbursementDetails;
        //}

        //private ReimbursementTravelRequestDetails GetTravelReimbursementDetails(DbConnection dbConn, string travelRequestId)
        //{
        //    try
        //    {

        //        ReimbursementTravelRequestDetails response = null;
        //        string query = string.Format("Select * from TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
        //        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //        command.CommandText = query;
        //        DbDataReader dataReader = command.ExecuteReader();

        //        if (dataReader.HasRows)
        //        {
        //            while (dataReader.Read())
        //            {

        //                response = new ReimbursementTravelRequestDetails()
        //                {
        //                    BadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]),
        //                    Name = dataReader["NAME"].ToString(),
        //                    Division = dataReader["DIVISION"].ToString(),                             
        //                    DepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]),
        //                    ReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"])                             
        //                };
        //            }

        //            var employeeDetails =  GetEmployeeDetails(response.BadgeNumber);
        //            response.CostCenterId = employeeDetails.Result.CostCenter;
        //            response.Department = employeeDetails.Result.Department;
        //            response.Extension = employeeDetails.Result.EmployeeWorkPhone;
        //            response.VendorNumber = GetVendorNumber(response.BadgeNumber).Result;
        //        }
        //        else
        //        {
        //            throw new Exception("Couldn't retrieve travel request");
        //        }
        //        command.Dispose();
        //        dataReader.Close();
        //        return response;

        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
        //        throw;
        //    }
        //}

        //private string GetVendorId(DbConnection dbConn, int badgeNumber)
        //{
        //    throw new NotImplementedException();
        //}


        //public Task<int> SaveTravelRequestReimbursement(ReimbursementInput reimbursementRequest)
        //{
        //    string travelRequestId = string.Empty;
        //    int estimatedExpenseId = 0;
        //    TravelRequestInputResponse response = null;
        //    try
        //    {
        //        //Validate basic information


        //        using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
        //        {
        //            // Insert or update travel request
        //            travelRequestId = SaveReimbursementTravelRequestDetails(dbConn, reimbursementRequest.reimbursementDetails);
        //            if (string.IsNullOrEmpty(travelRequestId))
        //            {
        //                throw new Exception("Couldn't save/update travel request information");
        //            }

        //            // Insert or update estimated expense
        //            estimatedExpenseId = SaveEstimatedExpenseRequestNew(dbConn, reimbursementRequest.EstimatedExpenseData, travelRequestId);
        //            if (estimatedExpenseId == 0)
        //            {
        //                throw new Exception("Couldn't save/update estimated expense information");
        //            }
        //            // Insert or update FIS expense
        //            SaveFISData(dbConn, reimbursementRequest.fis, travelRequestId);

        //            response = new TravelRequestInputResponse() { TravelRequestId = travelRequestId, BadgeNumber = travelRequest.TravelRequestData.BadgeNumber };

        //            return response;

        //        }
        //        dbConn.Close();
        //        dbConn.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage.Log("SaveTravelRequestInput : " + ex.Message);
        //        throw new Exception("Couldn't save record into Travel Request - ");
        //    }
        //}

        //private string SaveReimbursementTravelRequestDetails(DbConnection dbConn, ReimbursementDetails reimbursementDetails)
        //{
        //    throw new NotImplementedException();
        //}


        #endregion

        #region  Helper Methods

        private string getLastApproverDateTime(DbConnection dbConn, int travelRequestId)
        {
            string response = "";
            string query = string.Format(@"Select ApprovalDateTime from (
                                            SELECT
	                                        APPROVALDATETIME
                                        FROM
	                                        TRAVELREQUEST_APPROVAL
                                        WHERE
	                                        TRAVELREQUESTID = {0}
                                        AND APPROVALDATETIME IS NOT NULL
                                        ORDER BY
	                                        APPROVALDATETIME DESC)
                                            where ROWNUM =1", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = dataReader["APPROVALDATETIME"].ToString();
                }
            }
            command.Dispose();
            dataReader.Close();
            return response;
        }

        //private string getReimburseLastApproverDateTime(DbConnection dbConn, int travelRequestId)
        //{
        //    string response = "";
        //    string query = string.Format(@"Select ApprovalDateTime from (
        //                                    SELECT
	       //                                 APPROVALDATETIME
        //                                FROM
	       //                                 TRAVEL_REIMBURSE_APPROVAL
        //                                WHERE
	       //                                 TRAVELREQUESTID = {0}
        //                                AND APPROVALDATETIME IS NOT NULL
        //                                ORDER BY
	       //                                 APPROVALDATETIME DESC)
        //                                    where ROWNUM =1", travelRequestId);

        //    OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //    command.CommandText = query;
        //    DbDataReader dataReader = command.ExecuteReader();
        //    if (dataReader.HasRows)
        //    {
        //        while (dataReader.Read())
        //        {
        //            response = dataReader["APPROVALDATETIME"].ToString();
        //        }
        //    }
        //    command.Dispose();
        //    dataReader.Close();
        //    return response;
        //}
        private string getLastApproverName(DbConnection dbConn, int travelRequestId)
        {
            string response = "";
            string query = string.Format(@"select APPROVERNAME from (
                                                                    SELECT
	                                                                    APPROVERNAME
                                                                    FROM
	                                                                    TRAVELREQUEST_APPROVAL
                                                                    WHERE
	                                                                    TRAVELREQUESTID = {0}
                                                                    AND APPROVALDATETIME IS NOT NULL
                                                                    ORDER BY
	                                                                    APPROVALDATETIME desc )
                                                                    where ROWNUM =1", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = dataReader["APPROVERNAME"].ToString();
                }
            }
            command.Dispose();
            dataReader.Close();
            return response;
        }

        //private string getReimburseLastApproverName(DbConnection dbConn, int travelRequestId)
        //{
        //    string response = "";
        //    string query = string.Format(@"select APPROVERNAME from (
        //                                                            SELECT
	       //                                                             APPROVERNAME
        //                                                            FROM
	       //                                                             TRAVEL_REIMBURSE_APPROVAL
        //                                                            WHERE
	       //                                                             TRAVELREQUESTID = {0}
        //                                                            AND APPROVALDATETIME IS NOT NULL
        //                                                            ORDER BY
	       //                                                             APPROVALDATETIME desc )
        //                                                            where ROWNUM =1", travelRequestId);

        //    OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //    command.CommandText = query;
        //    DbDataReader dataReader = command.ExecuteReader();
        //    if (dataReader.HasRows)
        //    {
        //        while (dataReader.Read())
        //        {
        //            response = dataReader["APPROVERNAME"].ToString();
        //        }
        //    }
        //    command.Dispose();
        //    dataReader.Close();
        //    return response;
        //}
        private string GetApproversListByTravelRequestId(DbConnection dbConn, int travelRequestId)
        {
            string query = string.Empty;
            string query1 = string.Format("select * from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID = {0} AND TAUPDATEDAPPROVAL is not NULL order by ApprovalOrder", travelRequestId);
            OracleCommand command1 = new OracleCommand(query1, (OracleConnection)dbConn);
            command1.CommandText = query1;
            DbDataReader dataReader1 = command1.ExecuteReader();
            if (dataReader1.HasRows)
            {
                  query = string.Format("select APPROVERNAME from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID = {0} AND TAUPDATEDAPPROVAL is not NULL order by ApprovalOrder", travelRequestId);

            } else
            {
                query = string.Format("select APPROVERNAME from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID = {0}  AND TAUPDATEDAPPROVAL is  NULL order by ApprovalOrder", travelRequestId);
            }
            string response = string.Empty;
           
            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            List<string> result = new List<string>();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    result.Add(dataReader["APPROVERNAME"].ToString());
                }
            }
            response = string.Join(", ", result);
            command.Dispose();
            dataReader.Close();
            return response;
        }

        //private string GetReimburseApproversListByTravelRequestId(DbConnection dbConn, int travelRequestId)
        //{
        //    string response = string.Empty;
        //    string query = string.Format("select APPROVERNAME from TRAVEL_REIMBURSE_APPROVAL where TRAVELREQUESTID = {0} order by ApprovalOrder", travelRequestId);
        //    OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
        //    command.CommandText = query;
        //    DbDataReader dataReader = command.ExecuteReader();
        //    List<string> result = new List<string>();
        //    if (dataReader.HasRows)
        //    {
        //        while (dataReader.Read())
        //        {
        //            result.Add(dataReader["APPROVERNAME"].ToString());
        //        }
        //    }
        //    response = string.Join(", ", result);
        //    command.Dispose();
        //    dataReader.Close();
        //    return response;
        //}

        public bool getApprovalSatus(DbConnection dbConn, int travelRequestId, int approverBadgeNumber)
        {
            bool result = false;
            int response = 0;
            string query = string.Format(@"SELECT
	                                            *
                                            FROM
	                                            (
		                                            SELECT
			                                            BadgeNumber, APPROVEROTHERBADGENUMBER
		                                            FROM
			                                            TRAVELREQUEST_APPROVAL
		                                            WHERE
			                                            TRAVELREQUESTID = {0}
		                                            AND APPROVALDATETIME IS NULL
		                                            ORDER BY
			                                            APPROVALORDER
	                                            )
                                            WHERE
	                                            ROWNUM <= 1", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = Convert.ToInt32(dataReader["BADGENUMBER"].ToString());
                    if (response == -1)
                    {
                        response = Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"].ToString());
                    }
                }
            }
            command.Dispose();
            dataReader.Close();

            if (response == approverBadgeNumber && (!EditActionEligible(dbConn,travelRequestId)))
            {
                result = true;
            }

            return result;
        }

        public bool EditActionEligible(DbConnection dbConn, int travelRequestId)
        {
            string response = "";
            string query = string.Format(@"SELECT
	                                        STATUS
                                        FROM
	                                        TRAVELREQUEST 
                                        WHERE
	                                        TRAVELREQUESTID = {0}  ", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = dataReader["STATUS"].ToString();
                }
            }
            command.Dispose();
            dataReader.Close();

            if (response == ApprovalStatus.New.ToString() || response == ApprovalStatus.Rejected.ToString() )
            {
                return true;
            }
            return false;

        }

        public bool EditActionEligibleForTravelAdimin(DbConnection dbConn, int travelRequestId)
        {
            string response = "";
            string query = string.Format(@"SELECT
	                                        STATUS
                                        FROM
	                                        TRAVELREQUEST 
                                        WHERE
	                                        TRAVELREQUESTID = {0}  ", travelRequestId);

            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = dataReader["STATUS"].ToString();
                }
            }
            command.Dispose();
            dataReader.Close();

            if (response == ApprovalStatus.Cancelled.ToString())
            {
                return false;
            }
            return true;

        }

        public bool CheckFISDataExists(DbConnection dbConn, string travelRequestId)
        {
            bool result = false;
            string query = string.Format(@"Select * from TravelRequest_fis where travelRequestId = {0}", travelRequestId);
            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            string userName = string.Empty;
            if (dataReader != null && dataReader.HasRows == true)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            command.Dispose();
            dataReader.Close();
            return result;
        }

        public int getNextApproverBadgeNumber(DbConnection dbConn, string travelRequestId)
        {
            var result = 0;
            string query = string.Format(@"SELECT
	                                                    BADGENUMBER,APPROVEROTHERBADGENUMBER
                                                    FROM
	                                                    (
		                                                    SELECT
			                                                    BADGENUMBER, APPROVEROTHERBADGENUMBER
		                                                    FROM
			                                                    TRAVELREQUEST_APPROVAL
		                                                    WHERE
			                                                    TRAVELREQUESTID = {0}
                                                            AND APPROVALSTATUS = '{1}'
		                                                    AND APPROVALDATETIME IS NULL
		                                                    ORDER BY
			                                                    APPROVALORDER 
	                                                    )
                                                    WHERE
	                                                    ROWNUM = 1", travelRequestId, ApprovalStatus.Pending);
            OracleCommand cmd1 = new OracleCommand(query, (OracleConnection)dbConn);
            cmd1.CommandText = query;
            DbDataReader dataReader = cmd1.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    result = Convert.ToInt32(dataReader["BADGENUMBER"].ToString());
                    if (result == -1)
                    {
                        result = Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"].ToString());
                    }
                }
               
            }
            else
            {
                result = 0;
            }
         //   cmd1.Dispose();
            dataReader.Close();
           // dbConn.Close();
           // dbConn.Dispose();
            return result;
        }





        #endregion

        #region Dashboard

        public List<TravelRequestDashboard> GetTravelRequestDashboardData()
        {
            try
            {
                var result = new List<TravelRequestDashboard>();
                 
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    string query1 = string.Format("Select Count(*) as count from TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.New);
                    OracleCommand command = new OracleCommand(query1, (OracleConnection)dbConn);
                    command.CommandText = query1;
                    DbDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "orange", Label = "New", Value = Convert.ToInt32(dataReader["Count"])});                            
                        }
                    }
                 
                    string query2 = string.Format("Select Count(*) as count from TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Pending);
                    command = new OracleCommand(query2, (OracleConnection)dbConn);
                    command.CommandText = query2;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "dodgeblue", Label = "Pending", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }
                    

                    string query3 = string.Format("Select Count(*) as count from TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Rejected);
                    command = new OracleCommand(query3, (OracleConnection)dbConn);
                    command.CommandText = query3;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "red", Label = "Rejected", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }
                    
                    string query4 = string.Format("Select Count(*) as count from TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Complete);
                    command = new OracleCommand(query4, (OracleConnection)dbConn);
                    command.CommandText = query4;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "green", Label = "Completed", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }
                    
                    string query5 = string.Format("Select Count(*) as count from TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Cancelled);
                    command = new OracleCommand(query5, (OracleConnection)dbConn);
                    command.CommandText = query5;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "purple", Label = "Cancelled", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }
                   
                    command.Dispose();
                    dataReader.Close();
                    dbConn.Close();
                    dbConn.Dispose();
                }
                return result;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
                throw;
            }
        }

      
        public List<TravelRequestDashboard> GetTravelReimbursementDashboardData()
        {
            try
            {
                var result = new List<TravelRequestDashboard>();

                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    string query1 = string.Format("Select Count(*) as count from REIMBURSE_TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.New);
                    OracleCommand command = new OracleCommand(query1, (OracleConnection)dbConn);
                    command.CommandText = query1;
                    DbDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "orange", Label = "New", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }

                    string query2 = string.Format("Select Count(*) as count from REIMBURSE_TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Pending);
                    command = new OracleCommand(query2, (OracleConnection)dbConn);
                    command.CommandText = query2;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "dodgeblue", Label = "Pending", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }


                    string query3 = string.Format("Select Count(*) as count from REIMBURSE_TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Rejected);
                    command = new OracleCommand(query3, (OracleConnection)dbConn);
                    command.CommandText = query3;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "red", Label = "Rejected", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }

                    string query4 = string.Format("Select Count(*) as count from REIMBURSE_TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Complete);
                    command = new OracleCommand(query4, (OracleConnection)dbConn);
                    command.CommandText = query4;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "green", Label = "Completed", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }

                    string query5 = string.Format("Select Count(*) as count from REIMBURSE_TRAVELREQUEST where STATUS= '{0}'", ApprovalStatus.Cancelled);
                    command = new OracleCommand(query5, (OracleConnection)dbConn);
                    command.CommandText = query5;
                    dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            result.Add(new TravelRequestDashboard() { Color = "purple", Label = "Cancelled", Value = Convert.ToInt32(dataReader["Count"]) });
                        }
                    }

                    command.Dispose();
                    dataReader.Close();
                    dbConn.Close();
                    dbConn.Dispose();
                }
                return result;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
                throw;
            }
        }


        #endregion
    }
}