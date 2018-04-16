using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public interface IReimbursementRepository
    {
        List<TravelRequestDetails> GetApprovedTravelRequestList(int submittedBadgeNumber, int selectedRoleId);
        ReimbursementAllTravelInformation GetTravelRequestInfoForReimbursement(string travelRequestId);
        ReimbursementTravelRequestDetails GetTravelReimbursementDetails(DbConnection dbConn, string travelRequestId);
        string SaveTravelRequestReimbursement(ReimbursementInput reimbursementRequest);
        ReimbursementInput GetAllReimbursementDetails(string travelRequestId);
        List<ReimburseGridDetails> GetReimbursementRequestsList(int badgeNumber, int selectedRoleId);

        bool Approve(int badgeNumber, string travelRequestId, string comments);
        bool Reject(int approverBadgeNumber, int travelRequestBadgeNumber, string travelRequestId, string comments, string rejectReason);
        TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId);
    }
}