using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TravelApplication.DAL.Repositories;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public class ApprovalService : IApprovalService
    {
        IApprovalRepository approvalRepository = new ApprovalRepository();

        ITravelRequestRepository travelRequestRepository = new TravelRequestRepository();

        public SubmitTravelRequest GetapproverDetails(string travelRequestId)
        {
            SubmitTravelRequest result = approvalRepository.GetApproverDetails(travelRequestId);
            return result;
        }

        public async Task<List<HeirarchichalPosition>> GetHeirarchichalPositions(int badgeNumber)
        {
            List<HeirarchichalPosition> result = await approvalRepository.GetHeirarchichalPositions(badgeNumber).ConfigureAwait(false);
            result.Add(new HeirarchichalPosition() { BadgeNumber = -1, Name = "Other" });
            result.Add(new HeirarchichalPosition() { BadgeNumber = 0, Name = "Not Applicable" });

            return result;
        }

        public List<HeirarchichalPosition> GetTAAprovers()
        {
            List<HeirarchichalPosition> result = new List<HeirarchichalPosition>();
            //result.Add(new HeirarchichalPosition() { BadgeNumber = -1, Name = "Other" });
            result.Add(new HeirarchichalPosition() { BadgeNumber = 85163, Name = "MARIA BANUELOS" });
            return result;
        }

        public bool SubmitReimburse(SubmitReimburseData submitReimburseData)
        {
            var result = approvalRepository.SubmitReimburse(submitReimburseData);
            return result;
        }

        public bool SubmitTravelRequest(SubmitTravelRequestData submitTravelRequestData)
        {
            var result = approvalRepository.SubmitTravelRequest(submitTravelRequestData);
            return result;
        }

        public bool SubmitTravelRequestNew(SubmitTravelRequest submitTravelRequest)
        {
            var result = approvalRepository.SubmitTravelRequestNew(submitTravelRequest);
            return result;
        }

        public bool UpdateApproveStatus(EmailApprovalDetails emailApproveDetails)
        {
            var result = travelRequestRepository.Approve(emailApproveDetails.BadgeNumber, emailApproveDetails.TravelRequestId, emailApproveDetails.Comments);
            return result;
        }

        public bool UpdateRejectStatus(EmailApprovalDetails emailApproveDetails)
        {
            var result = travelRequestRepository.Reject(emailApproveDetails);
            return result;
        }
    }
}