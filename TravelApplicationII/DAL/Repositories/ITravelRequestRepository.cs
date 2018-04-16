using System.Collections.Generic;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.Services
{
    public interface ITravelRequestRepository
    {
        Task<EmployeeDetails> GetEmployeeDetails(int BadgeNumber);
        Task<int> SaveTravelRequest(TravelRequest request);
        TravelRequest GetTravelRequestDetail(int travelRequestId);
        List<TravelRequestDetails> GetTravelRequestList(int badgeNumber, int selectedRoleId);
        bool Approve(int badgeNumber, string travelRequestId, string comments);
        bool Reject(ApproveRequest approveRequest);
        TravelRequestInputResponse SaveTravelRequestInput(TravelRequestInput travelRequest);
        TravelRequestInput GetTravelRequestDetailNew(string travelRequestId);
        Task<string> GetVendorNumber(int badgeNumber);
        TravelRequestSubmitDetailResponse GetSubmitDetails(int travelRequestId);
        bool Cancel(string travelRequestId, int travelRequestBadgeNumber, string comments);
        List<TravelRequestDashboard> GetTravelRequestDashboardData();
        List<TravelRequestDashboard> GetTravelReimbursementDashboardData();

    }
}