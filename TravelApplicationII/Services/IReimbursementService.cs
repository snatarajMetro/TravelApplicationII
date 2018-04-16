using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface IReimbursementService
    {
        List<TravelRequestDetails> GetApprovedTravelrequestList(int badgeNumber, int selectedRoleId);
        ReimbursementAllTravelInformation GetTravelRequestInfoForReimbursement(string travelRequestId);
        string SaveTravelRequestReimbursement(ReimbursementInput reimbursementRequest);
        List<ReimburseGridDetails> GetReimbursementRequests(int badgeNumber, int roleId);
        ReimbursementInput GetReimbursementDetails(string travelRequestId);
        bool Approve(int badgeNumber, string travelRequestId, string comments);
        bool Reject(int approverBadgeNumber, int travelRequestBadgeNumber, string travelRequestId, string comments , string rejectReason);

        TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId);
    }
}