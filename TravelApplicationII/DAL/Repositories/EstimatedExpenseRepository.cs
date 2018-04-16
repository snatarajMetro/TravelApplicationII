using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.Class.Common;
using TravelApplication.DAL.DBProvider;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public class EstimatedExpenseRepository : IEstimatedExpenseRepository
    {
        private DbConnection dbConn;

        public EstimatedExpense GetTravelRequestDetail(int travelRequestId)
        {
            try
            {

            EstimatedExpense response = null;
            using (dbConn = ConnectionFactory.GetOpenDefaultConnection())
            {
                string query = string.Format("Select * from TRAVELREQUEST_ESTIMATEDEXPENSE where TRAVELREQUESTID= {0}", travelRequestId);
                OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
                command.CommandText = query;
                DbDataReader dataReader = command.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        response = new EstimatedExpense()
                        {
                            EstimatedExpenseId = Convert.ToInt32(dataReader["ESTIMATEDEXPENSEID"]),
                            TravelRequestId = Convert.ToInt32(dataReader["TRAVELREQUESTID"]),
                            AdvanceLodging = Convert.ToInt32(dataReader["ADVANCELODGING"]),
                            AdvanceAirFare = Convert.ToInt32(dataReader["ADVANCEAIRFARE"]),
                            AdvanceRegistration = Convert.ToInt32(dataReader["ADVANCEREGISTRATION"]),
                            AdvanceMeals = Convert.ToInt32(dataReader["ADVANCEMEALS"]),
                            AdvanceCarRental = Convert.ToInt32(dataReader["ADVANCECARRENTAL"]),
                            AdvanceMiscellaneous = Convert.ToInt32(dataReader["ADVANCEMISCELLANEOUS"]),
                            AdvanceTotal = Convert.ToInt32(dataReader["ADVANCETOTAL"]),
                            TotalEstimatedLodge = Convert.ToInt32(dataReader["TOTALESTIMATEDLODGE"]),
                            TotalEstimatedAirFare = Convert.ToInt32(dataReader["TOTALESTIMATEDAIRFARE"]),
                            TotalEstimatedRegistration = Convert.ToInt32(dataReader["TOTALESTIMATEDREGISTRATION"]),
                            TotalEstimatedMeals = Convert.ToInt32(dataReader["TOTALESTIMATEDMEALS"]),
                            TotalEstimatedCarRental = Convert.ToInt32(dataReader["TOTALESTIMATEDCARRENTAL"]),
                            TotalEstimatedMiscellaneous = Convert.ToInt32(dataReader["TOTALESTIMATEDMISCELLANEOUS"]),
                            TotalEstimatedTotal = Convert.ToInt32(dataReader["TOTALESTIMATEDTOTAL"]),
                            HotelNameAndAddress = dataReader["HOTELNAMEANDADDRESS"].ToString(),
                            AgencyNameAndReservation = dataReader["AGENCYNAMEANDRESERVATION"].ToString(),
                            PayableToAndAddress = dataReader["PAYABLETOANDADDRESS"].ToString(),
                            Shuttle = dataReader["SHUTTLE"].ToString(),
                            Schedule = dataReader["SCHEDULE"].ToString(),
                            CashAdvance = Convert.ToInt32(dataReader["CASHADVANCE"]),
                            DateNeededBy = Convert.ToDateTime(dataReader["DATENEEDEDBY"]),
                            Note =  dataReader["NOTE"].ToString()
                        };
                    }
                }
                else
                {
                    throw new Exception("Couldn't retrieve travel request");
                }
                command.Dispose();
                dbConn.Close();
                dbConn.Dispose();
            }
            return response;

            }
            catch (Exception ex )
            {
                LogMessage.Log("Estimated expense repository : GetTravelRequestDetail : " + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> SaveEstimatedExpenseRequest(EstimatedExpense request)
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
						DATENEEDEDBY					 
						 ) VALUES (:p1 ,:p2,:p3,:p4,:p5,:p6,:p7,:p8,:p9,:p10,:p11,:p12,:p13,:p14,:p15,:p16,:p17,:p18,:p19,:p20,:p21,:p22,:p23 ) returning ESTIMATEDEXPENSEID into : estimatedExpenseId ";
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
						DATENEEDEDBY  = :p23
                        WHERE TRAVELREQUESTID = {0}",request.TravelRequestId);
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
                LogMessage.Log("Estimated expense repository : SaveEstimatedExpenseRequest : " + ex.Message);
                throw new Exception("Could not save estimated expense successfully");
            }

            return estimatedExpenseId;
        }

        public EstimatedExpense GetTravelRequestDetailNew(DbConnection dbConn, string travelRequestId)
        {
            EstimatedExpense response = null;
            string query = string.Format("Select * from TRAVELREQUEST_ESTIMATEDEXPENSE where TRAVELREQUESTID= {0}", travelRequestId);
            OracleCommand command = new OracleCommand(query, (OracleConnection)dbConn);
            command.CommandText = query;
            DbDataReader dataReader = command.ExecuteReader();
            try
            {
            
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    response = new EstimatedExpense()
                    {
                        EstimatedExpenseId = Convert.ToInt32(dataReader["ESTIMATEDEXPENSEID"]),
                        TravelRequestId = Convert.ToInt32(dataReader["TRAVELREQUESTID"]),
                        AdvanceLodging = Convert.ToInt32(dataReader["ADVANCELODGING"]),
                        AdvanceAirFare = Convert.ToInt32(dataReader["ADVANCEAIRFARE"]),
                        AdvanceRegistration = Convert.ToInt32(dataReader["ADVANCEREGISTRATION"]),
                        AdvanceMeals = Convert.ToInt32(dataReader["ADVANCEMEALS"]),
                        AdvanceCarRental = Convert.ToInt32(dataReader["ADVANCECARRENTAL"]),
                        AdvanceMiscellaneous = Convert.ToInt32(dataReader["ADVANCEMISCELLANEOUS"]),
                        AdvanceTotal = Convert.ToInt32(dataReader["ADVANCETOTAL"]),
                        TotalEstimatedLodge = Convert.ToInt32(dataReader["TOTALESTIMATEDLODGE"]),
                        TotalEstimatedAirFare = Convert.ToInt32(dataReader["TOTALESTIMATEDAIRFARE"]),
                        TotalEstimatedRegistration = Convert.ToInt32(dataReader["TOTALESTIMATEDREGISTRATION"]),
                        TotalEstimatedMeals = Convert.ToInt32(dataReader["TOTALESTIMATEDMEALS"]),
                        TotalEstimatedCarRental = Convert.ToInt32(dataReader["TOTALESTIMATEDCARRENTAL"]),
                        TotalEstimatedMiscellaneous = Convert.ToInt32(dataReader["TOTALESTIMATEDMISCELLANEOUS"]),
                        TotalEstimatedTotal = Convert.ToInt32(dataReader["TOTALESTIMATEDTOTAL"]),
                        HotelNameAndAddress = dataReader["HOTELNAMEANDADDRESS"].ToString(),
                        AgencyNameAndReservation = dataReader["AGENCYNAMEANDRESERVATION"].ToString(),
                        PayableToAndAddress = dataReader["PAYABLETOANDADDRESS"].ToString(),
                        Shuttle = dataReader["SHUTTLE"].ToString(),
                        Schedule = dataReader["SCHEDULE"].ToString(),
                        CashAdvance = Convert.ToInt32(dataReader["CASHADVANCE"]),
                        DateNeededBy = Convert.ToDateTime(dataReader["DATENEEDEDBY"]),
                        Note = dataReader["NOTE"].ToString(),
                        TotalOtherEstimatedLodge = Convert.ToInt32(dataReader["APPROVEDLODGE"]),
                        TotalOtherEstimatedAirFare = Convert.ToInt32(dataReader["APPROVEDAIRFARE"]),
                        TotalOtherEstimatedMeals = Convert.ToInt32(dataReader["APPROVEDMEALS"]),
                        TotalActualEstimatedLodge = Convert.ToInt32(dataReader["ACTUALLODGE"]),
                        TotalActualEstimatedAirFare = Convert.ToInt32(dataReader["ACTUALAIRFARE"]),
                        TotalActualEstimatedMeals = Convert.ToInt32(dataReader["ACTUALMEALS"]),
                        PersonalTravelExpense = (string.IsNullOrEmpty(dataReader["PERSONALTRAVELEXPENSE"].ToString())) ? 0 : Convert.ToDecimal(dataReader["PERSONALTRAVELEXPENSE"]),
                        PersonalTravelIncluded =(string.IsNullOrEmpty(dataReader["PERSONALTRAVELINCLUDED"].ToString())) ? false: Convert.ToBoolean(dataReader["PERSONALTRAVELINCLUDED"]),
                        SpecialInstruction = dataReader["SPECIALINSTRUCTIONS"].ToString()

                    };                        
                }

            }
            else
            {
                throw new Exception("Couldn't retrieve travel request");
            }

            }
            catch (Exception ex)
            {
                LogMessage.Log("Estimated expense repository: GetTravelRequestDetailNew" + ex.Message);
                throw;
            }
            command.Dispose();
            return response;
        }
    }
}