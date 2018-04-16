using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelApplication.DAL.Repositories;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public class ReimbursementService : IReimbursementService
    {
        IReimbursementRepository reimbursementRepository = new ReimbursementRepository();
        public List<TravelRequestDetails> GetApprovedTravelrequestList(int badgeNumber, int selectedRoleId)
        {
            var result = reimbursementRepository.GetApprovedTravelRequestList(badgeNumber, selectedRoleId);
            return result;
        }

        public ReimbursementInput GetReimbursementDetails(string travelRequestId)
        {
            var result = reimbursementRepository.GetAllReimbursementDetails(travelRequestId);
            return result;
        }

        public List<ReimburseGridDetails> GetReimbursementRequests(int badgeNumber, int roleId)
        {
            var result = reimbursementRepository.GetReimbursementRequestsList(badgeNumber, roleId);
            return result;
        }

        public ReimbursementAllTravelInformation GetTravelRequestInfoForReimbursement(string travelRequestId)
        {
            ReimbursementAllTravelInformation result = reimbursementRepository.GetTravelRequestInfoForReimbursement(travelRequestId);
            return result;
        }

        public string SaveTravelRequestReimbursement(ReimbursementInput reimbursementRequest)
        {
            try
            {
                string reimbursementId = reimbursementRepository.SaveTravelRequestReimbursement(reimbursementRequest);
                return reimbursementId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public bool Approve(int badgeNumber, string travelRequestId, string comments)
        {
            var result = reimbursementRepository.Approve(badgeNumber, travelRequestId, comments);
            return result;
        }

        public bool Reject(int approverBadgeNumber, int travelRequestBadgeNumber, string travelRequestId, string comments, string rejectReason)
        {
            var result = reimbursementRepository.Reject(approverBadgeNumber, travelRequestBadgeNumber, travelRequestId, comments, rejectReason);
            return result;
        }

        public TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId)
        {
            TravelRequestSubmitDetailResponse result = reimbursementRepository.GetSubmitDetails(travelRequestId);
            return result;
        }
    }
}