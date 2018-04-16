using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.Class.Common;
using TravelApplication.Common;
using TravelApplication.DAL.DBProvider;
using TravelApplication.Models;
using TravelApplication.Services;

namespace TravelApplication.DAL.Repositories
{
    public class ReimbursementRepository : ApprovalRepository, IReimbursementRepository
    {
        private DbConnection dbConn;
        IEstimatedExpenseRepository estimatedExpenseRepository = new EstimatedExpenseRepository();
        IFISRepository fisRepository = new FISRepository();
        ITravelRequestRepository travelRequestRepository = new TravelRequestRepository();
        TravelRequestReportService travelRequestReportService = new TravelRequestReportService();
        public List<TravelRequestDetails> GetApprovedTravelRequestList(int submittedBadgeNumber, int selectedRoleId)
        {
             
            try
            {
                List<TravelRequestDetails> response = new List<TravelRequestDetails>();

                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    if (selectedRoleId == 1 || selectedRoleId == 2)
                    {

                        string query = string.Format("Select * from TRAVELREQUEST where BADGENUMBER= {0} AND STATUS = '{2}' AND TRAVELREQUESTID NOT IN (SELECT TRAVELREQUESTID FROM REIMBURSE_TRAVELREQUEST )order by CREATIONDATETIME desc", submittedBadgeNumber, selectedRoleId, ApprovalStatus.Complete);
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
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = EditActionEligible(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["PURPOSE"].ToString()
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
			                                                        BADGENUMBER = {0}
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
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = getApprovalSatus(dbConn, Convert.ToInt32(dataReader["TravelRequestId"]), submittedBadgeNumber) ? true : false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["Purpose"].ToString()
                                });
                            }
                        }
                        command.Dispose();
                        dataReader.Close();
                    }
                    if (selectedRoleId == 4)
                    {
                        string query = string.Format("Select * from TRAVELREQUEST where STATUS = '{2}' AND TRAVELREQUESTID NOT IN (SELECT TRAVELREQUESTID FROM REIMBURSE_TRAVELREQUEST )order by CREATIONDATETIME desc", submittedBadgeNumber, selectedRoleId, ApprovalStatus.Complete);
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
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString(),
                                    RequiredApprovers = GetApproversListByTravelRequestId(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApproveredByUser = getLastApproverName(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    LastApprovedDateTime = getLastApproverDateTime(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])),
                                    EditActionVisible = EditActionEligible(dbConn, Convert.ToInt32(dataReader["TravelRequestId"])) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = false,
                                    Status = dataReader["STATUS"].ToString(),
                                    Purpose = dataReader["PURPOSE"].ToString()
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
                throw new Exception(ex.Message);
            }
        }

        // Populate reimbursement page with travel details 
        public ReimbursementAllTravelInformation GetTravelRequestInfoForReimbursement(string travelRequestId)
        {
            ReimbursementTravelRequestDetails travelReimbursementDetails = null;
            EstimatedExpense estimatedExpense = null;
            FIS fisDetails = null;
            ReimbursementAllTravelInformation travelRequestReimbursementDetails = null;

            try
            {
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    travelReimbursementDetails = GetTravelReimbursementDetails(dbConn, travelRequestId);
                    estimatedExpense = estimatedExpenseRepository.GetTravelRequestDetailNew(dbConn, travelRequestId);
                    fisDetails = fisRepository.GetFISdetails(dbConn, travelRequestId);
                    dbConn.Close();
                    dbConn.Dispose();
                }
                travelRequestReimbursementDetails = new ReimbursementAllTravelInformation()
                {
                    TravelReimbursementDetails = travelReimbursementDetails,
                    Fis = fisDetails,
                    CashAdvance = estimatedExpense.CashAdvance,
                    PersonalTravelExpense = estimatedExpense.PersonalTravelExpense
                };
            }
            catch (Exception ex)
            {
                LogMessage.Log("Error while getting travel request details - " + ex.Message);
                throw new Exception(ex.Message);
            }

            return travelRequestReimbursementDetails;
        }

        // view existing records 
        public ReimbursementTravelRequestDetails GetTravelReimbursementDetails(DbConnection dbConn, string travelRequestId)
        {
            try
            {

                ReimbursementTravelRequestDetails response = null;
                string query = string.Format("Select * from TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        response = new ReimbursementTravelRequestDetails()
                        {
                            TravelRequestId = travelRequestId,
                            BadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]),
                            Name = dataReader["NAME"].ToString(),
                            Division = dataReader["DIVISION"].ToString(),
                            DepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]),
                            ReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"]),
                            StrDepartureDateTime = Convert.ToDateTime(dataReader["MEETINGBEGINDATETIME"]).ToShortDateString() ?? string.Empty,
                            StrReturnDateTime = Convert.ToDateTime(dataReader["MEETINGENDDATETIME"]).ToShortDateString() ?? string.Empty,
                            Purpose = dataReader["Purpose"].ToString()                              

                        };
                    }

                    var employeeDetails = travelRequestRepository.GetEmployeeDetails(response.BadgeNumber);
                    response.CostCenterId = (employeeDetails.Result.CostCenter  ?? string.Empty);
                    response.Department = (employeeDetails.Result.Department  ?? string.Empty);
                    response.Extension = (employeeDetails.Result.EmployeeWorkPhone  ?? string.Empty);
                    var vendorId = travelRequestRepository.GetVendorNumber(response.BadgeNumber).Result;
                    response.VendorNumber = (vendorId == "null") ? string.Empty : vendorId;
                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }

                string query1 = string.Format("Select APPROVEDAIRFARE,APPROVEDLODGE, APPROVEDMEALS, ACTUALLODGE, ACTUALMEALS from TRAVELREQUEST_ESTIMATEDEXPENSE where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command1 = new OracleCommand(query1, (OracleConnection)dbConn);
                command1.CommandText = query1;
                DbDataReader dataReader1 = command1.ExecuteReader();

                if (dataReader1.HasRows)
                {
                    while (dataReader1.Read())
                    {

                        response.TAEstimatedAirFare =  Convert.ToDecimal(dataReader1["APPROVEDAIRFARE"]);
                        response.TAEstimatedLodge = Convert.ToInt32(dataReader1["APPROVEDLODGE"]);
                        response.TAEstimatedMeals = Convert.ToInt32(dataReader1["APPROVEDMEALS"]);
                        response.TAActualLodge = Convert.ToInt32(dataReader1["ACTUALLODGE"]);
                        response.TAActualMeals = Convert.ToInt32(dataReader1["ACTUALMEALS"]);
                    }

                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }

                // Get TA estimated amount


                command1.Dispose();
                dataReader1.Close();
                return response;

            }
            catch (Exception ex)
            {
                LogMessage.Log("GetTravelRequestDetail : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public ReimbursementTravelRequestDetails GetTravelReimbursementDetails2(DbConnection dbConn, string travelRequestId)
        {
            try
            {

                ReimbursementTravelRequestDetails response = null;
                string query = string.Format("Select * from REIMBURSE_TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        response = new ReimbursementTravelRequestDetails()
                        {
                            TravelRequestId = travelRequestId,
                            BadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]),
                            Name = dataReader["NAME"].ToString(),
                            Division = dataReader["DIVISION"].ToString(),
                            DepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]),
                            ReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"]),
                            StrDepartureDateTime = Convert.ToDateTime(dataReader["DEPARTUREDATETIME"]).ToShortDateString() ?? string.Empty,
                            StrReturnDateTime = Convert.ToDateTime(dataReader["RETURNDATETIME"]).ToShortDateString() ?? string.Empty,
                            Purpose = dataReader["Purpose"].ToString(),
                            ReimbursementId = dataReader["REIMBURSEMENTID"].ToString(),
                            CostCenterId = dataReader["COSTCENTERID"].ToString(),
                            Department = dataReader["DEPARTMENT"].ToString(),
                            VendorNumber = dataReader["VENDORNUMBER"].ToString(),
                            Extension = dataReader["EXT"].ToString()

                        };
                    }

                    //var employeeDetails = travelRequestRepository.GetEmployeeDetails(response.BadgeNumber);
                    //response.CostCenterId = (employeeDetails.Result.CostCenter ?? string.Empty);
                    //response.Department = (employeeDetails.Result.Department ?? string.Empty);
                    //response.Extension = (employeeDetails.Result.EmployeeWorkPhone ?? string.Empty);
                    //var vendorId = travelRequestRepository.GetVendorNumber(response.BadgeNumber).Result;
                    //response.VendorNumber = (vendorId == "null") ? string.Empty : vendorId;
                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }

                string query1 = string.Format("Select APPROVEDAIRFARE,APPROVEDLODGE, APPROVEDLODGE, ACTUALLODGE, ACTUALMEALS from TRAVELREQUEST_ESTIMATEDEXPENSE where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command1 = new OracleCommand(query1, (OracleConnection)dbConn);
                command1.CommandText = query1;
                DbDataReader dataReader1 = command1.ExecuteReader();

                if (dataReader1.HasRows)
                {
                    while (dataReader1.Read())
                    {

                        response.TAEstimatedAirFare = Convert.ToDecimal(dataReader1["APPROVEDAIRFARE"]);
                        response.TAEstimatedLodge = Convert.ToInt32(dataReader1["APPROVEDLODGE"]);
                        response.TAEstimatedMeals = Convert.ToInt32(dataReader1["APPROVEDLODGE"]);
                        response.TAActualLodge = Convert.ToInt32(dataReader1["ACTUALLODGE"]);
                        response.TAActualMeals = Convert.ToInt32(dataReader1["ACTUALMEALS"]);
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
                LogMessage.Log("GetTravelRequestDetail2 : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public string SaveTravelRequestReimbursement(ReimbursementInput reimbursementRequest)
        {
            string reimbursementId = string.Empty;
             
           
            try
            {
                //Validate basic information


                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    // Insert or update travel request in  reimbursement 
                    reimbursementId = SaveReimbursementTravelRequestDetails(dbConn, reimbursementRequest.ReimbursementTravelRequestDetails);
                    if (string.IsNullOrEmpty(reimbursementId))
                    {
                        throw new Exception("Couldn't save/update travel request information");
                    }

                    // Insert or update estimated expense
                     SaveReimburse(dbConn, reimbursementRequest.ReimbursementDetails);

                    // Insert or update FIS expense
                     SaveFISData(dbConn, reimbursementRequest.FIS, reimbursementRequest.ReimbursementTravelRequestDetails.TravelRequestId);
                  

                    return reimbursementId;

                }
                dbConn.Close();
                dbConn.Dispose();
            }
            catch (Exception ex)
            {
                LogMessage.Log("SaveTravelRequestInput : " + ex.Message);
                throw new Exception("Couldn't save record into Travel Request - ");
            }
            return reimbursementId;
        }

        public void SaveReimburse(DbConnection dbConn, ReimbursementDetails reimbursementDetails)
        {
            try
            {
                        foreach (var reimbursement in reimbursementDetails.Reimbursement)
                    {
                        if (!CheckReimburseDataExists(dbConn, reimbursement.Id) && reimbursement.Date != DateTime.MinValue)
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = (OracleConnection)dbConn;
                            cmd.CommandText = string.Format(@"INSERT INTO REIMBURSE (                                                  
                                                                TRAVELREQUESTID,
                                                                RDATE ,
                                                                CITYSTATEANDBUSINESSPURPOSE ,
                                                                MILES ,
                                                                MILEAGETOWORK ,
                                                                BUSINESSMILES ,
                                                                BUSINESSMILESXRATE,
                                                                PARKINGANDGAS,
                                                                AIRFARE ,
                                                                TAXIRAIL ,
                                                                LODGE ,
                                                                MEALS ,
                                                                REGISTRATION ,
                                                                INTERNET,
                                                                OTHERS,
                                                                DAILYTOTAL ,
                                                                TOTALMILES ,
                                                                TOTALMILEAGETOWORK ,
                                                                TOTALBUSINESSMILES ,
                                                                TOTALBUSINESSMILESXRATE ,
                                                                TOTALPARKIMGGAS,
                                                                TOTALAIRFARE,
                                                                TOTALTAXIRAIL ,
                                                                TOTALLODGE ,
                                                                TOTALMEALS ,
                                                                TOTALREGISTRATION ,
                                                                TOTALINTERNET ,
                                                                TOTALOTHER,
                                                                TOTALDAILYTOTAL,
                                                                TOTALPART1TRAVELEXPENSES ,
                                                                TOTALPART2TRAVELEXPENSES ,
                                                                TOTALEXPSUBMITTEDFORAPPROVAL ,
                                                                SUBTRACTPAIDBYMTA ,
                                                                TOTALEXPENSES ,
                                                                SUBTRACTCASHADVANCE,
                                                                TOTAL,
                                                                SUBTRACTPERSONALTRAVELEXPENSE
                                                            )
                                                            VALUES
                                                                (:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13,:p14,:p15,:p16,:p17,:p18,:p19,:p20,:p21,:p22,:p23,:p24,:p25,:p26,:p27,:p28,:p29,:p30,:p31,:p32,:p33,:p34,:p35,:p36, :p37)");
                            cmd.Parameters.Add(new OracleParameter("p1", reimbursement.TravelRequestId));
                            cmd.Parameters.Add(new OracleParameter("p2", reimbursement.Date));
                            cmd.Parameters.Add(new OracleParameter("p3", reimbursement.CityStateAndBusinessPurpose));
                            cmd.Parameters.Add(new OracleParameter("p4", reimbursement.Miles));
                            cmd.Parameters.Add(new OracleParameter("p5", reimbursement.MileageToWork));
                            cmd.Parameters.Add(new OracleParameter("p6", reimbursement.BusinessMiles));
                            cmd.Parameters.Add(new OracleParameter("p7", reimbursement.BusinessMilesXRate));
                            cmd.Parameters.Add(new OracleParameter("p8", reimbursement.ParkingAndGas));
                            cmd.Parameters.Add(new OracleParameter("p9", reimbursement.AirFare));
                            cmd.Parameters.Add(new OracleParameter("p10", reimbursement.TaxiRail));
                            cmd.Parameters.Add(new OracleParameter("p11", reimbursement.Lodge));
                            cmd.Parameters.Add(new OracleParameter("p12", reimbursement.Meals));
                            cmd.Parameters.Add(new OracleParameter("p13", reimbursement.Registration));
                            cmd.Parameters.Add(new OracleParameter("p14", reimbursement.Internet));
                            cmd.Parameters.Add(new OracleParameter("p15", reimbursement.Others));
                            cmd.Parameters.Add(new OracleParameter("p16", reimbursement.DailyTotal));
                            cmd.Parameters.Add(new OracleParameter("p17", reimbursementDetails.TotalMiles));
                            cmd.Parameters.Add(new OracleParameter("p18", reimbursementDetails.TotalMileageToWork));
                            cmd.Parameters.Add(new OracleParameter("p19", reimbursementDetails.TotalBusinessMiles));
                            cmd.Parameters.Add(new OracleParameter("p20", reimbursementDetails.TotalBusinessMilesXRate));
                            cmd.Parameters.Add(new OracleParameter("p21", reimbursementDetails.TotalParkingGas));
                            cmd.Parameters.Add(new OracleParameter("p22", reimbursementDetails.TotalAirFare));
                            cmd.Parameters.Add(new OracleParameter("p23", reimbursementDetails.TotalTaxiRail));
                            cmd.Parameters.Add(new OracleParameter("p24", reimbursementDetails.TotalLodge));
                            cmd.Parameters.Add(new OracleParameter("p25", reimbursementDetails.TotalMeals));
                            cmd.Parameters.Add(new OracleParameter("p26", reimbursementDetails.TotalRegistration));
                            cmd.Parameters.Add(new OracleParameter("p27", reimbursementDetails.TotalInternet));
                            cmd.Parameters.Add(new OracleParameter("p28", reimbursementDetails.TotalOther));
                            cmd.Parameters.Add(new OracleParameter("p29", reimbursementDetails.TotalDailyTotal));
                            cmd.Parameters.Add(new OracleParameter("p30", reimbursementDetails.TotalPart1TravelExpenses));
                            cmd.Parameters.Add(new OracleParameter("p31", reimbursementDetails.TotalPart2TravelExpenses));
                            cmd.Parameters.Add(new OracleParameter("p32", reimbursementDetails.TotalExpSubmittedForApproval));
                            cmd.Parameters.Add(new OracleParameter("p33", reimbursementDetails.SubtractPaidByMTA));
                            cmd.Parameters.Add(new OracleParameter("p34", reimbursementDetails.TotalExpenses));
                            cmd.Parameters.Add(new OracleParameter("p35", reimbursementDetails.SubtractCashAdvance));
                            cmd.Parameters.Add(new OracleParameter("p36", reimbursementDetails.Total));
                            cmd.Parameters.Add(new OracleParameter("p37", reimbursementDetails.SubtractPersonalAdvance));

                        var rowsUpdated = cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                else
                {
                         OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE REIMBURSE SET                                                  
                                                        TRAVELREQUESTID = :p1,
                                                        RDATE= :p2,
                                                        CITYSTATEANDBUSINESSPURPOSE = :p3,
                                                        MILES = :p4,
                                                        MILEAGETOWORK = :p5,
                                                        BUSINESSMILES = :p6,
                                                        BUSINESSMILESXRATE= :p7,
                                                        PARKINGANDGAS= :p8,
                                                        AIRFARE= :p9,
                                                        TAXIRAIL = :p10,
                                                        LODGE = :p11,
                                                        MEALS = :p12,
                                                        REGISTRATION = :p13,
                                                        INTERNET= :p14,
                                                        OTHERS= :p15,
                                                        DAILYTOTAL = :p16,
                                                        TOTALMILES = :p17,
                                                        TOTALMILEAGETOWORK = :p18,
                                                        TOTALBUSINESSMILES = :p19,
                                                        TOTALBUSINESSMILESXRATE = :p20,
                                                        TOTALPARKIMGGAS= :p21,
                                                        TOTALAIRFARE= :p22,
                                                        TOTALTAXIRAIL = :p23,
                                                        TOTALLODGE = :p24,
                                                        TOTALMEALS = :p25,
                                                        TOTALREGISTRATION = :p26,
                                                        TOTALINTERNET= :p27,
                                                        TOTALOTHER= :p28,
                                                        TOTALDAILYTOTAL= :p29,
                                                        TOTALPART1TRAVELEXPENSES = :p30,
                                                        TOTALPART2TRAVELEXPENSES = :p31,
                                                        TOTALEXPSUBMITTEDFORAPPROVAL = :p32,
                                                        SUBTRACTPAIDBYMTA= :p33,
                                                        TOTALEXPENSES = :p34,
                                                        SUBTRACTCASHADVANCE= :p35,
                                                        TOTAL = :p36,
                                                        SUBTRACTPERSONALTRAVELEXPENSE = :p37
                                                        WHERE ID = {0}", reimbursement.Id);
                        cmd.Parameters.Add(new OracleParameter("p1", reimbursement.TravelRequestId));
                        cmd.Parameters.Add(new OracleParameter("p2", reimbursement.Date));
                        cmd.Parameters.Add(new OracleParameter("p3", reimbursement.CityStateAndBusinessPurpose));
                        cmd.Parameters.Add(new OracleParameter("p4", reimbursement.Miles));
                        cmd.Parameters.Add(new OracleParameter("p5", reimbursement.MileageToWork));
                        cmd.Parameters.Add(new OracleParameter("p6", reimbursement.BusinessMiles));
                        cmd.Parameters.Add(new OracleParameter("p7", reimbursement.BusinessMilesXRate));
                        cmd.Parameters.Add(new OracleParameter("p8", reimbursement.ParkingAndGas));
                        cmd.Parameters.Add(new OracleParameter("p9", reimbursement.AirFare));
                        cmd.Parameters.Add(new OracleParameter("p10", reimbursement.TaxiRail));
                        cmd.Parameters.Add(new OracleParameter("p11", reimbursement.Lodge));
                        cmd.Parameters.Add(new OracleParameter("p12", reimbursement.Meals));
                        cmd.Parameters.Add(new OracleParameter("p13", reimbursement.Registration));
                        cmd.Parameters.Add(new OracleParameter("p14", reimbursement.Internet));
                        cmd.Parameters.Add(new OracleParameter("p15", reimbursement.Others));
                        cmd.Parameters.Add(new OracleParameter("p16", reimbursement.DailyTotal));
                        cmd.Parameters.Add(new OracleParameter("p17", reimbursementDetails.TotalMiles));
                        cmd.Parameters.Add(new OracleParameter("p18", reimbursementDetails.TotalMileageToWork));
                        cmd.Parameters.Add(new OracleParameter("p19", reimbursementDetails.TotalBusinessMiles));
                        cmd.Parameters.Add(new OracleParameter("p20", reimbursementDetails.TotalBusinessMilesXRate));
                        cmd.Parameters.Add(new OracleParameter("p21", reimbursementDetails.TotalParkingGas));
                        cmd.Parameters.Add(new OracleParameter("p22", reimbursementDetails.TotalAirFare));
                        cmd.Parameters.Add(new OracleParameter("p23", reimbursementDetails.TotalTaxiRail));
                        cmd.Parameters.Add(new OracleParameter("p24", reimbursementDetails.TotalLodge));
                        cmd.Parameters.Add(new OracleParameter("p25", reimbursementDetails.TotalMeals));
                        cmd.Parameters.Add(new OracleParameter("p26", reimbursementDetails.TotalRegistration));
                        cmd.Parameters.Add(new OracleParameter("p27", reimbursementDetails.TotalInternet));
                        cmd.Parameters.Add(new OracleParameter("p28", reimbursementDetails.TotalOther));
                        cmd.Parameters.Add(new OracleParameter("p29", reimbursementDetails.TotalDailyTotal));
                        cmd.Parameters.Add(new OracleParameter("p30", reimbursementDetails.TotalPart1TravelExpenses));
                        cmd.Parameters.Add(new OracleParameter("p31", reimbursementDetails.TotalPart2TravelExpenses));
                        cmd.Parameters.Add(new OracleParameter("p32", reimbursementDetails.TotalExpSubmittedForApproval));
                        cmd.Parameters.Add(new OracleParameter("p33", reimbursementDetails.SubtractPaidByMTA));
                        cmd.Parameters.Add(new OracleParameter("p34", reimbursementDetails.TotalExpenses));
                        cmd.Parameters.Add(new OracleParameter("p35", reimbursementDetails.SubtractCashAdvance));
                        cmd.Parameters.Add(new OracleParameter("p36", reimbursementDetails.Total));
                        cmd.Parameters.Add(new OracleParameter("p37", reimbursementDetails.SubtractPersonalAdvance));

                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    }
            }
            catch (Exception ex)
            {

                LogMessage.Log("Save Reimburse : " + ex.Message);
                throw new Exception("Couldn't save record into Reimburse ");
            }
        }

        private bool CheckReimburseDataExists(DbConnection dbConn, int id)
        {
            bool result = false;
            string query = string.Format(@"Select * from REIMBURSE where ID = {0}", id);
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

        private string SaveReimbursementTravelRequestDetails(DbConnection dbConn, ReimbursementTravelRequestDetails reimbursementDetails)
        {
            string reimbursementId = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(reimbursementDetails.ReimbursementId))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = (OracleConnection)dbConn;
                    cmd.CommandText = @"INSERT INTO REIMBURSE_TRAVELREQUEST(
                                            TRAVELREQUESTID,
                                            BADGENUMBER,
                                            NAME,
                                            EXT,
                                            DIVISION,
                                            DEPARTMENT,
                                            DEPARTUREDATETIME,
                                            RETURNDATETIME,
                                            CREATIONDATETIME, 
                                            SELECTEDROLEID,
                                            SUBMITTEDBYBADGENUMBER,
                                            SUBMITTEDDATETIME,
                                            STATUS
                                            )
                                            VALUES
                                            (:p1,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13) returning REIMBURSEMENTID into :reimbursementId";
                    cmd.Parameters.Add(new OracleParameter("p1", reimbursementDetails.TravelRequestId));
                    cmd.Parameters.Add(new OracleParameter("p2", reimbursementDetails.BadgeNumber));
                    cmd.Parameters.Add(new OracleParameter("p3", reimbursementDetails.Name));
                    cmd.Parameters.Add(new OracleParameter("p4", reimbursementDetails.Extension));
                    cmd.Parameters.Add(new OracleParameter("p5", reimbursementDetails.Division));
                    cmd.Parameters.Add(new OracleParameter("p6", reimbursementDetails.Department));
                    cmd.Parameters.Add(new OracleParameter("p7", reimbursementDetails.DepartureDateTime));
                    cmd.Parameters.Add(new OracleParameter("p8", reimbursementDetails.ReturnDateTime));
                    cmd.Parameters.Add(new OracleParameter("p9", DateTime.Now));
                    cmd.Parameters.Add(new OracleParameter("p10", reimbursementDetails.SelectedRoleId));
                    cmd.Parameters.Add(new OracleParameter("p11", reimbursementDetails.SubmittedByBadgeNumber));
                    cmd.Parameters.Add(new OracleParameter("p12", DateTime.Now));
                    cmd.Parameters.Add(new OracleParameter("p13", ApprovalStatus.New.ToString()));
                    cmd.Parameters.Add("reimbursementId", OracleDbType.Int32, ParameterDirection.ReturnValue);
                    var rowsUpdated = cmd.ExecuteNonQuery();
                    reimbursementId = cmd.Parameters["reimbursementId"].Value.ToString();
                    cmd.Dispose();
                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = (OracleConnection)dbConn;
                    cmd.CommandText = string.Format(@"UPDATE  REIMBURSE_TRAVELREQUEST SET 
                                                        TRAVELREQUESTID = :p1,
                    BADGENUMBER = :p2,
                    NAME =:p3,
                    EXT =:p4,
                    DIVISION =:p5,
                    DEPARTMENT = :p6,
                    DEPARTUREDATETIME = :p7,
                    RETURNDATETIME = :p8,
                    LASTUPDATEDDATETIME = :p9
 WHERE REIMBURSEMENTID = {0}", reimbursementDetails.ReimbursementId);

                    cmd.Parameters.Add(new OracleParameter("p1", reimbursementDetails.TravelRequestId));
                    cmd.Parameters.Add(new OracleParameter("p2", reimbursementDetails.BadgeNumber));
                    cmd.Parameters.Add(new OracleParameter("p3", reimbursementDetails.Name));
                    cmd.Parameters.Add(new OracleParameter("p4", reimbursementDetails.Extension));
                    cmd.Parameters.Add(new OracleParameter("p5", reimbursementDetails.Division));
                    cmd.Parameters.Add(new OracleParameter("p6", reimbursementDetails.Department));
                    cmd.Parameters.Add(new OracleParameter("p7", reimbursementDetails.DepartureDateTime));
                    cmd.Parameters.Add(new OracleParameter("p8", reimbursementDetails.ReturnDateTime));
                    cmd.Parameters.Add(new OracleParameter("p9", DateTime.Now));
                    var rowsUpdated = cmd.ExecuteNonQuery();
                    reimbursementId = reimbursementDetails.ReimbursementId;
                    cmd.Dispose();

                }
            }
            catch (Exception ex)
            {

                LogMessage.Log("Save Travel request for reimbursement  : " + ex.Message);
                throw new Exception("Couldn't insert/update record into Travel Request for Reimburse - ");
            }

            return reimbursementId;
             
        }


        private string GetApproversListByTravelRequestId(DbConnection dbConn, int travelRequestId)
        {
            try
            {

            string response = string.Empty;
            string query = string.Format("select APPROVERNAME from TRAVELREQUEST_APPROVAL where TRAVELREQUESTID = {0} order by ApprovalOrder", travelRequestId);
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
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : GetApproversListByTravelRequestId : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private string getLastApproverName(DbConnection dbConn, int travelRequestId)
        {
            try
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
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : getLastApproverName : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private string getLastApproverDateTime(DbConnection dbConn, int travelRequestId)
        {
            try
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
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : getLastApproverDateTime : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool getApprovalSatus(DbConnection dbConn, int travelRequestId, int approverBadgeNumber)
        {
            try
            {

           
            bool result = false;
            int response = 0;
            string query = string.Format(@"SELECT
	                                            *
                                            FROM
	                                            (
		                                            SELECT
			                                            BadgeNumber
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
                }
            }
            command.Dispose();
            dataReader.Close();

            if (response == approverBadgeNumber)
            {
                result = true;
            }

            return result;
            }
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : getApprovalSatus : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public bool getReimburseApprovalSatus(DbConnection dbConn, int travelRequestId, int approverBadgeNumber)
        {
            try
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
			                                            REIMBURSE_APPROVAL
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

            if (response == approverBadgeNumber)
            {
                result = true;
            }

            return result;
            }
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : getReimburseApprovalSatus : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public bool EditActionEligible(DbConnection dbConn, int travelRequestId)
        {
            try
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

            if (response == ApprovalStatus.New.ToString())
            {
                return true;
            }
            return false;
            }
            catch (Exception ex)
            {
                LogMessage.Log("ReimbursementRepository : EditActionEligible : " + ex.Message);
                throw new Exception(ex.Message);
            }

        }

        public void SaveFISData(DbConnection dbConn, FIS request, string travelRequestId)
        {
            try
            {
                OracleCommand cmd1 = new OracleCommand();
                cmd1.Connection = (OracleConnection)dbConn;
                cmd1.CommandText = string.Format(@" DELETE FROM REIMBURSE_FIS WHERE TRAVELREQUESTID = {0}", travelRequestId);
                cmd1.ExecuteNonQuery();

                foreach (var fis in request.FISDetails)
                {
                    if (!string.IsNullOrEmpty(fis.CostCenterId) && fis.CostCenterId != "?")
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"INSERT INTO REIMBURSE_FIS (                                                  
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
            //    if (!CheckFISDataExistsInReimburse(dbConn, travelRequestId))
            //    {
            //        foreach (var fis in request.FISDetails)
            //        {
            //            OracleCommand cmd = new OracleCommand();
            //            cmd.Connection = (OracleConnection)dbConn;
            //            cmd.CommandText = string.Format(@"INSERT INTO REIMBURSE_FIS (                                                  
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
            //            cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
            //            cmd.Parameters.Add(new OracleParameter("p2", fis.CostCenterId));
            //            cmd.Parameters.Add(new OracleParameter("p3", fis.LineItem));
            //            cmd.Parameters.Add(new OracleParameter("p4", fis.ProjectId));
            //            cmd.Parameters.Add(new OracleParameter("p5", fis.Task));
            //            cmd.Parameters.Add(new OracleParameter("p6", fis.Amount));
            //            cmd.Parameters.Add(new OracleParameter("p7", request.TotalAmount));
            //            var rowsUpdated = cmd.ExecuteNonQuery();
            //            cmd.Dispose();
            //        }
            //    }
            //    else
            //    {

            //        foreach (var fis in request.FISDetails)
            //        {
            //            OracleCommand cmd = new OracleCommand();
            //            cmd.Connection = (OracleConnection)dbConn;
            //            cmd.CommandText = string.Format(@"UPDATE  REIMBURSE_FIS SET                                                  
            //                                    TRAVELREQUESTID = :p1,
            //                                            COSTCENTERID =:p2,
            //                                            LINEITEM =:p3,
            //                                            PROJECTID =:p4,
            //                                            TASK =:p5,
            //                                            AMOUNT =:p6,
            //                                            TOTALAMOUNT =:p7
            //                                    WHERE TRAVELREQUESTID = {0}", travelRequestId);
            //            cmd.Parameters.Add(new OracleParameter("p1", travelRequestId));
            //            cmd.Parameters.Add(new OracleParameter("p2", fis.CostCenterId));
            //            cmd.Parameters.Add(new OracleParameter("p3", fis.LineItem));
            //            cmd.Parameters.Add(new OracleParameter("p4", fis.ProjectId));
            //            cmd.Parameters.Add(new OracleParameter("p5", fis.Task));
            //            cmd.Parameters.Add(new OracleParameter("p6", fis.Amount));
            //            cmd.Parameters.Add(new OracleParameter("p7", request.TotalAmount));
            //            var rowsUpdated = cmd.ExecuteNonQuery();
            //            cmd.Dispose();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogMessage.Log("SaveFISData : " + ex.Message);
            //    throw new Exception("Couldn't insert/update record into Travel Request ");
            //}


        }

        public bool CheckFISDataExistsInReimburse(DbConnection dbConn, string travelRequestId)
        {
            bool result = false;
            string query = string.Format(@"Select * from REIMBURSE_FIS where travelRequestId = {0}", travelRequestId);
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

        public List<ReimburseGridDetails> GetReimbursementRequestsList(int badgeNumber, int selectedRoleId)
        {
            try
            {
                List<ReimburseGridDetails> response = new List<ReimburseGridDetails>();

                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    if (selectedRoleId == 1 || selectedRoleId == 2)
                    {

                        string query = string.Format("Select * from REIMBURSE_TRAVELREQUEST where BADGENUMBER= {0}   order by CREATIONDATETIME desc", badgeNumber, selectedRoleId);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new ReimburseGridDetails()
                                {
                                    TravelRequestId = dataReader["TravelRequestId"].ToString(),
                                    BadgeNumber = Convert.ToInt32(dataReader["BadgeNumber"].ToString()),
                                    Purpose = "", //dataReader["PURPOSE"].ToString(),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = Convert.ToDateTime(dataReader["SUBMITTEDDATETIME"]),
                                    RequiredApprovers = GetReimburseApproversListByTravelRequestId(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedByUser = getReimburseLastApproverName(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedDateTime = getReimburseLastApproverDateTime(dbConn, dataReader["TravelRequestId"].ToString()),
                                    EditActionVisible = ReimburseEditActionEligible(dbConn, dataReader["TravelRequestId"].ToString()) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = false,
                                    Status = dataReader["STATUS"].ToString(),
                                    StrSubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString() ?? string.Empty,
                                    ReimbursementId = Convert.ToInt32(dataReader["REIMBURSEMENTID"])
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
                                                         REIMBURSE_TRAVELREQUEST
                                                        WHERE
                                                         TRAVELREQUESTID IN (
                                                          SELECT
                                                           TRAVELREQUESTId
                                                          FROM
                                                           REIMBURSE_APPROVAL
                                                          WHERE
                                                           BADGENUMBER = {0} OR APPROVEROTHERBADGENUMBER = {0}
                                                         )   order by CREATIONDATETIME desc", badgeNumber);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new ReimburseGridDetails()
                                {
                                    TravelRequestId = dataReader["TravelRequestId"].ToString(),
                                    BadgeNumber = Convert.ToInt32(dataReader["BadgeNumber"].ToString()),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = Convert.ToDateTime(dataReader["SUBMITTEDDATETIME"]),
                                    RequiredApprovers = GetReimburseApproversListByTravelRequestId(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedByUser = getReimburseLastApproverName(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedDateTime = getReimburseLastApproverDateTime(dbConn, dataReader["TravelRequestId"].ToString()),                                    
                                    EditActionVisible = false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = getReimburseApprovalSatus(dbConn, Convert.ToInt32(dataReader["TravelRequestId"]), badgeNumber) ? true : false,
                                    Status = dataReader["STATUS"].ToString(),                                   
                                });
                            }
                        }
                        command.Dispose();
                        dataReader.Close();
                    }

                    if (selectedRoleId == 4)
                    {

                        string query = string.Format("Select * from REIMBURSE_TRAVELREQUEST  order by CREATIONDATETIME desc", badgeNumber, selectedRoleId);
                        OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                        command.CommandText = query;
                        DbDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                response.Add(new ReimburseGridDetails()
                                {
                                    TravelRequestId = dataReader["TravelRequestId"].ToString(),
                                    BadgeNumber = Convert.ToInt32(dataReader["BadgeNumber"].ToString()),
                                    Purpose = "", //dataReader["PURPOSE"].ToString(),
                                    SubmittedByUser = dataReader["SUBMITTEDBYUSERNAME"].ToString(),
                                    SubmittedDateTime = Convert.ToDateTime(dataReader["SUBMITTEDDATETIME"]),
                                    RequiredApprovers = GetReimburseApproversListByTravelRequestId(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedByUser = getReimburseLastApproverName(dbConn, dataReader["TravelRequestId"].ToString()),
                                    LastApprovedDateTime = getReimburseLastApproverDateTime(dbConn, dataReader["TravelRequestId"].ToString()),
                                    EditActionVisible = true , //ReimburseEditActionEligible(dbConn, dataReader["TravelRequestId"].ToString()) ? true : false,
                                    ViewActionVisible = true,
                                    ApproveActionVisible = getReimburseApprovalSatus(dbConn, Convert.ToInt32(dataReader["TravelRequestId"]), badgeNumber) ? true : false,
                                    Status = dataReader["STATUS"].ToString(),
                                    StrSubmittedDateTime = dataReader["SUBMITTEDDATETIME"].ToString() ?? string.Empty,
                                    ReimbursementId = Convert.ToInt32(dataReader["REIMBURSEMENTID"])
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
                LogMessage.Log("GetReimburseGridDetails : " + ex.Message);
                throw;
            }
            
        }

        private string GetReimburseApproversListByTravelRequestId(DbConnection dbConn, string travelRequestId)
        {
            string response = string.Empty;
            string query = string.Format("select APPROVERNAME from REIMBURSE_APPROVAL where TRAVELREQUESTID = {0} order by ApprovalOrder", travelRequestId);
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
        private string getReimburseLastApproverName(DbConnection dbConn, string travelRequestId)
        {
            string response = "";
            string query = string.Format(@"select APPROVERNAME from (
                                                                    SELECT
	                                                                    APPROVERNAME
                                                                    FROM
	                                                                    REIMBURSE_APPROVAL
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
        private string getReimburseLastApproverDateTime(DbConnection dbConn, string travelRequestId)
        {
            string response = "";
            string query = string.Format(@"Select ApprovalDateTime from (
                                            SELECT
	                                        APPROVALDATETIME
                                        FROM
	                                        REIMBURSE_APPROVAL
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

        public bool ReimburseEditActionEligible(DbConnection dbConn, string travelRequestId)
        {
            string response = "";
            string query = string.Format(@"SELECT
	                                        STATUS
                                        FROM
	                                        REIMBURSE_TRAVELREQUEST 
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

            if (response == ApprovalStatus.New.ToString() || response == ApprovalStatus.Rejected.ToString())
            {
                return true;
            }
            return false;

        }

        //public bool SubmitTravelRequest(SubmitReimburseData submitReimburseData)
        //{
        //    List<BadgeInfo> approvalOrderList = new List<BadgeInfo>();
        //    approvalOrderList.Add(new BadgeInfo() { BadgeId = submitReimburseData.DepartmentHeadBadgeNumber, Name = submitReimburseData.DepartmentHeadName });
        //    approvalOrderList.Add(new BadgeInfo() { BadgeId = submitReimburseData.ExecutiveOfficerBadgeNumber, Name = submitReimburseData.ExecutiveOfficerName });
        //    approvalOrderList.Add(new BadgeInfo() { BadgeId = submitReimburseData.CEOForAPTABadgeNumber, Name = submitReimburseData.CEOForAPTAName });
        //    approvalOrderList.Add(new BadgeInfo() { BadgeId = submitReimburseData.CEOForInternationalBadgeNumber, Name = submitReimburseData.CEOForInternationalName });
        //    approvalOrderList.Add(new BadgeInfo() { BadgeId = submitReimburseData.TravelCoordinatorBadgeNumber, Name = submitReimburseData.TravelCoordinatorName });

        //    try
        //    {

        //        dbConn = ConnectionFactory.GetOpenDefaultConnection();
        //        int count = 1;
        //        foreach (var item in approvalOrderList)
        //        {
        //            if (!string.IsNullOrEmpty(item.BadgeId))
        //            {
        //                // submit to approval 
        //                OracleCommand cmd = new OracleCommand();
        //                cmd.Connection = (OracleConnection)dbConn;
        //                cmd.CommandText = @"INSERT INTO REIMBURSE_APPROVAL (                                                  
        //                                                    TRAVELREQUESTID,
        //                                                    BADGENUMBER,
        //                                                    APPROVERNAME,
        //                                                    APPROVALSTATUS,
        //                                                    APPROVALORDER
        //                                                )
        //                                                VALUES
        //                                                    (:p1,:p2,:p3,:p4,:p5)";
        //                cmd.Parameters.Add(new OracleParameter("p1", submitReimburseData.TravelRequestId));
        //                cmd.Parameters.Add(new OracleParameter("p2", item.BadgeId));
        //                cmd.Parameters.Add(new OracleParameter("p3", item.Name));
        //                cmd.Parameters.Add(new OracleParameter("p4", Common.ApprovalStatus.Pending.ToString()));
        //                cmd.Parameters.Add(new OracleParameter("p5", count));
        //                var rowsUpdated = cmd.ExecuteNonQuery();
        //                cmd.Dispose();
        //                count++;
        //            }
        //        }
        //        OracleCommand cmd1 = new OracleCommand();
        //        cmd1.Connection = (OracleConnection)dbConn;
        //        cmd1.CommandText = string.Format(@"UPDATE REIMBURSE_TRAVELREQUEST SET                                                 
        //                                               SUBMITTEDBYUSERNAME = :p1 ,
        //                                                SUBMITTEDDATETIME = :p2,
        //                                                STATUS = :p3,
        //                                                AGREE = :p4
        //                                           WHERE TRAVELREQUESTID = {0}", submitReimburseData.TravelRequestId);

        //        cmd1.Parameters.Add(new OracleParameter("p1", submitReimburseData.SubmittedByUserName));
        //        cmd1.Parameters.Add(new OracleParameter("p2", DateTime.Now));
        //        cmd1.Parameters.Add(new OracleParameter("p3", Common.ApprovalStatus.Pending.ToString()));
        //        cmd1.Parameters.Add(new OracleParameter("p4", (submitReimburseData.AgreedToTermsAndConditions) ? "Y" : "N"));
        //        var rowsUpdated1 = cmd1.ExecuteNonQuery();
        //        cmd1.Dispose();
        //        dbConn.Close();
        //        dbConn.Dispose();


        //        string link = string.Format("<a href=\"http://localhost:2462/\">here</a>");
        //        string subject = string.Format(@"Reimburse Request Approval for Id - {0} ", submitReimburseData.TravelRequestId);
        //        string body = string.Format(@"Please visit Travel application website " + link + " to Approve/Reject for travel request Id : {0}", submitReimburseData.TravelRequestId);
        //        sendEmail(submitReimburseData.DepartmentHeadBadgeNumber, body, subject);
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.Message);
        //    }

        //}

        public ReimbursementInput GetAllReimbursementDetails(string travelRequestId)
        {
            ReimbursementInput response = new ReimbursementInput();
            try
            {
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    
                    response.ReimbursementTravelRequestDetails = GetTravelReimbursementDetails2(dbConn, travelRequestId);
                    response.ReimbursementDetails =  GetReimbursementDetails(dbConn, travelRequestId);
                    response.FIS = fisRepository.GetFISdetailsForReimburse(dbConn, travelRequestId);

                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("Reimbursement repository : GetAllReimbursementDetails "+ ex.Message);
                throw new Exception(ex.Message);
            }
           

            return response;
        }

        private ReimbursementDetails GetReimbursementDetails(DbConnection dbconn, string travelRequestId)
        {
            try
            {
                ReimbursementDetails finalResponse = null;
                List<Reimbursement> response =  new List<Reimbursement>();
                string query = string.Format("Select * from Reimburse where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        response.Add(new Reimbursement()
                        {
                            TravelRequestId = travelRequestId,
                            Date = Convert.ToDateTime(dataReader["RDate"]),
                            CityStateAndBusinessPurpose = dataReader["CITYSTATEANDBUSINESSPURPOSE"].ToString(),
                            Miles = Convert.ToInt32(dataReader["MILES"]),
                            MileageToWork = Convert.ToInt32(dataReader["MILEAGETOWORK"]),
                            BusinessMiles = Convert.ToInt32(dataReader["BUSINESSMILES"]),
                            BusinessMilesXRate = Convert.ToInt32(dataReader["BUSINESSMILESXRATE"]),
                            ParkingAndGas = Convert.ToInt32(dataReader["PARKINGANDGAS"]),
                            AirFare = Convert.ToInt32(dataReader["AIRFARE"]),
                            TaxiRail = Convert.ToInt32(dataReader["TAXIRAIL"]),
                            Lodge = Convert.ToInt32(dataReader["LODGE"]),
                            Meals = Convert.ToInt32(dataReader["MEALS"]),
                            Registration = Convert.ToInt32(dataReader["REGISTRATION"]),
                            Internet = Convert.ToInt32(dataReader["INTERNET"]),
                            Others = Convert.ToInt32(dataReader["OTHERS"]),
                            DailyTotal = Convert.ToInt32(dataReader["DAILYTOTAL"]),
                            DtReimburse = Convert.ToDateTime(dataReader["RDate"]).ToShortDateString(),
                            Id = Convert.ToInt32(dataReader["ID"])
                        });
                        finalResponse = new ReimbursementDetails()
                        {
                            Reimbursement = response,
                            SubtractCashAdvance = Convert.ToInt32(dataReader["SUBTRACTCASHADVANCE"]),
                            SubtractPersonalAdvance = (string.IsNullOrEmpty(dataReader["SUBTRACTPERSONALTRAVELEXPENSE"].ToString())) ? 0 : Convert.ToDecimal(dataReader["SUBTRACTPERSONALTRAVELEXPENSE"]),
                            SubtractPaidByMTA = Convert.ToInt32(dataReader["SUBTRACTPAIDBYMTA"]),
                            Total = Convert.ToInt32(dataReader["TOTAL"]),
                            TotalAirFare = Convert.ToInt32(dataReader["TOTALAIRFARE"]),
                            TotalBusinessMiles = Convert.ToInt32(dataReader["TOTALBUSINESSMILES"]),
                            TotalBusinessMilesXRate = Convert.ToInt32(dataReader["TOTALBUSINESSMILESXRATE"]),
                            TotalDailyTotal = Convert.ToInt32(dataReader["TOTALDAILYTOTAL"]),
                            TotalExpenses = Convert.ToInt32(dataReader["TOTALEXPENSES"]),
                            TotalExpSubmittedForApproval = Convert.ToInt32(dataReader["TOTALEXPSUBMITTEDFORAPPROVAL"]),
                            TotalInternet = Convert.ToInt32(dataReader["TOTALINTERNET"]),
                            TotalLodge = Convert.ToInt32(dataReader["TOTALLODGE"]),
                            TotalMeals = Convert.ToInt32(dataReader["TOTALMEALS"]),
                            TotalMileageToWork = Convert.ToInt32(dataReader["TOTALMILEAGETOWORK"]),
                            TotalMiles = Convert.ToInt32(dataReader["TOTALMILES"]),
                            TotalOther = Convert.ToInt32(dataReader["TOTALOTHER"]),
                            TotalParkingGas = Convert.ToInt32(dataReader["TOTALPARKIMGGAS"]),
                            TotalPart1TravelExpenses = Convert.ToInt32(dataReader["TOTALPART1TRAVELEXPENSES"]),
                            TotalPart2TravelExpenses = Convert.ToInt32(dataReader["TOTALPART2TRAVELEXPENSES"]),
                            TotalRegistration = Convert.ToInt32(dataReader["TOTALREGISTRATION"]),
                            TotalTaxiRail = Convert.ToInt32(dataReader["TOTALTAXIRAIL"]),

                        };
                    }
                     
 
                }
                //else
                //{
                //    throw new Exception("Couldn't retrieve reimbursement details");
                //}
                command.Dispose();
                dataReader.Close();
                return finalResponse;

            }
            catch (Exception ex)
            {
                LogMessage.Log("Reimbursement repository : GetTravelRequestDetail : " + ex.Message);
                throw;
            }
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
                    string query2 = string.Format("Select * from  REIMBURSE_APPROVAL WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1} OR APPROVEROTHERBADGENUMBER ={1} ", travelRequestId, approverBadgeNumber);
                    OracleCommand command2 = new OracleCommand(query2, (OracleConnection)dbConn);
                    command2.CommandText = query2;
                    DbDataReader dataReader2 = command2.ExecuteReader();

                    if (dataReader2.HasRows)
                    {
                        //Update travel request _approval
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  REIMBURSE_APPROVAL SET                                                  
                                                            APPROVERCOMMENTS = :p1,
                                                            APPROVALSTATUS = :p2 ,
                                                            APPROVALDATETIME = :p3
                                                            WHERE TRAVELREQUESTID = {0} AND BADGENUMBER = {1} OR APPROVEROTHERBADGENUMBER = {1} ", travelRequestId, approverBadgeNumber);
                        cmd.Parameters.Add(new OracleParameter("p1", comments));
                        cmd.Parameters.Add(new OracleParameter("p2", ApprovalStatus.Approved.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p3", DateTime.Now));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        // Get the approval badgeNumber 
                        var result = 0;
                        string query = string.Format(@"SELECT
	                                                        BADGENUMBER, APPROVEROTHERBADGENUMBER
                                                        FROM
	                                                        (
		                                                        SELECT
			                                                        BADGENUMBER,APPROVEROTHERBADGENUMBER
		                                                        FROM
			                                                        REIMBURSE_APPROVAL
		                                                        WHERE
			                                                        TRAVELREQUESTID = {0}
		                                                        AND APPROVALDATETIME IS NULL
		                                                        ORDER BY
			                                                        APPROVALORDER 
	                                                        )
                                                        WHERE
	                                                        ROWNUM = 1", travelRequestId);
                        OracleCommand cmd1 = new OracleCommand(query, (OracleConnection)dbConn);
                        cmd1.CommandText = query;
                        DbDataReader dataReader = cmd1.ExecuteReader();

                        // update travel request for the latest status 
                        cmd1.CommandText = string.Format(@"UPDATE  REIMBURSE_TRAVELREQUEST SET                                                  
                                                             STATUS = :p1 
                                                            WHERE TRAVELREQUESTID = {0}", travelRequestId);
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
                            if (result != null || result != 0)
                            {

                                cmd1.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Pending.ToString()));
                                var rowsUpdated1 = cmd1.ExecuteNonQuery();

                                //Send Email for next approver
                                string subject = string.Format(@"Travel Request Reimbursement Approval for Id - {0} ", travelRequestId);

                                var dateTime = System.DateTime.Now.Ticks;
                                //Generate Crystal report
                                travelRequestReportService.RunReport("Travel_Business_Expense.rpt", "ReimbursementRequest_" + travelRequestId+"_"+dateTime, travelRequestId.ToString());

                                sendEmail(result, subject,travelRequestId, "Form2", "ReimbursementRequest_" + travelRequestId+"_"+dateTime);
                            }
                        }
                        else
                        {
                            cmd1.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Complete.ToString()));
                            var rowsUpdated1 = cmd1.ExecuteNonQuery();
                        }

                        cmd1.Dispose();
                        dataReader.Close();
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
                LogMessage.Log("Reimbursement : Approve :" + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public bool Reject(int  approveBadgeNumber, int travelRequestBadgeNumber, string travelRequestId, string comments , string rejectReason)
        {
            try
            {
                bool response = false;
                int approvalOrder = 0;
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    // check if its already approved by website or Email 
                    string query2 = string.Format("Select * from  REIMBURSE_APPROVAL WHERE TRAVELREQUESTID = {0} AND APPROVALDATETIME IS Null AND BADGENUMBER = {1}  ", travelRequestId, approveBadgeNumber);
                    OracleCommand command2 = new OracleCommand(query2, (OracleConnection)dbConn);
                    command2.CommandText = query2;
                    DbDataReader dataReader2 = command2.ExecuteReader();

                    if (dataReader2.HasRows)
                    {
                        //Update travel request _approval
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = (OracleConnection)dbConn;
                        cmd.CommandText = string.Format(@"UPDATE  REIMBURSE_APPROVAL SET                                                  
                                                            APPROVERCOMMENTS = :p1,
                                                            APPROVALSTATUS = :p2 ,
                                                            APPROVALDATETIME = :p3,
                                                            REJECTREASON = :p4
                                                            WHERE TRAVELREQUESTID = {0} AND BADGENUMBER = {1} ", travelRequestId, approveBadgeNumber);
                        cmd.Parameters.Add(new OracleParameter("p1", comments));
                        cmd.Parameters.Add(new OracleParameter("p2", ApprovalStatus.Rejected.ToString()));
                        cmd.Parameters.Add(new OracleParameter("p3", DateTime.Now));
                        cmd.Parameters.Add(new OracleParameter("p4", rejectReason));
                        var rowsUpdated = cmd.ExecuteNonQuery();
                        cmd.Dispose();

                        var rejectReiumburseRequest = string.Empty;
                        if (approveBadgeNumber == 85163)
                        {
                            rejectReiumburseRequest = "true";
                        }
                        //Update travel request _approval
                        OracleCommand cmd1 = new OracleCommand();
                        cmd1.Connection = (OracleConnection)dbConn;
                        // update travel request for the latest status 
                        cmd1.CommandText = string.Format(@"UPDATE  REIMBURSE_TRAVELREQUEST SET                                                  
                                                             STATUS = :p1,
                                                             REJECTREIMBURSEREQUEST = :p2 
                                                            WHERE TRAVELREQUESTID = {0}", travelRequestId);

                        cmd1.Parameters.Add(new OracleParameter("p1", ApprovalStatus.Rejected.ToString()));
                        cmd1.Parameters.Add(new OracleParameter("p2", rejectReiumburseRequest));
                        var rowsUpdated1 = cmd1.ExecuteNonQuery();

                        cmd1.Dispose();
                        dbConn.Close();
                        dbConn.Dispose();

                        //Send Email submitter and traveller 
                        //string link = string.Format("<a href=\"http://localhost:2462/\">here</a>");
                        //string subject = string.Format(@"Travel Request Approval for Id - {0} ", travelRequestId);
                        //string body = string.Format(@"Please visit Travel application website " + link + " to Approve/Reject for travel request Id : {0}", travelRequestId);
                        //sendEmail(result.ToString(), body, subject);
                        string subject = string.Format(@"Reimbursement Rejection  for Id - {0} ", travelRequestId);
                        sendRejectionEmail(travelRequestBadgeNumber, subject, travelRequestId, comments, rejectReason);
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
                LogMessage.Log("Reimbursement : Reject : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId)
        {
            TravelRequestSubmitDetailResponse response = null;
            TravelRequestSubmitDetail travelRequestSubmitDetail = null;
            try
            {
                using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
                {
                    string query = string.Format("select TRAVELREQUESTID,BADGENUMBER,APPROVALORDER,APPROVEROTHERBADGENUMBER, APPROVERNAME from REIMBURSE_APPROVAL where TRAVELREQUESTID='{0}' and APPROVALSTATUS = '{1}'", travelRequestId, ApprovalStatus.Pending);

                    OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                    command.CommandText = query;
                    DbDataReader dataReader = command.ExecuteReader();
                    travelRequestSubmitDetail = new TravelRequestSubmitDetail();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            switch (Convert.ToInt32(dataReader["APPROVALORDER"]))
                            {
                                case 1:
                                    travelRequestSubmitDetail.TravelCoordinatorBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    break;
                                case 2:

                                    travelRequestSubmitDetail.DepartmentHeadBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.DepartmentHeadOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.DepartmentHeadOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 3:
                                    travelRequestSubmitDetail.ExecutiveOfficerBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.ExecutiveOfficerOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.ExecutiveOfficerOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 4:
                                    travelRequestSubmitDetail.CEOInternationalBadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOInternationalOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                case 5:
                                    travelRequestSubmitDetail.CEOAPTABadgeNumber = Convert.ToInt32(dataReader["BADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherBadgeNumber = string.IsNullOrEmpty(dataReader["APPROVEROTHERBADGENUMBER"].ToString()) ? 0 : Convert.ToInt32(dataReader["APPROVEROTHERBADGENUMBER"]);
                                    travelRequestSubmitDetail.CEOAPTAOtherName = dataReader["APPROVERNAME"].ToString();
                                    break;
                                
                            }
                            travelRequestSubmitDetail.TravelRequestId = dataReader["TRAVELREQUESTID"].ToString();
                            travelRequestSubmitDetail.RejectedTravelRequest = GetRejectedReimburseRequestStatus(dbConn, dataReader["TRAVELREQUESTID"].ToString());

                            string agree = string.Empty;
                            string submitter = string.Empty;
                            //  travelRequestSubmitDetail.Agree = GetAgeedAcknowledgement(dbConn, travelRequestId);
                            GetSubmitterName(dbConn, travelRequestId, out agree, out submitter);
                            travelRequestSubmitDetail.SubmitterName = submitter;
                            travelRequestSubmitDetail.Agree = (agree == "Y") ? true : false;

                        }
                    }
                    command.Dispose();
                    dataReader.Close();
                    response = new TravelRequestSubmitDetailResponse();
                    response.TravelRequestSubmitDetail = travelRequestSubmitDetail;
                    response.RequiredExecutiveOfficerApproval = getRequiredExecutiveOfficeApproval(travelRequestId, dbConn);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Log("Reimbursement : GetSubmitDetails" + ex.Message);
                throw new Exception(ex.Message);
            }
           
            return response;
        }

        private bool getRequiredExecutiveOfficeApproval(int travelRequestId, DbConnection dbConn)
        {
            try
            {
                var result = false;
                string query = string.Format("Select Total from REIMBURSE  where TRAVELREQUESTID= {0} and Total > 3000", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {

                    result = true;
                     
                }
                command.Dispose();
                dataReader.Close();
                return result;

            }
            catch (Exception ex)
            {
                LogMessage.Log("getRequiredExecutiveOfficeApproval : " + ex.Message);
                throw;
            }
        }

        private void GetSubmitterName(DbConnection dbConn, int travelRequestId, out string agree, out string submitter)
        {
            try
            {

                string response = string.Empty;
                agree = string.Empty;
                submitter = string.Empty;
                string query = string.Format("Select * from REIMBURSE_TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
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

        private bool GetRejectedReimburseRequestStatus(DbConnection dbconn, string travelRequestId)
        {
            var result = false;
            try
            {
                string query = string.Format(@"select REJECTREIMBURSEREQUEST FROM REIMBURSE_TRAVELREQUEST where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        result = (dataReader["REJECTREIMBURSEREQUEST"].ToString()) == "true" ? true : false;

                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve rejected reiumburse request status");
                }
                command.Dispose();
                dataReader.Close();
            }
            catch (Exception ex)
            {
                LogMessage.Log("GetRejectedReimburseRequestStatus : " + ex.Message);
                throw;
            }
            return result;

        }
    }
}