using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelApplication.Models;

namespace TravelApplication.DAL.Repositories
{
    public interface IFISRepository
    {
        Task<List<CostCenter>> GetCostCenters();
        Task<List<Project>> GetProjectsByCostCenterName(string costCenterName);
        FIS GetFISdetails(DbConnection dbConn, string travelRequestId);
        FIS GetFISdetailsForReimburse(DbConnection dbConn, string travelRequestId);
    }
}
