using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface IApprovalService
    {
        Task<List<HeirarchichalPosition>> GetHeirarchichalPositions(int badgeNumber);
        List<HeirarchichalPosition> GetTAAprovers();
        bool SubmitTravelRequest(SubmitTravelRequestData submitTravelRequestData);
        bool SubmitTravelRequestNew(SubmitTravelRequest submitTravelRequest);
        SubmitTravelRequest GetapproverDetails(string travelRequestId);
        bool SubmitReimburse(SubmitReimburseData submitReimburseData);
        bool UpdateApproveStatus(EmailApprovalDetails emailApproveDetails);

        bool UpdateRejectStatus(EmailApprovalDetails emailApproveDetails);
    }
}
